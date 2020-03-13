using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using webhookshell.Interfaces;
using webhookshell.Models;

namespace webhookshell.Services
{
    public class ScriptRunner : IScriptRunner
    {
        private readonly IConfiguration _config;

        public ScriptRunner(IConfiguration config)
        {
            _config = config;
        }
        public string Run(DTOScript scriptToRun)
        {

            ProcessToRun processToRun = ProcessBuilder(scriptToRun);
            string stdout = ExecuteScript(processToRun);
            return stdout;
        }
        private string ExecuteScript(ProcessToRun processToRun)
        {
            ProcessStartInfo processToStart = new ProcessStartInfo();
            processToStart.FileName = processToRun.processName;
            processToStart.Arguments = processToRun.scriptWithArgs;
            processToStart.CreateNoWindow = true;
            processToStart.RedirectStandardError = true;
            processToStart.RedirectStandardOutput = true;

            using(Process process = Process.Start(processToStart))
            {
                using(StreamReader reader = process.StandardOutput)
                {
                    string stderr = process.StandardError.ReadToEnd();
                    string stdout = reader.ReadToEnd();
                    if(!String.IsNullOrEmpty(stderr))
                    {
                        throw new ApplicationException($@"Exception occurred in script execution.
                                        \n'{processToRun.scriptWithArgs} failed. Output (stdOut): {stdout}' Error (stdErr): {stderr}");
                    }
                    return stdout;
                }
            }
        }

        private ProcessToRun ProcessBuilder(DTOScript scriptToRun)
        {
            
            ProcessToRun processToRun = new ProcessToRun();
            switch (scriptToRun.script.Split(".", StringSplitOptions.RemoveEmptyEntries).Last())
            {
                case "ps1":
                    string path = _config.GetValue<string>("ScriptLocations:Powershell");
                    var scriptPath = Path.Combine(path, scriptToRun.script);
                    processToRun.processName = "powershell";
                    processToRun.scriptWithArgs = $"{scriptPath} {scriptToRun.param}";
                    break;
                case "py":
                    string pathPY = _config.GetValue<string>("ScriptLocations:Python");
                    var scriptPathPY = Path.Combine(pathPY, scriptToRun.script);
                    processToRun.processName = "python";
                    processToRun.scriptWithArgs = $"{scriptPathPY} {scriptToRun.param}";
                    break;
                default:
                    throw new InvalidDataException($"Unsupported file type: {scriptToRun.script}");

            }
            return processToRun;
        }
    }
}