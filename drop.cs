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
    public class dropcall
    {
        public static string dropcallid(string args1)
        {       
            Console.ForegroundColor = ConsoleColor.Green;
            Logger.WriteLine("ActiveConnection Number for this Extension: " + args1);
            Console.ResetColor();
            string mod2 = "";
            int callidisset = 0;
            using (var dn = PhoneSystem.Root.GetDNByNumber(args1))
                {
                    if (dn.ID != 0)
                    {
                        Logger.WriteLine($"Call {dn.ID}:");
                    }
                    using (var connections = dn.GetActiveConnections().GetDisposer())
                    {
                        if (connections.Count() == 0)
                            {
                             return "false";    
                            }
                    var alltakenconnections = connections.ToDictionary(x => x, y => y.OtherCallParties);
                    foreach (var kv in alltakenconnections)
                        {
                        callidisset = 1;
                        var owner = kv.Key;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Logger.WriteLine($"Call {kv.Key.CallID}:");
                        Logger.WriteLine($"ID {kv.Key.ID}:");
                        Logger.WriteLine("Active Connection Number is:");
                        Logger.WriteLine(mod2);
                        Console.ResetColor();
                        mod2 = "" + kv.Key.ID;
                        }

                    }
                }
                if (callidisset != 0)
                    {
                        PhoneSystem.Root.GetByID<ActiveConnection>(int.Parse(mod2)).Drop();
                        return "true"; 
                    }
                else if (callidisset == 0)
                    {
                        return "false"; 
                    }
                else
                    {
                        return "other error";
                    }
            
        }
    }
}