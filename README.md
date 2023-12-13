# 3CX Web API / 3CX Phone System Web API 

------------

I´m intrested to know who is using my API.
Sometimes i get a bug Mail, but no respone who is using it.

@All please send me an mail to **hoening`at`googlemail.com

------------
#### Supportet by v20 Beta2
#### Supportet by v18 Alpha4

------------


#### Requirements
- Dot Net 5.0
- 3CXPhoneSystem.ini (Debian User must fix the Path in the ini for the 3cxpscomcpp2.dll)

------------


#### Installation

##### Windows

- Download from Microsoft Dot Net 5.0
- Install Dot Net Core
- run in cmd dotnet build WebAPICore.csproj

##### Linux (Debian 10)

```bash
wget https://packages.microsoft.com/config/debian/10/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt-get update; \ sudo apt-get install -y apt-transport-https && \ sudo apt-get update && \ sudo apt-get install -y dotnet-sdk-5.0
```

**Bevore compile, you need to edit the **  *WebAPICore.csproj*
- remove *`<Private>false</Private>`* in <ItemGroup> for 3cxpscomcpp2
- edit path: `<HintPath>..\..\..\Program Files\3CX Phone System\Bin\3cxpscomcpp2.dll</HintPath>` to `<HintPath>/usr/lib/3cxpbx/3cxpscomcpp2.dll</HintPath>` 


```bash
dotnet build WebAPICore.csproj
```


#### Start the API
Now you can start the API.
it is in this path: bin\Debug\net5.0
You need the 3CXPhoneSystem.ini in your API folder

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

