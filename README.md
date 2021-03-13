# Azure Functions

Azure functions template, based upon the Clean Architecture

![Clean Architecture](Readme/Images/CleanArchitecture.PNG)

## local.settings.json

```
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "SqlConnectionString": "{DATABASESTRING}"
  }
}
```