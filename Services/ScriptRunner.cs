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

        public ScriptRunner(IOptionsSnapshot<ScriptOptions> options)
        {
            _options = options.Value;
        }
        public Result<DTOResult> Run(DTOScript scriptToRun)
        {
            Result<ScriptHandlerOptions> validationResult = CheckIfScriptIsValid(scriptToRun);
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
                ScriptName = scriptToRun.Script,
                Param = scriptToRun.Param,
                Output = ExecuteScript(processToRun)
            };

            return scriptRunResult;
        }

        private Result<ScriptHandlerOptions> CheckIfScriptIsValid(DTOScript scriptToRun)
        {
            Result<ScriptHandlerOptions> result = new();

            string scriptExtension = scriptToRun
                .Script
                .Split(".", StringSplitOptions.RemoveEmptyEntries)
                .Last();
            
            if (scriptExtension is null)
            {
                result.Errors.Add($"Unable to extract script extension from '{scriptToRun.Script}'.");
                return result;
            }
            
            ScriptHandlerOptions handler = _options
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

            if (!handler.KeysMapping.TryGetValue(scriptToRun.Script, out string key))
            {
                key = handler.Key ?? _options.DefaultKey;
            }

            if (!string.Equals(key, scriptToRun.Key))
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

        private ProcessToRun ProcessBuilder(DTOScript scriptToRun, ScriptHandlerOptions handler)
        {
            
            ProcessToRun processToRun = new();
            var scriptPath = Path.Combine(handler.ScriptsLocation, scriptToRun.Script);
            processToRun.processName = handler.ProcessName;
            processToRun.scriptWithArgs = $"{scriptPath} {scriptToRun.Param}";
            
            return processToRun;
        }
    }
}