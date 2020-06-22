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
    public class park
    {
        public static string call(string args1)
        { 
        int mod2 = 0;
        using (var dn = PhoneSystem.Root.GetDNByNumber(args1))
        {
            using (var connections = dn.GetActiveConnections().GetDisposer())
            {
                var alltakenconnections = connections.ToDictionary(x => x, y => y.OtherCallParties);
                foreach (var kv in alltakenconnections)
                {
                    var owner = kv.Key;
                    string result = owner.Status.ToString();
                    Logger.WriteLine("Owner: " + owner);
                    Logger.WriteLine("Owner Status:" + result);
                    mod2 = owner.CallID;
                    Logger.WriteLine("CallID: " + mod2);
                    string ParkPos = "*00";
                    Logger.WriteLine("Park Position: " + ParkPos);
                    PhoneSystem.Root.TransferCall(mod2, owner, ParkPos);
                    return "true";
                }
            Logger.WriteLine("keine offene Connection gefunden");
            return "false";
            }
        }
        }
    }
}
