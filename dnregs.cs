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
            foreach (var dn in PhoneSystem.Root.GetDN().GetDisposer(x => x.Number.StartsWith(args1 == "all" ? "" : args1)))
            {
                foreach (var r in dn.GetRegistrarContactsEx())
                {
                    string result = $"{r.ID}-{r.Contact}-{r.UserAgent}";
                    Logger.WriteLine(result);
                    return result;
                }
                return "error";
            }
            return "error";
        }
    }
}