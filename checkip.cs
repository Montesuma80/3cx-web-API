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
using System.Net.Sockets;

namespace WebAPI
{
    public class getlocalip
    {
    public static string localIPAddress()
        {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in host.AddressList)
            {
                localIP = ip.ToString();
                string[] temp = localIP.Split('.');
                if (ip.AddressFamily == AddressFamily.InterNetwork && temp[0] == "192")
                {
                    break;
                }
                else
                {
                    localIP = null;
                }
            }
            return localIP;
        }
        public static string localAddress()
        {
            IPHostEntry host;
            string localIP = "";
            string localIPRange = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                localIP = ip.ToString();
                string[] temp = localIP.Split('.');
                if (ip.AddressFamily == AddressFamily.InterNetwork && temp[0] == "192")
                {
                    localIPRange = temp[0]+"."+temp[1];
                    break;
                }
                else
                {
                    localIPRange = null;
                }
            }
            return localIPRange;
        }
    }
}