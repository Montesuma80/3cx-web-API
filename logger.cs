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
public static class Logger
{
    public static StringBuilder LogString = new StringBuilder();
    public static void WriteLine(String str)
    {
        Console.WriteLine(str);
        LogString.Append(str).Append(Environment.NewLine);
        if (Program.Debugger == "debug")
            {
                addtext(str);
            }
        
    }
    public static void Write(string str)
    {
        Console.Write(str);
        LogString.Append(str);
        if (Program.Debugger == "debug")
            {
                addtext(str);
            }

    }

public static void addtext(string str)
    {
                String filedate = DateTime.Now.ToString("yyyyMMdd");;
                string Path = "./" + filedate + "_log.txt";
                using (StreamWriter file = System.IO.File.AppendText(Path))
                {
                    file.WriteLine(str);
                    file.Close();
                    file.Dispose();
                }

    }

    public static void SaveLog(bool Append = false, string Path = "./Log.txt")
    {
        if (LogString != null && LogString.Length > 0)
        {
            if (Append)
            {
                String filedate = DateTime.Now.ToString("yyyyMMdd");;
                Path = "./" + filedate + "_log.txt";
                using (StreamWriter file = System.IO.File.AppendText(Path))
                {
                    file.Write(LogString.ToString());
                    file.Close();
                    file.Dispose();
                }
            }
            else
            {
                String filedate = DateTime.Now.ToString("yyyyMMddhh");;
                Path = "./" + filedate + "_log.txt";
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Path))
                {
                    file.Write(LogString.ToString());
                    file.Close();
                    file.Dispose();
                }
            }  
           
        }
    }
}
}