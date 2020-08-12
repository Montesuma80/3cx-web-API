# 3CX Web API for 3CX V16


#### Requirements
- Dot Net Core 3.1 (2.1 for V16.0 - 16.4)
- 3CXPhoneSystem.ini

------------


##### Installation

###### Windows

- Download from Microsoft Dot Net Core v3.1
- Install Dot Net Core
- run in cmd dotnet build WebAPICore.csproj

###### Linux

- Bash: **apt-get install -y dotnet-runtime-3.1**
- Bash:  **dotnet build WebAPICore.csproj**

###### Start the API
Now you can start the API.
it is in this path: bin\Debug\netcoreapp3.1

**For Windows User, the API need Admin rights, so start cmd as Administrator.**

dotnet WebAPICore.dll 
or 
dotnet WebAPICore Port

Sample: dotnet WebAPICore.dll 8888

Sample debug mode: dotnet WebAPICore.dll 8888 debug

##### Features
