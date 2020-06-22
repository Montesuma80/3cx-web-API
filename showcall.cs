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
    public class getcall
    {
        public static string showallcall()
        { 
           string connectionAsString(ActiveConnection ac)
            {
                string Queue_number = null;
                string Queuename = "";
                
                if (ac.AttachedData.ContainsKey("requested-target-id"))
                {
                    Queue_number = ac.AttachedData.GetValueOrDefault("requested-target-id");
                    Queue_number = Queue_number.Substring(0,4);
                    PhoneSystem ps = PhoneSystem.Root;
                    var qa =new Queue[] { ps.GetDNByNumber(Queue_number) as Queue };
                    Logger.WriteLine( qa[0].Name);
                    Queuename = qa[0].Name;
                }
                else
                    {
                    Queue_number = "null";
                    Queuename = "Direkter Anruf"; 
                    }
                return $"ID={ac.ID};S={ac.Status};DN={ac.DN.Number};Queue_Name={Queuename};Queue_Nummer={Queue_number};EP={ac.ExternalParty};REC={ac.RecordingState}";
            }
        
        string callerid = "<html><head></head><body>";
        foreach (var c in PhoneSystem.Root.GetActiveConnectionsByCallID())
            {
            Console.ResetColor();
            Logger.WriteLine($"Call {c.Key}:");
            foreach (var ac in c.Value.OrderBy(x => x.CallConnectionID))
                {
                callerid = callerid + connectionAsString(ac) + "<br>";
                Logger.WriteLine($"    {connectionAsString(ac)}");
                }
            }
        callerid = callerid + "</body></html>";      
        return (callerid);
        }
    }
}