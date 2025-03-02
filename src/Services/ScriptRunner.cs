using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Webhookshell.Interfaces;
using Webhookshell.Models;
using Webhookshell.Options;

namespace Webhookshell.Services
{
    /// <summary>
    /// Script runner service
    /// </summary>
    public class ScriptRunner : IScriptRunnerService
    {
        private readonly ScriptOptions _options;
        private readonly IHandlerDispatcher _handlerDispatcher;
        private readonly IScriptValidationService _validationService;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ScriptRunner(IHandlerDispatcher handlerDispatcher,
          IScriptValidationService validationService,
          IOptionsSnapshot<ScriptOptions> options)
        {
            _handlerDispatcher = handlerDispatcher;
            _validationService = validationService;
            _options = options.Value;
        }
        
        /// <summary>
        /// Run given script
        /// </summary>
        /// <param name="scriptToRun">Script to run. <seealso cref="DtoScript"/></param>
        /// <param name="httpContext">Http context <seealso cref="HttpContext"/></param>
        /// <returns>Script execution result. <seealso cref="Result{T}"/></returns>
        public Result<DtoResult> Run(DtoScript scriptToRun, HttpContext httpContext)
        {
            Result<DtoResult> scriptRunResult = _validationService.Validate(scriptToRun, httpContext);

            if (scriptRunResult.IsNotValid)
            {
                return scriptRunResult;
            }

            Result<ScriptHandler> handlerResult = _handlerDispatcher.GetScriptHandler(scriptToRun);
            ProcessToRun processToRun = ProcessBuilder(scriptToRun, handler: handlerResult.Data);

            var scriptExecutionResult = ExecuteScript(processToRun);
            if (scriptExecutionResult.IsNotValid)
            {
                scriptRunResult.Errors.AddRange(scriptExecutionResult.Errors);
                return scriptRunResult;
            }
            
            scriptRunResult.Data = new DtoResult
            {
                ScriptName = scriptToRun.Script,
                Params = scriptToRun.Params,
                Output = scriptExecutionResult.Data 
            };

            return scriptRunResult;
        }

        private Result<string> ExecuteScript(ProcessToRun processToRun)
        {
            Result<string> scriptRunResult = new();
            ProcessStartInfo processToStart = new()
            {
                FileName = processToRun.ProcessName,
                Arguments = processToRun.ScriptWithArgs,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };
            try
            {
                using Process process = Process.Start(processToStart);
                using StreamReader reader = process.StandardOutput;

                string stderr = process.StandardError.ReadToEnd();
                string stdout = reader.ReadToEnd();
                scriptRunResult.Data = stdout;
                if (!string.IsNullOrEmpty(stderr))
                {
                    scriptRunResult.Errors.Add($@"Exception occurred in script execution.
										    \n'{processToRun.ScriptWithArgs} failed. Output (stdOut): {stdout}' Error (stdErr): {stderr}");
                }
            }
            catch (Win32Exception e)
            {
                scriptRunResult.Errors.Add($"Exception occurred in launching script runner. It might happen when you don't have '{processToRun.ProcessName}' installed and added to the PATH. Message: '{e.Message}'. Exception: '{e}'.");
            }
            
            return scriptRunResult;
        }

        private ProcessToRun ProcessBuilder(DtoScript scriptToRun, ScriptHandler handler)
        {

            ProcessToRun processToRun = new();
            var scriptPath = Path.Combine(handler.ScriptsLocation, scriptToRun.Script);
            processToRun.ProcessName = handler.ProcessName;
            processToRun.ScriptWithArgs = $"{scriptPath} {scriptToRun.Params}";

            return processToRun;
        }
    }
}