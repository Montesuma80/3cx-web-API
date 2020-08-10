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
    public class transfercall
    {
        public static string cold(string args1, string args2)
        { 
            string connectionAsString(ActiveConnection ac)
                {
                    return $"ID={ac.ID}:CCID={ac.CallConnectionID}:S={ac.Status}:DN={ac.DN.Number}:EP={ac.ExternalParty}:REC={ac.RecordingState}";
                }
            string mod2 = "";
            using (var dn = PhoneSystem.Root.GetDNByNumber(args1))
                {
                    using (var connections = dn.GetActiveConnections().GetDisposer())
                        {
                            var alltakenconnections = connections.ToDictionary(x => x, y => y.OtherCallParties);
                            foreach (var kv in alltakenconnections)
                                {
                                    var owner = kv.Key;
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Logger.WriteLine($"Call {kv.Key.CallID}:");
                                    Console.ResetColor();
                                    string result = connectionAsString(owner);
                                    Logger.WriteLine(result);
                                    if (result.Contains("S=Connected"))
                                        {
                                            int cut = result.IndexOf(':');
                                            string mod = result.Substring(0, cut);
                                            mod2 = mod.Substring(3);
                                            Console.ForegroundColor = ConsoleColor.Red;
                                            Logger.WriteLine("Active Connection Number is:");
                                            Logger.WriteLine(mod2);
                                            Console.ResetColor();
                                        }
                                    else
                                        {
                                            Console.ForegroundColor = ConsoleColor.Red;
                                            Logger.WriteLine("Active Connection Number is unknown");
                                            Console.ResetColor();
                                        }
                                }
                            }
                    }
            int i = 0;
            try
                {
                    i = System.Convert.ToInt32(mod2);
                }
            catch (FormatException)
                {
                    // the FormatException is thrown when the string text does 
                    // not represent a valid integer.
                }
            catch (OverflowException)
                {
                    // the OverflowException is thrown when the string is a valid integer, 
                    // but is too large for a 32 bit integer.  Use Convert.ToInt64 in
                    // this case.
                }
            if (args2.Length != 0 && i != 0)
                {
                Logger.WriteLine("Transfer from:" + i + " to" + args2);
                Logger.WriteLine("Answering on Phone:" + i);
                PhoneSystem.Root.GetByID<ActiveConnection>(i).ReplaceWith(args2);
                return("true");
                }
            else
                {
                    return("false");
                }
        }
    }
}