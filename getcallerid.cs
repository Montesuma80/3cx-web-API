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
    public class getcallqueuenumber
    {
        public static string showid(string args1)
        {       
            string mod2 = "error";
            string Queue_number = "";
            string Queuename = "";
            using (var dn = PhoneSystem.Root.GetDNByNumber(args1))
            {
                using (var connections = dn.GetActiveConnections().GetDisposer())
                {
                    var alltakenconnections = connections.ToDictionary(x => x, y => y.OtherCallParties);
                    foreach (var kv in alltakenconnections)
                    {
                        var owner = kv.Key;
                        
                        if (owner.AttachedData.ContainsKey("requested-target-id"))
                            {
                                Queue_number = owner.AttachedData.GetValueOrDefault("requested-target-id");
                                Queue_number = Queue_number.Substring(0,4);
                                PhoneSystem ps = PhoneSystem.Root;
                                var qa =new Queue[] { ps.GetDNByNumber(Queue_number) as Queue };
                                Logger.WriteLine( qa[0].Name);
                                Queuename = qa[0].Name;
                            }
                        else
                            {
                               Queuename = "Direkter Anruf"; 
                            }
                        mod2 = owner.AttachedData.GetValueOrDefault("extnumber");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Logger.WriteLine("Called Number: " + mod2);
                        Logger.WriteLine("Called Queue: " + Queue_number);
                        Console.ResetColor();
                        mod2= "Queue: " + Queuename + " Anrufer: " + mod2;
                    }
                }

                if (mod2 == "error")
                {
                    return "idle";
                }
                else
                {
                    return mod2;
                }
            }

        }
    }
}