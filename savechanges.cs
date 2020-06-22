using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TCX.Configuration;
using System.IO;
namespace WebAPI
{
    class savechanges
    {
        public static IEnumerable<string> AllAgentQueues(Extension agentdn)
        {
            return agentdn.GetQueues().Select(x => x.Number);
        }

        public static IEnumerable<string> GetWorkingSet(Extension agentdn)
        {
            return agentdn.GetPropertyValue("LOGGED_IN_QUEUES")?.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries) ?? AllAgentQueues(agentdn);
        }
        
        public static void ChangeWorkingSet(Extension agentdn, IEnumerable<string> qadd, IEnumerable<string> qremove)
        {
            var res = GetWorkingSet(agentdn).Except(qremove).Union(qadd).Intersect(AllAgentQueues(agentdn));
            if (!res.Any() || res.Count() == agentdn.GetQueues().Length) //if no queues are left or all are specified - remove property which selects current queues
                agentdn.DeleteProperty("LOGGED_IN_QUEUES");
            else
                agentdn.SetProperty("LOGGED_IN_QUEUES", string.Join(",", res));
            if (!res.Any()) //no any queues, set loging status
            {
                agentdn.QueueStatus = QueueStatusType.LoggedOut; //set logout if current working set is empty - set logout status (list of current queus was reset above)
            }
        }

        public static void SetWorkingQueues(Extension agentdn, IEnumerable<string> qadd, IEnumerable<string> qremove, QueueStatusType? force_login_status)
        {
            ChangeWorkingSet(agentdn, qadd, qremove);//change current set of queues. If it will become empty - status of extension will reflect status in all queues.
            if (force_login_status.HasValue)
            {
                agentdn.QueueStatus = force_login_status.Value;
            }
            agentdn.Save();
        }

        public static string Run(string args1,string args2,string args3)
        {
            string[] arraylist = new string[] { args1, args2, args3 };

            var action = args1;
            var agentDN =  args2;
            var queues =  arraylist.Skip(2);
            using (var agent = PhoneSystem.Root.GetDNByNumber(agentDN) as Extension)
            {
                if (agent == null)
                {
                    Console.WriteLine($"{agentDN} is not a valid extension");
                    return "false";
                }

                if (!AllAgentQueues(agent).Any())
                {
                    Console.WriteLine($"Extension {agent.Number} is not an agent of the queues");
                    return "false";
                }

                switch (action)
                {
                    case "login_all":
                        //login to all queues (reset current set of the queue and set extension status to LoggedIn
                        SetWorkingQueues(agent, AllAgentQueues(agent), new string[0], QueueStatusType.LoggedIn);
                        break;
                    case "logout_all":
                        //reset working set to default (all queues) and set status of the extension to LoggedOut
                        SetWorkingQueues(agent, new string[0], AllAgentQueues(agent), QueueStatusType.LoggedOut);
                        break;
                    case "login_only_to":
                        //Set status login and specify requested set of the queues to be logged in
                        SetWorkingQueues(agent, queues, AllAgentQueues(agent), QueueStatusType.LoggedIn);
                        break;
                    case "logout_only_from":
                        //remove list of specified queues form current working set.
                        //if set will become empty - state of the extension should be set to LoggedOut
                        SetWorkingQueues(agent, new string[0], queues, null);
                        break;
                    case "login_current":
                        //set status of the extension as logged in. Extension will become logged is to current set of the queues.
                        SetWorkingQueues(agent, new string[0], new string[0], QueueStatusType.LoggedIn);
                        break;
                    case "logout_current":
                        //simply change status to logged out. Working set of the queues will be left the same.
                        SetWorkingQueues(agent, new string[0], new string[0], QueueStatusType.LoggedOut);
                        break;
                    default:
                        Console.WriteLine($"Unknown action '{action}'");
                        return "false";
                }
                Console.WriteLine("Agent {0} {1}:\nWorking set:{2}[forced set {3}]\nInactive Queues:{4}", agentDN, agent.QueueStatus, string.Join(",", GetWorkingSet(agent)), "'" + (agent.GetPropertyValue("LOGGED_IN_QUEUES") ?? "None") + "'", string.Join(",", AllAgentQueues(agent).Except(GetWorkingSet(agent))));
                return "true";
            }
        }
    }
}