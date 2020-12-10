using System;
using System.Collections.Generic;
using System.Text;
using TCX.Configuration;
using TCX.PBXAPI;
using System.Threading;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Net;

namespace WebAPI
{
    public class atttrans
    {
        public static string dotrans(string args1)
        { 
            var dn = PhoneSystem.Root.GetDNByNumber(args1);
            
            ActiveConnection[] activeConnectionArray = dn.GetActiveConnections();
            Logger.WriteLine(activeConnectionArray.Length.ToString());
            if (activeConnectionArray.Length == 2)
                {
                var activeConnection = activeConnectionArray[0];
                var activeConnection2 = activeConnectionArray[1];
                foreach (var r in dn.GetRegistrarContactsEx())
                    {
                    string result = $"{r.UserAgent}-{r.Contact}-{r.ID}";
                    Logger.WriteLine(result); 
                    if (result.Contains("3CXPhone for Windows"))
                        {
                        PhoneSystem.Root.JoinCalls(args1, r.Contact , activeConnection, activeConnection2);
                        }
                    }
                }
            else
                {
                    return("false");
                }
            return ("true");   
        }
    }
}