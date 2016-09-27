//using System;
//using System.AddIn;
//using System.Collections.Generic;
//using System.Reflection;
//using System.Text;
//using System.Windows.Forms;
//using RightNow.AddIns.AddInViews;
//using RightNow.AddIns.Common;
//using Oracle.RightNow.Cti.Logger;

//namespace Oracle.RightNow.Cti
//{
//    [AddIn("ChatAutomationClient", Version = "1.0.0.0")]
//    public class ChatAutomationClient : IChatAutomationClient
//    {
//        public bool Initialize(IGlobalContext context)
//        {
//            GlobalContext = context;
//            return true;
//        }

//        public IChatSession ChatSession
//        {
//            set
//            {
//                ChatAutomationClient.chatSession = value;

//                chatSession.SystemErrorEvent += OnSystemErrorEvent;
//                chatSession.SessionStatusEvent += OnSessionStatusEvent;
//                chatSession.EngagementAssignmentEvent += OnEngagementAssignmentEvent;
//                chatSession.EngagementRemovedEvent += OnEngagementRemovedEvent;
//            }
//        }

//        public static void Login(bool logonOverride)
//        {
//            if (chatSession != null && isLoggedIn == false)
//            {
//                agentStatus = chatSession.EnumerateAgentStatuses();
//                foreach (var ast in agentStatus)
//                {
//                    Logger.Logger.Log.Debug(string.Format("AgentStatus : StausID - {0} StausIDLabel - {1} StatusType - {2} StatusTypeLabel - {3}", ast.StatusId, ast.StatusIdLabel.ToString(), ast.StatusType.ToString(), ast.StatusTypeLabel.ToString()));
//                }

//                chatSession.Login(logonOverride);
//                isLoggedIn = true;
//                Logger.Logger.Log.Debug("Chat is not null.");
//            }
//            else
//            {
//                Logger.Logger.Log.Debug("Chat is null.");
//            }
//        }
//        public static void Logoff(bool forceLogoff)
//        {
//            if (chatSession != null && isLoggedIn == true)
//            {
//                Logger.Logger.Log.Debug("Chat is not null.");
//                chatSession.Logoff(forceLogoff);
//                isLoggedIn = false;
//            }
//            else
//            {
//                Logger.Logger.Log.Debug("Chat is null.");
//            }
//        }
//        public static void SetAgentStatus(ChatActivityState statusType, int statusId)
//        {
//            Logger.Logger.Log.Debug(string.Format("{0} - {1}", statusType, statusId));
//            if ((ChatAutomationClient.statusType != statusType || ChatAutomationClient.statusId != statusId) && chatSession != null && isLoggedIn == true)
//            {
//                Logger.Logger.Log.Debug("Calling SetAgent Statue Method.");
//                chatSession.SetAgentStatus(statusType, statusId);
//            }
//        }
//        public static void Conclude(long engagementId)
//        {
//            Logger.Logger.Log.Debug("Chat Conclude");
//            chatSession.Conclude(engagementId);
//        }
//        public static void Release(long engagementId)
//        {
//            Logger.Logger.Log.Debug("Chat Release");
//            chatSession.Release(engagementId);
//        }
//        public static void Close()
//        {
//            Logger.Logger.Log.Debug("Chat Close.");
//            chatSession.Close();
//        }

//        public static void CallSetAgentState(string agentState)
//        {
//            int statusId = GetStatusID(agentState);
//            Logger.Logger.Log.Debug(string.Format("Current Agent State: {0} StatusID: {1} ", agentState, statusId));
//            switch (agentState)
//            {
//                case "Ready":
//                    SetAgentStatus(ChatActivityState.AVAILABLE, statusId);
//                    break;
//                case "NotReady":
//                    if (statusId != 0) SetAgentStatus(ChatActivityState.UNAVAILABLE, statusId);
//                    else SetAgentStatus(ChatActivityState.UNAVAILABLE, 22);
//                    break;
//                case "WrapUp":
//                    if (statusId != 0) SetAgentStatus(ChatActivityState.UNAVAILABLE, statusId);
//                    else SetAgentStatus(ChatActivityState.UNAVAILABLE, 22);
//                    break;
//            }
//        }

//        public static int GetStatusID(string agentState)
//        {
//            int sID = 0;
//            if (agentState == "Ready")
//                sID = 21;
//            else if (agentState == "NotReady")
//            {
//                if (agentStatus.Count != 0)
//                {
//                    foreach (var astatus in agentStatus)
//                    {
//                        if ((astatus.StatusIdLabel.ToUpper() == "AVAYA AUX") && astatus.StatusType == ChatActivityState.UNAVAILABLE)
//                        {
//                            sID = astatus.StatusId;
//                        }
//                    }
//                }
//            }
//            else if (agentState == "WrapUp")
//            {
//                if (agentStatus.Count != 0)
//                {
//                    foreach (var astatus in agentStatus)
//                    {
//                        if ((astatus.StatusIdLabel.ToUpper() == "AVAYA ACW") && astatus.StatusType == ChatActivityState.UNAVAILABLE)
//                        {
//                            sID = astatus.StatusId;
//                        }
//                    }
//                }
//            }
//            return sID;
//        }

//        public static uint MaxEngagements
//        {
//            get
//            {
//                return chatSession.MaximumEngagements;
//            }
//            set
//            {
//                chatSession.MaximumEngagements = value;
//            }
//        }

//        private static void OnSystemErrorEvent(object sender, EventArgs ea)
//        {
//            var methodName = MethodBase.GetCurrentMethod().Name;
//            GlobalContext.LogMessage(string.Format("{0} - {1}", methodName, ObjToString(ea)));
//            Logger.Logger.Log.Debug(string.Format("{0} - {1}", methodName, ObjToString(ea)));
//        }
//        private static void OnSessionStatusEvent(object sender, ChatSessionStatusEventArgs ea)
//        {
//            GlobalContext.LogMessage(string.Format("{0} - {1}", MethodBase.GetCurrentMethod().Name, ObjToString(ea)));
//            Logger.Logger.Log.Debug(string.Format("{0} - {1}", MethodBase.GetCurrentMethod().Name, ObjToString(ea)));
//            if (statusType == ea.StatusType && statusId == ea.StatusId)
//                return;

//            clientId = ea.ClientId;
//            statusType = ea.StatusType;
//            statusId = ea.StatusId;
//        }
//        private static void OnEngagementAssignmentEvent(object sender, ChatEngagementAssignmentEventArgs ea)
//        {
//            engagementCount++;
//            GlobalContext.LogMessage(string.Format("{0} - {1}", MethodBase.GetCurrentMethod().Name, ObjToString(ea)));
//            Logger.Logger.Log.Debug(string.Format("{0} : {1} - {2}", MethodBase.GetCurrentMethod().Name, engagementCount, ObjToString(ea)));
//        }
//        private static void OnEngagementRemovedEvent(object sender, ChatEngagementRemovedEventArgs ea)
//        {
//            engagementCount--;
//            GlobalContext.LogMessage(string.Format("{0} - {1}", MethodBase.GetCurrentMethod().Name, ObjToString(ea)));
//            Logger.Logger.Log.Debug(string.Format("{0} : {1} - {2}", MethodBase.GetCurrentMethod().Name, engagementCount, ObjToString(ea)));
//        }

//        private static string ObjToString(object obj)
//        {
//            var sb = new StringBuilder();
//            sb.AppendFormat("{0}\r\n", obj.GetType().Name.Replace("ContractToViewAdapter", ""));
//            foreach (var prop in obj.GetType().GetProperties())
//            {
//                sb.AppendFormat("   {0}: {1}\r\n", prop.Name, prop.GetValue(obj, null));
//            }
//            return sb.ToString();
//        }

//        public static IGlobalContext GlobalContext;
//        private static IChatSession chatSession;
//        private static long clientId = -1;
//        private static ChatActivityState statusType;
//        public static int statusId = -1;
//        private static int engagementCount;
//        private static bool isLoggedIn = false;
//        private static IList<IChatAgentStatus> agentStatus;
//    }

//}