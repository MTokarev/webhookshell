# WebHookShell

### Content
- [WebHookShell](#webhookshell)
    - [Content](#content)
  - [Product Overview](#product-overview)
  - [How it works](#how-it-works)
  - [Setting up](#setting-up)
    - [Script locations](#script-locations)
    - [Adding key](#adding-key)
  - [Install on windows](#install-on-windows)
  - [Make sure it works](#make-sure-it-works)
  - [Security Context](#security-context)


## Product Overview
In the modern cloud world you have different approaches to run scripts using WebHooks, for instance Azure Functions, Azure Automation or Amazon Lambda.
In some cases users tend to use some tools like Jenkins just to provide ability to trigger the function execution.
This small lightweight project was born to make an Open Source Alternative that you can easily deploy on your Linux or Windows box.

## How it works
User send the HTTP GET message to the server. The message includes following parameters:

1. Key
2. Script name
3. Param *[Optional]*

In order to protect web hook from accidental execution server will load the **Key** from appsettings.json and compare with the key provided in request. If not match, server stop executing pipeline and return an error:

![img](~/../Docs/Img/keyerror.png)

If any exception arise along the way, exception hander will return an error:

![img](~/../Docs/Img/scriptexecutionfailed.png)

**Example** of succesfful webhook without param:

`https://localhost:5001/webhook/v1?key=24ffc5be-7dd8-479f-898e-27169bf23e7f&script=Test-Script.ps1`

![img](~/../Docs/Img/scriptexecutedsuccesfully.png)

With parameters:

`https://localhost:5001/webhook/v1?key=24ffc5be-7dd8-479f-898e-27169bf23e7f&script=Test-Script.ps1&param=-Param1 A -Param2 B`

![img](~/../Docs/Img/scriptexecutedsuccesfully2.png)

## Setting up

### Script locations

In the appsettings file there is a section **ScriptLocations** that contain path to scripts, default value:

```
"ScriptLocations": {
    "PowerShell": ".\\powershellScripts"
  }
```
### Adding key

Key is nothing, but string. You can whatever you want. Key is stored in appsettings file:

```
"key": "24ffc5be-7dd8-479f-898e-27169bf23e7f"
```

## Install on windows


## Make sure it works

By default I included one ps1 script Test-Script.ps1 that you can run.
Script has 3 option parameters:

```
-Param1 [string]
-Param2 [string]
-CreateFileIn [string]
```
If you provide directory `CreateFileIn` then script will create an empty file `apidemo.tmp`. It will confirm that the app is configured properly. 

**Example**

Open your browser and go to (change localhost:5000 to your hostname)
`https://localhost:5001/webhook/v1/?key=24ffc5be-7dd8-479f-898e-27169bf23e7f&script=Test-Script.ps1&param=-Param1%20A%20-Param2%20B%20-CreateFileIn%20.\powershellScripts`

![img](~/../Docs/Img/scriptexecutedsuccesfully3.png)

If you can see file in the folder you provided with CreateFileIn parameter, then you can sleep in peace.

## Security Context

By default app on windows is using appPool user (LocalSystem) context and has a local admin access. You can provide any credentials inside your ps1 script by using your own logic, or you can change appSetting username in the IIS to change the context.