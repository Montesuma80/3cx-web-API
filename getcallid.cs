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
    public class getcallid
    {
        public static string showcallid(string args1)
        {       
            string mod2 = "0";
            string CallID = "0";
            int CallIDv16 = 0;
            using (var dn = PhoneSystem.Root.GetDNByNumber(args1))
            {
                using (var connections = dn.GetActiveConnections().GetDisposer())
                {
                    var alltakenconnections = connections.ToDictionary(x => x, y => y.OtherCallParties);
                    foreach (var kv in alltakenconnections)
                    {
                        var owner = kv.Key;
                        mod2 = "" +  kv.Key.HistoryIDOfTheCall;
                        /*int position = mod2.IndexOf("_");
                        if (position < 0)
                            continue;
                            Logger.WriteLine("Key: {0}, Value: '{1}'", 
                            mod2.Substring(0, position),
                            CallID = mod2.Substring(position + 1));
                            */
                        CallIDv16 = kv.Key.CallID;
                        CallID = mod2;
                        Logger.WriteLine("CallID: " + CallID);
                        Logger.WriteLine("CallID_v16: " + CallIDv16);
                    }
                }
                return CallID;
            }

        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}