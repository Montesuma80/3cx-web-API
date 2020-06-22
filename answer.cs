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
    public class answer
    {
        public static string call(string args1)
        {       
            string connectionAsString(ActiveConnection ac)
                {
                    return $"ID={ac.ID}:CCID={ac.CallConnectionID}:S={ac.Status}:DN={ac.DN.Number}:EP={ac.DN.GetRegistrarContactsEx()}";
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
                        string agent = "";
                        foreach (var r in owner.DN.GetRegistrarContactsEx())
                            {
                                if (r.UserAgent.Contains("3CXPhone for Windows"))
                                    {
                                        agent = r.Contact;
                                    }
                            }
                        string result = connectionAsString(owner);
                        string CCIDdata = kv.Key.AttachedData.GetValueOrDefault("devcontact");
                        if (result.Contains("S=Ringing") && CCIDdata == agent)
                        {
                            int cut = result.IndexOf(':');
                            string mod = result.Substring(0, cut);
                            mod2 = mod.Substring(3);
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
                                PhoneSystem.Root.GetByID<ActiveConnection>(i).Answer();
                                return ("true");
                        }
                        else if (result.Contains("S=Ringing") && CCIDdata.Contains("127.0.0.1"))
                        {
                            Logger.WriteLine("Active Connection Type is Mobilephone");

                        }
                        else
                        {
                            Logger.WriteLine("next");
                        }
                    }
                }
            return ("null");
            }


        }
    }
}