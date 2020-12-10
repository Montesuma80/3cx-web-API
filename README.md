# 3CX Web API / 3CX Phone System Web API 

>  **Attended Transfer bug is fixed with 3CX V16.0.1078**

#### Requirements
- Dot Net Core 3.1 (2.1 for V16.0 - 16.4 use old release of this API)
- 3CXPhoneSystem.ini (Debian User must fix the Path in the ini for the 3cxpscomcpp2.dll)

------------


#### Installation

##### Windows

- Download from Microsoft Dot Net Core v3.1
- Install Dot Net Core
- run in cmd dotnet build WebAPICore.csproj

##### Linux

- Bash: **apt-get install -y dotnet-runtime-3.1**
- Bash:  **dotnet build WebAPICore.csproj**

#### Start the API
Now you can start the API.
it is in this path: bin\Debug\netcoreapp3.1

**For Windows User, the API need Admin rights, so start cmd as Administrator.**

dotnet WebAPICore.dll 
or 
dotnet WebAPICore.dll Port

Sample: dotnet WebAPICore.dll 8888

Sample debug mode: dotnet WebAPICore.dll 8888 debug

##### Features
URL: http://ip:port/action/arg1/arg2/.....

**Action:**
- makecall
- ready
- notready
- logout
- login
- dnregs
- ondn
- getcallerid
- drop
- answer
- record
- transfer
- park
- unpark
- atttrans
- setstatus
- showstatus
- stop

