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
    public class update
    {
        public static string updateid(string args1, string args2) // args1: Extension number - args2: New Outbound Caller Id
        {
            //Test class for update the Outbound Caller Id of an extension
            Console.ForegroundColor = ConsoleColor.Green;
            Logger.WriteLine("Update Extension: " + args1);
            Logger.WriteLine("New Outbound Caller ID: " + args2);
            Console.ResetColor();
            try
            {
                PhoneSystem ps = PhoneSystem.Root;
                var extension = ps.GetDNByNumber(args1) as Extension;
                Logger.WriteLine($"Previous Outbound Caller ID: {extension.OutboundCallerID}");
                extension.OutboundCallerID = args2;
                extension.Save();
                return ($"Saved Caller ID : {extension.OutboundCallerID} for ");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update failed\n{ex}");
                return null;
            }
        }
    }
}