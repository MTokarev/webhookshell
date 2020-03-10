# WebHookShell

### Content
- [WebHookShell](#webhookshell)
    - [Content](#content)
  - [Product Overview](#product-overview)
  - [How it works](#how-it-works)
  - [Supported Shells](#supported-shells)
  - [Setting up](#setting-up)
    - [Supported OSs](#supported-oss)
    - [Script locations](#script-locations)
    - [Adding key](#adding-key)
  - [Install on windows](#install-on-windows)
  - [Make sure it works](#make-sure-it-works)
  - [Security Context](#security-context)
  - [How to add another providers](#how-to-add-another-providers)


## Product Overview
In the modern cloud world you have different approaches to run scripts using WebHooks, for instance Azure Functions, Azure Automation or Amazon Lambda.
In some cases users tend to use tools like Jenkins just to provide ability to trigger the function execution (overkill for webhooks).
This small lightweight project was born to make an Open Source Alternative that you can easily deploy on both Linux or Windows box.

## How it works
User send the HTTP GET message to the server. The message includes following parameters:

1. Key
2. Script name
3. Param *[Optional]*

base URI:

`https://servename/webhook/v1?key=yourKey&script=YourScript&param=-Your -Params`

In order to protect a web hook from accidental execution, server will load the **Key** from appsettings.json and compare with the key provided in request. If doesn't match, server stop executing pipeline and return an error:

![img](~/../Docs/Img/keyerror.png)

If any exceptions arise along the way, global exception handler will return an error:

![img](~/../Docs/Img/scriptexecutionfailed.png)

**Example** of succesfful webhook without params:

`https://localhost:5001/webhook/v1?key=24ffc5be-7dd8-479f-898e-27169bf23e7f&script=Test-Script.ps1`

![img](~/../Docs/Img/scriptexecutedsuccesfully.png)

With parameters:

`https://localhost:5001/webhook/v1?key=24ffc5be-7dd8-479f-898e-27169bf23e7f&script=Test-Script.ps1&param=-Param1 A -Param2 B`

![img](~/../Docs/Img/scriptexecutedsuccesfully2.png)

## Supported Shells

Currently this project supports PowerShell and Python, but can be easily extended to use whatever you want to trigger. In order to do so, just change a class **ScriptRunner.cs** and add your shell into the **ProcessBuilder()** method.

## Setting up

### Supported OSs

This project was tested on Windows, but supports Linux as well.
For powershell you need to have powershell installed and configured inside PATH variable (default for windows) and for python you have to have it in PATH as well.

### Script locations

In the appsettings file there is a section **ScriptLocations** that contain path to scripts, default value:

```json
"ScriptLocations": {
  "PowerShell": ".\\powershellscripts",
  "Python": ".\\pythonscripts"
  }
```
### Adding key

Key is stored in appsettings file and can be any string you like:

```
"key": "24ffc5be-7dd8-479f-898e-27169bf23e7f"
```

## Install on windows

Open PowerShell as an administrator and copy this super formatted line:

**Before You do that**

*You can change variables to reflect the path and names for your config.*

```
$tmpFile="C:\Windows\Temp\win-x64.zip";$tmpHostingEnv="C:\Windows\Temp\ihe.exe";$webAppLocation="C:\inetpub\webhookshell";$webSiteName="webHookShell";$AppPoolName="webhookshell";Write-Host -ForegroundColor Green "[+]Spelling magic, it wouldn't take more than ages...";Write-Host -ForegroundColor Green "`t-Downloading binaries from github";[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12;Invoke-WebRequest https://github.com/MTokarev/webhookshell/blob/master/bin/Debug/netcoreapp3.1/win-x64.zip?raw=true -OutFile $tmpFile;New-Item -Path $webAppLocation -Type Directory | Out-Null;Write-Host -ForegroundColor Green "`t-Expanding archive $tmpFile to $webAppLocation";[System.Reflection.Assembly]::LoadWithPartialName("System.IO.Compression.FileSystem") | Out-Null;[System.IO.Compression.ZipFile]::ExtractToDirectory($tmpFile, $webAppLocation);Write-Host -ForegroundColor Green "`t-Installing IIS";Install-WindowsFeature Web-Server -IncludeManagementTools | Out-Null;Write-Host -ForegroundColor Green "`t-Downloading Hosting Bundle from MSFT to support ASP.NET Core behind IIS";Invoke-WebRequest https://download.visualstudio.microsoft.com/download/pr/dd119832-dc46-4ccf-bc12-69e7bfa61b18/990843c6e0cbd97f9df68c94f6de6bb6/dotnet-hosting-3.1.2-win.exe -OutFile $tmpHostingEnv;Write-Host -ForegroundColor Green "`t-Installing Hosting Bundle in zero touch mode";Start-Process -FilePath $tmpHostingEnv -Wait -ArgumentList /passive;iisreset | Out-Null;New-WebAppPool -name $AppPoolName -Force | Out-Null;$appPool = Get-Item "IIS:\AppPools\$AppPoolName";$appPool.processModel.identityType = "LocalSystem";$appPool | Set-Item;Write-Host -ForegroundColor Green "`t-Registring Site in IIS";new-WebSite -name $webSiteName -PhysicalPath "$webAppLocation" -ApplicationPool $AppPoolName -force | Out-Null;Write-Host -ForegroundColor Green "`t-Allowing HTTP port passing Windows Firewall";New-NetFirewallRule -DisplayName "HTTP-TCP-80" -Direction Inbound -LocalPort 80 -Protocol TCP -Action Allow | Out-Null;Write-Host -ForegroundColor Green "`t***Execution has been completed***"

```

![img](~/../Docs/Img/installationOnWindows.png)

This script will:

1. Download the binary from github
2. Download Hosting bundle from MSFT (to support asp.net core behind IIS)
3. Install IIS
4. Create and configure website
5. Add firewall exception for HTTP port 

## Make sure it works

By default I included one ps1 script Test-Script.ps1 that you can run.
Script has 3 option parameters:

```
-Param1 [string]
-Param2 [string]
```
If you provide directory `-Param1 A -Param2 B` then script shoult return them (as in the screenshot above). It will confirm that the app is configured properly. 

## Security Context

By default web app on windows uses applicationPool context (LocalSystem) which means that it has a local admin access. You can provide any credentials inside your ps1 script by using your own logic, or you can change appSetting username in the IIS to change the context.

## How to add another providers

For this task you have to download the project and extend ScriptRunner.cs service with your provider. Here is how it looks like:

```cs
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
```
As you can see the only thing you need to do, beside adding your script folder into the appsettings.json, just add your file extension and the proccess name you need to call out.