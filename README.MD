# WebHookShell

### Content
- [WebHookShell](#webhookshell)
    - [Content](#content)
  - [Product Overview](#product-overview)
  - [How it works](#how-it-works)
  - [Specify root folder for scripts](#specify-root-folder-for-scripts)


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

`https://localhost:5001/webhook/v1/?key=24ffc5be-7dd8-479f-898e-27169bf23e7f&script=Test-Script.ps1`

![img](~/../Docs/Img/scriptexecutedsuccesfully.png)

With parameters:

`https://localhost:5001/webhook/v1/?key=24ffc5be-7dd8-479f-898e-27169bf23e7f&script=Test-Script.ps1&param=-Param1 A -Param2 B`

![img](~/../Docs/Img/scriptexecutedsuccesfully2.png)

## Specify root folder for scripts

In the appsettings file there is a section **ScriptLocations** that contain path to scripts, default value:

```
"ScriptLocations": {
    "PowerShell": ".\\powershellScripts"
  }
```
