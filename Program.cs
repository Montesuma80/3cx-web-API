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
    class Program
    {

        static Dictionary<string, Dictionary<string, string>> iniContent =
            new Dictionary<string, Dictionary<string, string>>(
                StringComparer.InvariantCultureIgnoreCase);

        public static bool Stop { get; private set; }
        
        static void ReadConfiguration(string filePath)
        {
            var content = File.ReadAllLines(filePath);
            Dictionary<string, string> CurrentSection = null;
            string CurrentSectionName = null;
            for (int i = 1; i < content.Length + 1; i++)
            {
                var s = content[i - 1].Trim();
                if (s.StartsWith("["))
                {
                    CurrentSectionName = s.Split(new[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries)[0];
                    CurrentSection = iniContent[CurrentSectionName] = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
                }
                else if (CurrentSection != null && !string.IsNullOrWhiteSpace(s) && !s.StartsWith("#") && !s.StartsWith(";"))
                {
                    var res = s.Split("=").Select(x => x.Trim()).ToArray();
                    CurrentSection[res[0]] = res[1];
                }
                else
                {
                    //Logger.WriteLine($"Ignore Line {i} in section '{CurrentSectionName}': '{s}' ");
                }
            }
            instanceBinPath = Path.Combine(iniContent["General"]["AppPath"], "Bin");
        }
        
        static void Bootstrap(string[] args)
        {
            String Port = "0";
            PhoneSystem.CfgServerHost = "127.0.0.1";
            PhoneSystem.CfgServerPort = int.Parse(iniContent["ConfService"]["ConfPort"]);
            PhoneSystem.CfgServerUser = iniContent["ConfService"]["confUser"];
            PhoneSystem.CfgServerPassword = iniContent["ConfService"]["confPass"];
            var ps = PhoneSystem.Reset(
                PhoneSystem.ApplicationName + new Random(Environment.TickCount).Next().ToString(),
                "127.0.0.1",
                int.Parse(iniContent["ConfService"]["ConfPort"]),
                iniContent["ConfService"]["confUser"],
                iniContent["ConfService"]["confPass"]);
            ps.WaitForConnect(TimeSpan.FromSeconds(30));
            try
            {
                //SampleStarter.StartSample(args);
			if (!HttpListener.IsSupported)
            {
                Logger.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }
            
            // URI prefixes are required,
            if (args.Length ==  0)
                {
                    Logger.WriteLine("No Port Submitted, use Generic Port: 8889");
                    Port = "8889";

                }
            else   
                {
                    Logger.WriteLine($"Port Submitted, use Generic Port: {args[0]}");
                    Port = args[0];
                }
            var prefixes = new List<string>() { $"http://*:{Port}/" };
            
            

            // Create a listener.
            HttpListener listener = new HttpListener();
            // Add the prefixes.
            foreach (string s in prefixes)
            {
                Console.WriteLine(s);
                listener.Prefixes.Add(s);
            }
                listener.Start();
                
                Logger.WriteLine("Listening... on Port " + Port + " for Development");
                Logger.WriteLine("To Stop open URL: http://127.0.0.1:" + Port + "/stop");
                while (true)
                {
                    HttpListenerContext context = listener.GetContext();
                    HttpListenerRequest request = context.Request;
                    Logger.WriteLine(request.RawUrl);

                    string documentContents;
                    using (Stream receiveStream = request.InputStream)
                    {
                        using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                        {
                            documentContents = readStream.ReadToEnd();
                        }
                    }
                    String url = request.RawUrl;
                    String[] queryStringArray = url.Split('/');
                    try
                    {

                        string connectionAsString(ActiveConnection ac)
                        {
                            return $"ID={ac.ID}:CCID={ac.CallConnectionID}:S={ac.Status}:DN={ac.DN.Number}:EP={ac.ExternalParty}:REC={ac.RecordingState}";
                        }
                    string respval = "idle";
                    string text;
                    using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        text = reader.ReadToEnd();
                    }           

                        switch (queryStringArray[1])
                        {
                            case "showallcalls":
                            {
                                respval = getcall.showallcall();
                            }
                            break;
                            case "makecall":
                                {
                                    respval = makedirectcall.dial(queryStringArray[2],queryStringArray[3],queryStringArray[4]);
                                }
                                break;
                            case "ready":
                                {
                                    string callerid = profiles.setstatus(queryStringArray[2], "avail");
                                    string status = "";
                                    if (callerid == "Available")
                                        {
                                            status = queuecontroll.status("login_all",queryStringArray[2]);
                                        }
                                    else
                                        {
                                            status = "false";
                                        }
                                    respval = status;
                                }
                                break;
                            case "notready":
                                {
                                    string callerid = profiles.setstatus(queryStringArray[2], "custom1");
                                    string status = "";
                                    if (callerid == "Custom 1")
                                        {
                                            status = queuecontroll.status("logout_all",queryStringArray[2]);
                                        }
                                    else
                                        {
                                            status = "false";
                                        }
                                respval = status;
                                }
                                break;
                            case "logout":
                                {
                                    string callerid = profiles.setstatus(queryStringArray[2], "away");
                                    string status = "";
                                    if (callerid == "Away")
                                        {
                                            status = queuecontroll.status("logout_all",queryStringArray[2]);
                                        }
                                    else
                                        {
                                            status = "false";
                                        }
                                    respval = status;
                                }
                                break;
                            case "login":
                                {
                                    respval = queuecontroll.status(queryStringArray[2],queryStringArray[3]);
                                }
                                break;
                            case "dnregs":
                                {
                                    respval =  getdnregs.status(queryStringArray[2]);
                                }
                                break;
                            case "ondn":
                                {
                                    respval = getcallid.showcallid(queryStringArray[2]);
                                }
                                break;
                            case "getcallerid":
                                {
                                    respval = getcallqueuenumber.showid(queryStringArray[2]);
                                }
                                break;
                            case "drop":
                                {
                                    respval = dropcall.dropcallid(queryStringArray[2]);
                                }
                                break;
                            case "answer":
                                {
                                    string wert = answer.call(queryStringArray[2]);
                                    string mod2 = "";
                                    Logger.WriteLine("Wert:"+ wert);
                                    if (wert == "true")
                                        {
                                            mod2 = getcallid.showcallid(queryStringArray[2]);
                                        }
                                    else   
                                        {
                                            mod2 = "null";
                                        }
                                    respval = mod2;
                                }
                                break;
                            case "record":
                                {
                                    string mod2 = "";
                                    string mod3 = "";
                                    using (var dn = PhoneSystem.Root.GetDNByNumber(queryStringArray[2]))
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
                                    Logger.WriteLine("Record on Extension:" + i);
                                    if (i > 0)
                                        {
                                        mod3 = "true";
                                    if (Enum.TryParse(queryStringArray[3], out RecordingAction ra))
                                        PhoneSystem.Root.GetByID<ActiveConnection>(i).ChangeRecordingState(ra);
                                    else
                                        throw new ArgumentOutOfRangeException("Invalid record action");
                                        }
                                    else
                                        {
                                            mod3 = "false";
                                        }
                                    respval = mod3;
                                }
                                break;
                            case "transfer":
                                {
                                     respval = transfercall.cold(queryStringArray[2], queryStringArray[3]);
                                }
                                break;
                            case "park":
                                {
                                    respval = park.call(queryStringArray[2]);
                                }
                                break;
                            case "unpark":
                                {
                                    string arg2 = "*10";
                                    string arg3 = "Soft";
                                    respval = makedirectcall.dial(queryStringArray[2],arg2 ,arg3);
                                    
                                }
                                break;
                           case "atttrans":
                                {
                                    respval = atttrans.dotrans(queryStringArray[2]);
                                }
                                break;
                            case "setstatus":
                                {
                                    respval = profiles.setstatus(queryStringArray[2], queryStringArray[3]);
                                }
                                break;
                            case "save":
                                {
                                    respval = savechanges.Run(queryStringArray[2], queryStringArray[3],queryStringArray[4]);
                                }
                                break;
                            case "showstatus":
                                {
                                    respval = profiles.show(queryStringArray[2]);
                                }
                                break;
                            case "divert":
                                {


                                }
                                break;
                            case "stop":
                                {
                                    respval = "<HTML><BODY> Server Stopped</BODY></HTML>";
                                    listener.Stop();
                                    break;
                                    throw new Exception("System Stopped");
                                }
                            default:
                                break;
                        }
                                // Obtain a response object.
                                Console.WriteLine(respval);
                                HttpListenerResponse response = context.Response;
                                // Construct a response.
                                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(respval);
                                // Get a response stream and write the response to it.
                                response.ContentLength64 = buffer.Length;
                                System.IO.Stream output = response.OutputStream;
                                output.Write(buffer, 0, buffer.Length);
                                // You must close the output stream.
                                output.Close();

                    }
                    catch (Exception ex)
                    {
                        if (queryStringArray[1] == "Stop")
                        {
                            Logger.WriteLine("system Stopped");
                            throw new Exception("System Stopped");
                        }
                        else
                        {
                            Logger.WriteLine(ex.Message);
                            continue;
                        }
                    }
                }
  
        }

            finally
            {
                ps.Disconnect();
            }
        }

        static string instanceBinPath;

        static void Main(string[] args)
        {
            try
            {
                var filePath = @"3CXPhoneSystem.ini";
                if (!File.Exists(filePath))
                {
                    Logger.WriteLine(filePath);
					throw new Exception("Cannot find 3CXPhoneSystem.ini");
                }
                ReadConfiguration(filePath);
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
                Bootstrap(args);
                Logger.WriteLine("exited gracefully");
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.ToString());
            }
        }
        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var name = new AssemblyName(args.Name).Name;
            try
            {
                return Assembly.LoadFrom(Path.Combine(instanceBinPath, name + ".dll"));
            }
            catch
            {
                return null;
            }
        }
    }

}
