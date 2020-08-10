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
    public class queuecontroll
    {
        public static string status(string args1, string args2)
        {   

        IEnumerable<string> AllAgentQueues(Extension agentdn)
        {
            return agentdn.GetQueues().Select(x => x.Number);
        }

        IEnumerable<string> GetWorkingSet(Extension agentdn)
        {
            return agentdn.GetPropertyValue("LOGGED_IN_QUEUES")?.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries) ?? AllAgentQueues(agentdn);
        }

        void ChangeWorkingSet(Extension agentdn, IEnumerable<string> qadd, IEnumerable<string> qremove)
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
        void SetWorkingQueues(Extension agentdn, IEnumerable<string> qadd, IEnumerable<string> qremove, QueueStatusType? force_login_status)
        {
        ChangeWorkingSet(agentdn, qadd, qremove);//change current set of queues. If it will become empty - status of extension will reflect status in all queues.
        if (force_login_status.HasValue)
        {
        agentdn.QueueStatus = force_login_status.Value;
        }
        agentdn.Save();
        }
                   
        Logger.WriteLine($"Login at Station {args2}");
        var action = args1;
        var agentDN = args2;
        
        using (var agent = PhoneSystem.Root.GetDNByNumber(agentDN) as Extension)
        {
            if (agent == null)
            {
                Logger.WriteLine($"{agentDN} is not a valid extension");
                return "is not a valid extension";
            }

            if (!AllAgentQueues(agent).Any())
            {
                Logger.WriteLine($"Extension {agent.Number} is not an agent of the queues");
                return "is not an agent of the queues";
            }

            switch (action)
            {
                case "login_all":
                    //login to all queues (reset current set of the queue and set extension status to LoggedIn
                    SetWorkingQueues(agent, AllAgentQueues(agent), new string[0], QueueStatusType.LoggedIn);
                    Logger.WriteLine($"Agent {agentDN} {agent.QueueStatus}:\nWorking set:{string.Join(",", GetWorkingSet(agent))}[forced set {"'" + (agent.GetPropertyValue("LOGGED_IN_QUEUES") ?? "None") + "'"}]\nInactive Queues:{string.Join(",", AllAgentQueues(agent).Except(GetWorkingSet(agent)))}");
                    return "true";

                case "logout_all":
                    //reset working set to default (all queues) and set status of the extension to LoggedOut
                    SetWorkingQueues(agent, new string[0], AllAgentQueues(agent), QueueStatusType.LoggedOut);
                    Logger.WriteLine($"Agent {agentDN} {agent.QueueStatus}:\nWorking set:{string.Join(",", GetWorkingSet(agent))}[forced set {"'" + (agent.GetPropertyValue("LOGGED_IN_QUEUES") ?? "None") + "'"}]\nInactive Queues:{string.Join(",", AllAgentQueues(agent).Except(GetWorkingSet(agent)))}");
                    return "true";
                default:
                    Logger.WriteLine($"Unknown action '{action}'");
                    return "false";
            }

        }
        }
    }
}

