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
    public class makedirectcall
    {

        //Arg1 = Extension
        //Arg2 = Destination
        //Arg3 = Phoneart (Soft /desktop)
        //if one of them is empty, send error Back 
        public static string dial(string args1,string args2,string args3)
        {       
            Console.ForegroundColor = ConsoleColor.Green;
            Logger.WriteLine("From Extension: " + args1);
            Console.ForegroundColor = ConsoleColor.Red;
            Logger.WriteLine("To Destination " + args2);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Logger.WriteLine("Phone " + args3);
            Console.ResetColor();

                                    string mod;
                                        foreach (var dn in PhoneSystem.Root.GetDN().GetDisposer(x => x.Number.StartsWith(args1 == "all" ? "" : args1)))
                                        {
                                            foreach (var r in dn.GetRegistrarContactsEx())
                                            {
                                                string result = $"{r.ID}-{r.UserAgent}";
                                                Logger.WriteLine("DNReg " + result);
                                                if (result.Contains("3CXPhone for Windows") && args3 == "Soft")
                                                {
                                                    int cut = result.IndexOf('-');
                                                    mod = result.Substring(0, cut);
                                                    var registrarRecord = PhoneSystem.Root.GetByID<RegistrarRecord>(int.Parse(mod));
                                                    PhoneSystem.Root.MakeCall(registrarRecord, args2);
                                                    Thread.Sleep(2500);

                                                    // Construct a response.
                                                    //respone the CallID
                                                    string callid =  getcallid.showcallid(args1);
                                                    return (callid);
                                                }
                                                else if (!result.Contains("3CX") && args3 == "Desktop")
                                                {
                                                    Logger.WriteLine(args3);
                                                    int cut = result.IndexOf('-');
                                                    mod = result.Substring(0, cut);
                                                    var registrarRecord = PhoneSystem.Root.GetByID<RegistrarRecord>(int.Parse(mod));

                                                    PhoneSystem.Root.MakeCall(registrarRecord, args2);
                                                   Thread.Sleep(2500);
                                                    string callid =  getcallid.showcallid(args1);
                                                    return (callid);
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