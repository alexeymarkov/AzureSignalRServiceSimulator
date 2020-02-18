ECHO off
CLS

ECHO CLEANING SOLUTION
dotnet clean AzureSignalRServiceSimulator.sln -v q -c Release /nologo

set buildFolder="..\build\win-64"

ECHO REMOVE OUTPUT FOLDER %buildFolder%
rd /S /Q %buildFolder%

ECHO PUBLISHING SOLUTION
dotnet publish .\AzureSignalRServiceSimulator\AzureSignalRServiceSimulator.csproj -o %buildFolder% -v q -c Release --self-contained -f netcoreapp3.1 -r win-x64 /nologo /p:PublishSingleFile=true

nuget pack "nuget\win-64\AzureSignalRServiceSimulator.nuspec" -OutputDirectory %buildFolder%

ECHO DONE!
pause
