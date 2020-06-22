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
    public class profiles
    {
        public static string show(string args1)
        { 
            PhoneSystem ps = PhoneSystem.Root;
            var extension = ps.GetDNByNumber(args1) as Extension;  
            Logger.WriteLine($"    CURRENT_STATUS={extension.CurrentProfile?.Name}");
            return($"    CURRENT_STATUS={extension.CurrentProfile?.Name}");
        }

        public static string setstatus(string args1, string args2)
        { 
        string newprofile = "";
        switch (args2)
		{
			case "avail": newprofile = "Available"; break;
			case "away": newprofile = "Away"; break;
			case "oof": newprofile = "Out of office"; break;
			case "custom1": newprofile = "Custom 1"; break;
			case "custom2": newprofile = "Custom 2"; break;
		}
            PhoneSystem ps = PhoneSystem.Root;
            var extension = ps.GetDNByNumber(args1) as Extension;  
            //int i = 1;
            //i = System.Convert.ToInt32(args2);
            var profile = extension.FwdProfiles.Where(x => x.Name == newprofile).First();
            //var profile = extension.FwdProfiles.ElementAt(i);
            extension.CurrentProfile = profile;
            extension.Save();
            Logger.WriteLine($"CURRENT_STATUS={extension.CurrentProfile?.Name}");
            return($"{extension.CurrentProfile?.Name}");
        }
    }
}