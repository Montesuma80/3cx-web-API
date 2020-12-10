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
    public class getdnregs
    {
        public static string status(string args1)
        {       
            var dn = PhoneSystem.Root.GetDNByNumber(args1);
            RegistrarRecord[] rr = dn.GetRegistrarContactsEx();
            
                foreach (var r in rr)
                {
                    string result = $"{r.ID}-{r.Contact}-{r.UserAgent}";
                    Logger.WriteLine(result);
                    return result;
                }
                return "error";
        }
    }
}