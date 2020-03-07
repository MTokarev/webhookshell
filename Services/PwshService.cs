using System;
using System.IO;
using System.Management.Automation;
using Microsoft.Extensions.Configuration;
using webhookshell.Interfaces;
using webhookshell.Models;

namespace webhookshell.Services
{
    public class PwshService : IPwshService
    {
        private readonly IConfiguration _config;

        public PwshService(IConfiguration config)
        {
            _config = config;
        }
        public bool RunPswh(DTOPswh pwsh)
        {
            
            string path = _config.GetValue<string>("ScriptLocations:Powershell");
            var scriptPath = Path.Combine(path, pwsh.script);
            
            if(!File.Exists(scriptPath))
            {
                throw new FileNotFoundException($"File not found in the directory: {scriptPath}");
            }

            using (PowerShell ps = PowerShell.Create())
            {
                // Allowing to run .ps1 scripts in powershell
                ps.AddScript("Set-ExecutionPolicy Unrestricted");
                // Adding script with parameters if any
                ps.AddScript(scriptPath + $" {pwsh.param}");          

                var results = ps.Invoke();
                
                // If any error occurs in powershell throw an Exception
                if(ps.HadErrors)
                {
                    //Reading all errors
                    var error = ps.Streams.Error.ReadAll();
                    throw new Exception($"Unable to execute powershell script {pwsh.script}. Exception: {String.Join(",", error)}");
                }
                
                // Clearing pipeline
                ps.Commands.Clear();
            }
            return true;
        }
    }
}