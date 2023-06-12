using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Options;
using webhookshell.Interfaces;
using webhookshell.Models;
using webhookshell.Options;

namespace webhookshell.Services
{
    public class ScriptRunner : IScriptRunner
    {
        private readonly ScriptOptions _options;

        public ScriptRunner(IOptions<ScriptOptions> options)
        {
            _options = options.Value;
        }
        public Result<DTOResult> Run(DTOScript scriptToRun)
        {
            Result<ScriptHandler> validationResult = CheckIfScriptIsValid(scriptToRun);
            Result<DTOResult> scriptRunResult = new();

            if (!validationResult.IsValid)
            {
                scriptRunResult
                    .Errors
                    .AddRange(validationResult.Errors);
                
                return scriptRunResult;
            }

            ProcessToRun processToRun = ProcessBuilder(scriptToRun, handler: validationResult.Data);

            scriptRunResult.Data = new DTOResult{
                ScriptName = scriptToRun.script,
                Param = scriptToRun.param,
                Output = ExecuteScript(processToRun)
            };

            return scriptRunResult;
        }

        private Result<ScriptHandler> CheckIfScriptIsValid(DTOScript scriptToRun)
        {
            Result<ScriptHandler> result = new();

            string scriptExtension = scriptToRun
                .script
                .Split(".", StringSplitOptions.RemoveEmptyEntries)
                .Last();
            
            if (scriptExtension is null)
            {
                result.Errors.Add($"Unable to extract script extension from '{scriptToRun.script}'.");
                return result;
            }
            
            ScriptHandler handler = _options
                .Handlers
                .Where(script => string.Equals(script.FileExtension, scriptExtension, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault();

            if (handler is null)
            {
                result.Errors.Add($"Unable to find a handler for the script extension '{scriptExtension}'. You need to add the handler to the service config.");

                return result;
            }
            else
            {
                result.Data = handler;
            }

            if (!handler.KeysMapping.TryGetValue(scriptToRun.script, out string key))
            {
                key = handler.Key ?? _options.DefaultKey;
            }

            if (!string.Equals(key, scriptToRun.key))
            {
                result.Errors.Add($"Invalid security key, please double check it and run command again.");
            }

            return result;
        }

        private string ExecuteScript(ProcessToRun processToRun)
        {
            ProcessStartInfo processToStart = new()
            {
                FileName = processToRun.processName,
                Arguments = processToRun.scriptWithArgs,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

            using Process process = Process.Start(processToStart);
            using StreamReader reader = process.StandardOutput;
            
            string stderr = process.StandardError.ReadToEnd();
            string stdout = reader.ReadToEnd();
            if (!string.IsNullOrEmpty(stderr))
            {
                throw new ApplicationException($@"Exception occurred in script execution.
                                        \n'{processToRun.scriptWithArgs} failed. Output (stdOut): {stdout}' Error (stdErr): {stderr}");
            }
            return stdout;
        }

        private ProcessToRun ProcessBuilder(DTOScript scriptToRun, ScriptHandler handler)
        {
            
            ProcessToRun processToRun = new();
            var scriptPath = Path.Combine(handler.ScriptsLocation, scriptToRun.script);
            processToRun.processName = handler.ProcessName;
            processToRun.scriptWithArgs = $"{scriptPath} {scriptToRun.param}";
            
            return processToRun;
        }
    }
}