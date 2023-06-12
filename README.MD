# WebHookShell

### Content
- [WebHookShell](#webhookshell)
    - [Content](#content)
  - [Product Overview](#product-overview)
  - [How it works](#how-it-works)
  - [Setting up](#setting-up)
    - [Supported OSs](#supported-oss)
    - [Key configuration](#key-configuration)
  - [Install](#install)
    - [Windows](#windows)
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

`https://localhost:5001/webhook/v1?key=yourKey&script=YourScript&param=-Your -Params`

In order to protect a web hook from accidental execution, server will load the **Key** from appsettings.json and compare with the key provided from the request. If it doesn't match, then server stop executing pipeline and return an error:

![img](~/../Docs/Img/keyerror.png)

If any exceptions arise along the way, global exception handler will return an error:

![img](~/../Docs/Img/scriptexecutionfailed.png)

**Example** of successful webhook without params:

`https://localhost:5001/webhook/v1?key=24ffc5be-7dd8-479f-898e-27169bf23e7f&script=Test-Script.ps1`

![img](~/../Docs/Img/scriptexecutedsuccesfully.png)

With parameters:

`https://localhost:5001/webhook/v1?key=24ffc5be-7dd8-479f-898e-27169bf23e7f&script=Test-Script.ps1&param=-Param1 A -Param2 B`

![img](~/../Docs/Img/scriptexecutedsuccesfully2.png)

## Setting up

### Supported OSs

This project was tested on Windows, but supports Linux as well.
For powershell you need to have powershell installed and configured inside PATH variable (default for windows) and for python you have to have it in PATH as well.

### Key configuration

There are few places where you can define the key:

1. You can provide a key per each script. In this case you don't need to share a common one.
2. Per script handler. All scripts that are registered under the handler will share the same key if they don't have a unique key (check option 1)
3. Global key. This key will be used if the script and handler don't have keys. 

Example:
```json
"Scripts": {
    "DefaultKey": "24ffc5be-7dd8-479f-898e-27169bf23e7f",
    "Handlers": [
      {
        "ProcessName": "pwsh",
        "ScriptsLocation": "./powershellscripts",
        "FileExtension": "ps1",
        "Key": "aaa"
      },
      {
        "ProcessName": "python3",
        "ScriptsLocation": "./pythonscripts",
        "FileExtension": "py",
        "KeysMapping": {
          "test-script.py": "77aae8aa-50d2-49d9-be8c-e9f59aaf39e9"
        }
      }
    ]
  }
```

Here `test-script.py` has the unique key `77aae8aa-50d2-49d9-be8c-e9f59aaf39e9`.

## Install

This is the cross platform app and can run on Mac, Windows or Linux.
If you want to run it on Linux you need to compile the app first.

I need to do a better job setting up a release pipeline that will publish the artifact to the github for different version.

However, I am going to keep instruction for Windows.

### Windows

Open PowerShell as an administrator and copy this super formatted line:

**Before You do that**

*You can change variables to reflect the path and names for your config.*

```ps
$tmpFile="C:\Windows\Temp\win-x64.zip";$tmpHostingEnv="C:\Windows\Temp\ihe.exe";$webAppLocation="C:\inetpub\webhookshell";$webSiteName="webHookShell";$AppPoolName="webhookshell";Write-Host -ForegroundColor Green "[+]Spelling magic, it wouldn't take more than ages...";Write-Host -ForegroundColor Green "`t-Downloading binaries from github";[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12;Invoke-WebRequest https://github.com/MTokarev/webhookshell/blob/master/bin/Debug/netcoreapp3.1/win-x64.zip?raw=true -OutFile $tmpFile;New-Item -Path $webAppLocation -Type Directory | Out-Null;Write-Host -ForegroundColor Green "`t-Expanding archive $tmpFile to $webAppLocation";[System.Reflection.Assembly]::LoadWithPartialName("System.IO.Compression.FileSystem") | Out-Null;[System.IO.Compression.ZipFile]::ExtractToDirectory($tmpFile, $webAppLocation);Write-Host -ForegroundColor Green "`t-Installing IIS";Install-WindowsFeature Web-Server -IncludeManagementTools | Out-Null;Write-Host -ForegroundColor Green "`t-Downloading Hosting Bundle from MSFT to support ASP.NET Core behind IIS";Invoke-WebRequest https://download.visualstudio.microsoft.com/download/pr/dd119832-dc46-4ccf-bc12-69e7bfa61b18/990843c6e0cbd97f9df68c94f6de6bb6/dotnet-hosting-3.1.2-win.exe -OutFile $tmpHostingEnv;Write-Host -ForegroundColor Green "`t-Installing Hosting Bundle in zero touch mode";Start-Process -FilePath $tmpHostingEnv -Wait -ArgumentList /passive;iisreset | Out-Null;New-WebAppPool -name $AppPoolName -Force | Out-Null;$appPool = Get-Item "IIS:\AppPools\$AppPoolName";$appPool.processModel.identityType = "LocalSystem";$appPool | Set-Item;Write-Host -ForegroundColor Green "`t-Registering Site in IIS";new-WebSite -name $webSiteName -PhysicalPath "$webAppLocation" -ApplicationPool $AppPoolName -force | Out-Null;Write-Host -ForegroundColor Green "`t-Allowing HTTP port passing Windows Firewall";New-NetFirewallRule -DisplayName "HTTP-TCP-80" -Direction Inbound -LocalPort 80 -Protocol TCP -Action Allow | Out-Null;Write-Host -ForegroundColor Green "`t***Execution has been completed***"

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

When you need to add a new script handler you need to modify appsettings.json.
You don't need to change the code or recompile the app.

Config example:
```json
"Scripts": {
    "DefaultKey": "DefaultKey",
    "Handlers": [
      {
        "ProcessName": "NewProcessToRun",
        "ScriptsLocation": "./newHandlerScriptLocation",
        "FileExtension": "nh" // Should be a script extension. For instance a python 'script.py' has a 'py' file extension.
      }
    ]
  }
```
