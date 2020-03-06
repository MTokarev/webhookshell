# WebHookShell

### Content
- [WebHookShell](#webhookshell)
    - [Content](#content)
  - [Product Overview](#product-overview)
  - [How it works](#how-it-works)


## Product Overview
In the modern cloud world you have different approaches to run scripts using WebHooks, Azure Functions for instance.
This small project was born to make an Open Source Alternative that you can easily deploy on your Linux or Windows machine.

## How it works
Server uses a key inside appsettings.json to check that all incoming webhooks is authorized (by comparing key from web hook with the one in appsettings). If the key is valid then server calls the script either without or with parameters.

You must specify root folder where all your scripts is located in the appsettings.json file.
Example:

```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "key": "24ffc5be-7dd8-479f-898e-27169bf23e7f",
  "ScriptLocations": {
    "PowerShell": ".\\powershellScripts"
  }
}

```

**Web Query Example:**

`https://localhost:5001/webhook/v1/?key=24ffc5be-7dd8-479f-898e-27169bf23e7f&script=Test-Script.ps1&param=-CreateFileIn%20C:\git\webhookshell\powershellScripts`

This will execute a script 

`.\powershellScripts\Test-Script.ps1 -CreateFileIn C:\git\webhookshell\powershellScripts`

If script will be executed without errors you will get HTTP 200 code and the text:
`Script 'Test-Script.ps1' has been succesfully executed.`

Oterwise HTTP 500 will be returned with exception as a result.