# 3CX We API for 3CX V16


#### Requirements
- Dot Net Core 3.1 (2.1 for V16.0 - 16.4)
- 3CXPhoneSystem.ini
- 3cxpscomcpp2.dll

------------


##### Installation

###### Windows

- Download from Microsoft Dot Net Core v3.1
- Install Dot Net Core
- Copy the 3cxpscomcpp2.dll from your 3CX Install dir to this API Folder
- run in cmd dotnet build WebAPICore.csproj

###### Linux

-  Bash: **apt-get install -y dotnet-runtime-3.1**
- Copy the 3cxpscomcpp2.dll from your 3CX Install dir to this API Folder
- Bash:  **dotnet build WebAPICore.csproj**

###### Start the API
Now you can start the API.
**For Windows User, the API need Admin rights, so start cmd as Administrator.**

dotnet WebAPICore.dll 
or 
dotnet WebAPICore Port

Sample: dotnet WebAPICore.dll 8888

##### Features
