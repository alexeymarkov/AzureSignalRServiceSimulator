# Introduction 
Azure SignalR Service Simulator.

An implementation of Azure SignalR Service to run locally.
It includes:
 * Negotiate endpoint
 * Implementation of https://github.com/Azure/azure-signalr/blob/dev/docs/rest-api.md#api

# Configuration

The configuration in appsettings.json is self explaining.

### Endpoint configucation

```json
"Kestrel": {
    "EndpointDefaults": {
        "Protocols": "Http1"
    },
    "EndPoints": {
        "Https": {
            "Url": "https://localhost:5301"
        }
    }
}
```

* Kestrel
 * EndPoints
   * Https
     * Url		: the address of Azure SignalR Service Simulator

### SignalR configucation

```json
"SignalR": {
    "AccessKey": "NPhD3PeL/jY5GHyTQ1PTJfC1vVyDmxUnJamPHwywhC0=",
    "CORS": [
        "https://localhost:8443"
    ],
    "Hubs": [
        "hub1",
        "hub2"
    ]
}
```

* SignalR
  * Hubs		: add hub names you would like to use (Azure SignalR Service manages hub names dynamically for you but Simulator cannot do this)
  * CORS		: add your web apps if necessary.
  * AccessKey	: you can put any value here if you want
}

### Connection string

The connection string (with the default configuration) to use in your Azure Apps is:
Endpoint=https://localhost:5301;AccessKey=NPhD3PeL/jY5GHyTQ1PTJfC1vVyDmxUnJamPHwywhC0=;Version=1.0;

Format is:
Endpoint={Kestrel:EndPoints:Https:Url};AccessKey={SignalR:AccessKey};Version=1.0;

# Usage

You can

1) Build the project yourself
2) Use the NuGet package: https://www.nuget.org/packages/AzureSignalRServiceSimulator
  Example PowerShell script (adjust a path to appsettings.json):
  $AzureSignalRServiceSimulatorPackage = (Install-Package AzureSignalRServiceSimulator -Scope CurrentUser -Force)
  $NuGetPackagesPath = [System.Environment]::ExpandEnvironmentVariables("%LOCALAPPDATA%\PackageManagement\NuGet\Packages")
  start-process "$($NuGetPackagesPath)\AzureSignalRServiceSimulator.$($AzureSignalRServiceSimulatorPackage.Version)\executable\AzureSignalRServiceSimulator.exe" -Args "--ConfigFile=$($PSScriptRoot)\configs\AzureSignalRServiceSimulator\appsettings.json"

3) Download binaries from Releases
