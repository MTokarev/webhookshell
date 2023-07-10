using System;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Webhookshell.Interfaces;
using Webhookshell.Models;
using Webhookshell.Options;

namespace Webhookshell.Services
{
    public class ScriptRunner : IScriptRunnerService
    {
        private readonly ScriptOptions _options;
        private readonly IHandlerDispatcher _handlerDispatcher;
        private readonly IScriptValidationService _validationService;

        public ScriptRunner(IHandlerDispatcher handlerDispatcher,
          IScriptValidationService validationService,
          IOptionsSnapshot<ScriptOptions> options)
        {
            _handlerDispatcher = handlerDispatcher;
            _validationService = validationService;
            _options = options.Value;
        }
        public Result<DtoResult> Run(DtoScript scriptToRun, HttpContext httpContext)
        {
            Result<DtoResult> scriptRunResult = _validationService.Validate(scriptToRun, httpContext);

            if (scriptRunResult.IsNotValid)
            {
                return scriptRunResult;
            }

            Result<ScriptHandler> handlerResult = _handlerDispatcher.GetScriptHandler(scriptToRun);

            ProcessToRun processToRun = ProcessBuilder(scriptToRun, handler: handlerResult.Data);

            scriptRunResult.Data = new DtoResult
            {
                ScriptName = scriptToRun.Script,
                Param = scriptToRun.Param,
                Output = ExecuteScript(processToRun)
            };

            return scriptRunResult;
        }


        private string ExecuteScript(ProcessToRun processToRun)
        {
            ProcessStartInfo processToStart = new()
            {
                FileName = processToRun.ProcessName,
                Arguments = processToRun.ScriptWithArgs,
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
										\n'{processToRun.ScriptWithArgs} failed. Output (stdOut): {stdout}' Error (stdErr): {stderr}");
            }
            return stdout;
        }

        private ProcessToRun ProcessBuilder(DtoScript scriptToRun, ScriptHandler handler)
        {

            ProcessToRun processToRun = new();
            var scriptPath = Path.Combine(handler.ScriptsLocation, scriptToRun.Script);
            processToRun.ProcessName = handler.ProcessName;
            processToRun.ScriptWithArgs = $"{scriptPath} {scriptToRun.Param}";

            return processToRun;
        }
    }
}