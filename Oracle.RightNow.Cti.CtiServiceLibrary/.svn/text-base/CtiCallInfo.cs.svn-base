using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocket4Net;
using System.Xml;
using System.Timers;

namespace Oracle.RightNow.Cti.CtiServiceLibrary
{
    public class CtiCallInfo
    {
        public delegate void OnMessageReceivedEventHandler(string message);
        public delegate void OnErrorReceivedEventHandler(string message);
        public delegate void OnSocketOpendedEventHandler();
        public event OnMessageReceivedEventHandler OnMessageReceived;
        public event OnErrorReceivedEventHandler OnErrorReceived;
        public event OnSocketOpendedEventHandler OnSocketOpended;

        private static CtiCallInfo ctiCallInfoObject;
        private static int invokeId=0;
        private static object _syncObj = new object();
        private int activeCall = 0;
        private int heldCall = 0;
        public bool blindTransfer = false;
        public bool blindConference = false;
        private string currentMonitoringStation = string.Empty;
        private System.Timers.Timer timer;

        private string currentAgentId = string.Empty;
        private string currentExtension = string.Empty;
        private string currentAgentsQueue = string.Empty;
        private string currentAgentsPassword = string.Empty;

        private int _monitorID { get;   set; }

        private string priUri = "";
        private string secUri = "";
       // private List<int> callIds;

        private static WebSocket websocket = null;
        System.Threading.ManualResetEvent m_Event;       
        private Dictionary<int, string> callData;

        private Dictionary<string, string> ExtensionAniData;
                
        private string destAddrConsult = "";
        private int transferedCall = 0;

        public bool IsACD = false;

        private CtiCallInfo()
        {
           // callIds = new List<int>();          
            callData = new Dictionary<int,string>();
            ExtensionAniData= new Dictionary<string,string>();
        }

        public string CurrentStation
        {
            get
            {
                return currentMonitoringStation;
            }
        }

        public static CtiCallInfo GetCtiCallInfoObject()
        {

            //double-checked locking, to avoid the expensive lock operation
            if (ctiCallInfoObject == null)
            {
                //Thread safe singleton implementation
                lock (_syncObj)
                {
                    if (ctiCallInfoObject == null)
                    {
                        ctiCallInfoObject = new CtiCallInfo();
                    }
                }
            }
            return ctiCallInfoObject;
        }

        private void websocket_Opened(object sender, EventArgs e)
        {
            Logger.Logger.Log.Debug("WebSocket is Opened");

            timer = new Timer(10000);
            timer.Elapsed += timer_Elapsed;
            timer.Start();
            if (OnSocketOpended != null)
            {
                OnSocketOpended();
            }
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                SendHeartBeat();
            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Error("Cti CallInfo:", ex);
            }
        }

        void SendHeartBeat()
        {
            SendMsg("0" + currentExtension + "-" + DateTime.Now.Millisecond);
            SendMsg("<echo>" + currentExtension + "</echo>");
            
        }

        void websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (OnMessageReceived != null)
            {
                OnMessageReceived(e.Message);
            }
        }

        void websocket_DataReceived(object sender, DataReceivedEventArgs e)
        {
            if (OnMessageReceived != null)
            {
                OnMessageReceived(e.Data.ToString());
            }
        }

        public void ConnectToServer(string primaryserveruri, string secondaryServerUri)
        {
            try
            {
                Logger.Logger.Log.Debug(string.Format("Primary server {0}, Secondary server {1}",primaryserveruri,secondaryServerUri) );
            priUri = primaryserveruri;
            secUri = secondaryServerUri;
            if(string.IsNullOrEmpty(priUri))
                return;
            m_Event = new System.Threading.ManualResetEvent(false);

            DestoryEvent(websocket);
            websocket = new WebSocket(priUri);            
            CreateEvent(websocket);
            websocket.Open();          
            Logger.Logger.Log.Info(string.Format("Connected Primary server {0}", primaryserveruri));
            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Error("CtiCallInfo: ", ex);
                System.Console.WriteLine(ex.Message);
                ReConnect(4);
            }
        }

        void websocket_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            Logger.Logger.Log.Debug("WebSocket Error");
            if (OnErrorReceived != null)
            {
                OnErrorReceived("Reconnect: "+e.Exception.Message);
            }
            Logger.Logger.Log.Error("WebSocket Error" + e.Exception);
        }

        public void ReConnect(int count)
        {
            try
            {
                Logger.Logger.Log.Debug(string.Format("ReConnect Connecting count {0}", count));
                if (count <= 3)
                {
                    Logger.Logger.Log.Info(string.Format("socket reconnect :{0} server {1}", count, priUri));
                    if (string.IsNullOrEmpty(priUri))
                        return;
                    DestoryEvent(websocket);
                    websocket = new WebSocket(priUri);
                    DestoryEvent(websocket);
                    CreateEvent(websocket);
                    websocket.Open();                   
                }
                else if (count > 3 && count <= 6)
                {
                    if (string.IsNullOrEmpty(secUri))
                        return;

                    Logger.Logger.Log.Info(string.Format("socket reconnect :{0} server {1}", count, secUri));
                    DestoryEvent(websocket);
                    websocket = new WebSocket(secUri);
                    DestoryEvent(websocket);
                    CreateEvent(websocket);
                    websocket.Open();
                }
            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Error(string.Format("Message {4} Count {0} Primary server {1}, Secondary server {2}", count,priUri, secUri,ex.StackTrace),ex);
            }
        }

        private void CreateEvent(WebSocket _websocket)
        {
            _websocket.Opened -= new EventHandler(websocket_Opened);
            _websocket.DataReceived -= new EventHandler<DataReceivedEventArgs>(websocket_DataReceived);
            _websocket.MessageReceived -= new EventHandler<MessageReceivedEventArgs>(websocket_MessageReceived);
            _websocket.Error -= new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(websocket_Error);
            _websocket.Closed -= websocket_Closed;

            _websocket.Opened += new EventHandler(websocket_Opened);
            _websocket.DataReceived += new EventHandler<DataReceivedEventArgs>(websocket_DataReceived);
            _websocket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(websocket_MessageReceived);
            _websocket.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(websocket_Error);
            _websocket.Closed += websocket_Closed;
            _websocket.EnableAutoSendPing = false;
        }

        private void DestoryEvent(WebSocket _websocket)
        {
            if (_websocket == null)
                return;
            
            _websocket.Opened -= new EventHandler(websocket_Opened);
            _websocket.DataReceived -= new EventHandler<DataReceivedEventArgs>(websocket_DataReceived);
            _websocket.MessageReceived -= new EventHandler<MessageReceivedEventArgs>(websocket_MessageReceived);
            _websocket.Error -= new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(websocket_Error);
            _websocket.Closed -= websocket_Closed;
        }

        void websocket_Closed(object sender, EventArgs e)
        {
            if (timer!=null && timer.Enabled)
            {
                ReConnect(0);
                Logger.Logger.Log.Debug("Application is running but un WebSocket Closed!.");
            }
         
            Logger.Logger.Log.Debug("WebSocket Closed!.");
        }

        public int GetMonitorID()
        {
            return _monitorID;
        }
        public void SetMonitorID(int id)
        {
            _monitorID = id;
        }

        public void StartMonitor(string extension, string agentid = "", string password = "", string queue = "")
        {
            Logger.Logger.Log.Debug(string.Format( "Start monitor extension {0}, Agent ID {1}, Queue {2}",extension,agentid,queue));

            currentMonitoringStation = extension;
            currentAgentId = (agentid=="0")?"":agentid;
            currentAgentsPassword = password;
            currentAgentsQueue = queue;
            currentExtension = extension;
            IsACD = string.IsNullOrEmpty(currentAgentId) ? false : true;

            string initialMessage = "0" + extension + "-" + DateTime.Now.Millisecond;
            string monitorRequestMessage = "<Monitor><dID>" + extension + "</dID><cInID>" + GetInvokeId().ToString() + "</cInID></Monitor>";

            SendMsg(initialMessage);
            SendMsg(monitorRequestMessage);
            Logger.Logger.Log.Info(string.Format("CtiCall info {0}",monitorRequestMessage));
        }

        public void StopMonitor()
        {
            Logger.Logger.Log.Debug(string.Format("Stoping monitor!.."));
            if (currentMonitoringStation != string.Empty)
            {
                
                string initialMessage = "0" + currentMonitoringStation + "-" + DateTime.Now.Millisecond;
                string monitorRequestMessage = "<MonitorStop><mCRI>" + currentMonitoringStation + "</mCRI><cInID>" + GetInvokeId().ToString() + "</cInID></MonitorStop>";
               // SendMsg(initialMessage);
               // SendMsg(monitorRequestMessage);
                currentMonitoringStation = string.Empty;
                timer.Stop();      
                timer = null;                        
                websocket.Close();
                DestoryEvent(websocket);
                Logger.Logger.Log.Info(string.Format("CtiCall info>>> {0}", monitorRequestMessage));
            }
        }

        public void AgentForceLogout()
        {
            if (timer != null)
            {                
                timer.Stop();
                timer = null;
            }
        }

        public void AgentLogin(string AgentID, string password, string extension, string queue)
        {
            Logger.Logger.Log.Debug(string.Format("Agent Login Agent ID {0}, Agent Extension {1}, Queue {2}",AgentID,extension,queue));
            currentMonitoringStation = extension;
            currentAgentId = (AgentID=="0")?"":AgentID;
            currentExtension = extension;
            currentAgentsQueue = queue;
            currentAgentsPassword = password;
            IsACD = string.IsNullOrEmpty(currentAgentId) ? false : true;
            string initialMessage = "0" + extension + "-" + DateTime.Now.Millisecond;
            SendMsg(initialMessage);
            // Set the agent to logged in state
            string login = "<AState><dvc>" + extension + "</dvc><ID>" + currentAgentId + "</ID><pwd>" + password + "</pwd><grp>" + "" + "</grp><AS>" + "0" + "</AS><RC>" + "1" + "</RC><cInID>" + GetInvokeId().ToString() + "</cInID></AState>";
            SendMsg(login);
            Logger.Logger.Log.Info(string.Format("CtiCall info>>> {0}", login));
        }

        public void AgentLogout()
        {
            Logger.Logger.Log.Debug(string.Format("Agent Logout!.."));        
            StopMonitor();
        }

        public void SocketClose()
        {
            
            //websocket.Close();
            DestoryEvent(websocket);
        }

        public void OnCallAnswered(string callid)
        {
            Logger.Logger.Log.Debug(string.Format("Agent Call Answered"));
            //foreach (var item in callData)
            //{
            //    if (item.Key!= activeCall)
            //    {
            //        OnHoldCall(item.Key);
            //    }
            //}

            if (websocket.State == WebSocketState.Open)
            {
                SendMsg('0' + currentExtension + "-" + DateTime.Now.Millisecond);
                string str = "<AnCall><cID>" + callid + "</cID><dID>" + currentExtension + "</dID><cInID>" + GetInvokeId() + "</cInID></AnCall>";
                SendMsg(str);
                Logger.Logger.Log.Info(string.Format("CtiCall info >>> {0}", str));
            }
        }

        public void OnCallDropped(string callid)
        {
            Logger.Logger.Log.Debug(string.Format("Agent Call Dropped"));           
            SendMsg('0' + currentExtension + "-" + DateTime.Now.Millisecond);
            string str = "<DCall><cID>" + callid + "</cID><dID>" + currentExtension + "</dID><cInID>" + GetInvokeId() + "</cInID></DCall>";
            SendMsg(str);            
            Logger.Logger.Log.Info(string.Format("CtiCall info >>> {0}", str));
        }

        public void OnDropLastParty(string extension,string callid)
        {
            SendMsg('0' + currentExtension + "-" + DateTime.Now.Millisecond);
            string str = "<DLC><cID>" + callid + "</cID><dID>" + extension + "</dID><cInID>" + GetInvokeId() + "</cInID></DLC>";
            SendMsg(str);
            Logger.Logger.Log.Info(string.Format("CtiCall info >>> {0}", str));
        }

        private void RemoveCallFromList(int removingCall)
        {
            Logger.Logger.Log.Debug(string.Format("Removing Call List"));
            try
            {                
                var callDataItem = callData.FirstOrDefault(x => x.Key == removingCall);
                callData.Remove(callDataItem.Key);
               
            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Error(string.Format("CtiCall info >>> {0}", ex.Message),ex);
            }
        }

        public void OnCallRetrieve(int activeCall)
        {
            Logger.Logger.Log.Debug(string.Format("Active Call Rertrieve {0}",activeCall));
            string msgRetrieveCall = "<RCall><cID>" + activeCall + "</cID><dID>" + currentExtension + "</dID><cInID>" + GetInvokeId() + "</cInID></RCall>";
            SendMsg('0' + currentExtension + "-" + DateTime.Now.Millisecond);
            SendMsg(msgRetrieveCall);
            Logger.Logger.Log.Info(string.Format("CtiCall info>>> {0}", msgRetrieveCall));
        }

        public void MakeCall(string targetNumber)
        {
            destAddrConsult = targetNumber;
            Logger.Logger.Log.Debug(string.Format("Make Call {0}", targetNumber));
            SendMsg('0' + currentExtension + "-" + DateTime.Now.Millisecond);
            string str = "<MCall><caD>" + currentExtension + "</caD><cDN>" + targetNumber + "</cDN><uData></uData><cInID>" + GetInvokeId() + "</cInID></MCall>";
            SendMsg(str);
            Logger.Logger.Log.Info(string.Format("CtiCall info>>> {0}", str));
        }

        public void OnCallTransferred(string callid, string targetNumber, bool isBlindTransfer,string UUI)
        {
            Logger.Logger.Log.Debug(string.Format("Active Call Rertrieve {0}", callid));
            blindTransfer = isBlindTransfer;
            ConsultCall(callid,targetNumber, UUI);
            Logger.Logger.Log.Info(string.Format("CtiCall info>>> {0}", targetNumber));
        }

        public void OnCallConferenced(string callid,string targetNumber, bool isBlindConference,string UUI)
        {
            blindConference = isBlindConference;
            ConsultCall(callid,targetNumber, UUI);
            Logger.Logger.Log.Info(string.Format("CtiCall info>>> {0}", targetNumber));
        }

        public void OnCallHoldOrRetrieve(bool hold,string callid)
        {
            if (hold)
            {
                string msgHoldCall = "<HCall><cID>" + callid + "</cID><dID>" + currentExtension + "</dID><cInID>" + GetInvokeId() + "</cInID></HCall>";
                SendMsg('0' + currentExtension + "-" + DateTime.Now.Millisecond);
                SendMsg(msgHoldCall);
                Logger.Logger.Log.Info(string.Format("CtiCall info>>> {0}", msgHoldCall));
            }
            else
            {
                string msgRetrieveCall = "<RCall><cID>" + callid + "</cID><dID>" + currentExtension + "</dID><cInID>" + GetInvokeId() + "</cInID></RCall>";
                SendMsg('0' + currentExtension + "-" + DateTime.Now.Millisecond);
                SendMsg(msgRetrieveCall);
                Logger.Logger.Log.Info(string.Format("CtiCall info>>> {0}", msgRetrieveCall));
            }
        }

        public void OnHoldCall(int callId)
        {
            string msgHoldCall = "<HCall><cID>" + callId + "</cID><dID>" + currentExtension + "</dID><cInID>" + GetInvokeId() + "</cInID></HCall>";
            SendMsg('0' + currentExtension + "-" + DateTime.Now.Millisecond);
            SendMsg(msgHoldCall);
            Logger.Logger.Log.Info(string.Format("CtiCall info>>> {0}", msgHoldCall));
        }

        public void ConsultCall(string callid,string targetNumber,string UUI)
        {
            heldCall = Convert.ToInt32(callid);            
            destAddrConsult = targetNumber;
            SendMsg('0' + currentExtension + "-" + DateTime.Now.Millisecond);
            string str = "<CnC><eCl><cID>" + callid + "</cID><dID>" + currentExtension + "</dID></eCl><cnD>" + targetNumber + "</cnD><uD>" + UUI + "</uD><cInID>" + GetInvokeId() + "</cInID></CnC>";
            SendMsg(str);
            Logger.Logger.Log.Info(string.Format("CtiCall info>>> {0}", str));
        }

        public void OnConsultCallResponse(string activeCallId)
        {
            if (blindTransfer)
            {
                if (heldCall != 0 && destAddrConsult!="")
                {
                    SendMsg('0' + currentExtension + "-" + DateTime.Now.Millisecond);
                    string str1 = "<TCall><hC><cID>" + heldCall + "</cID><dID>" + currentExtension + "</dID></hC><atC><cID>" + activeCallId + "</cID><dID>" + destAddrConsult + "</dID></atC><cInID>" + GetInvokeId() + "</cInID></TCall>";
                    SendMsg(str1);
                    Logger.Logger.Log.Info(string.Format("CtiCall info>>> {0}", str1));
                }
            }
            else if (blindConference)
            {
                if (heldCall != 0 && destAddrConsult != "")
                {
                    SendMsg('0' + currentExtension + "-" + DateTime.Now.Millisecond);
                    string str1 = "<CoCall><hC><cID>" + heldCall + "</cID><dID>" + currentExtension + "</dID></hC><atC><cID>" + activeCallId + "</cID><dID>" + destAddrConsult + "</dID></atC><cInID>" + GetInvokeId() + "</cInID></CoCall>";                   
                    SendMsg(str1);
                    Logger.Logger.Log.Info(string.Format("CtiCall info>>> {0}", str1));
                }
            }           
        }

        public void CompleteTransfer(string heldcallid, string transferedCallid)
        {
            SendMsg('0' + currentExtension + "-" + DateTime.Now.Millisecond);
            string str1 = "<TCall><hC><cID>" + heldcallid + "</cID><dID>" + currentExtension + "</dID></hC><atC><cID>" + transferedCallid.ToString() + "</cID><dID>" + destAddrConsult + "</dID></atC><cInID>" + GetInvokeId() + "</cInID></TCall>";
            SendMsg(str1);
            Logger.Logger.Log.Info(string.Format("CtiCall info>>> {0}", str1));
        }

        public void CompleteConference(string heldcallid, string transferedCallid)
        {
            SendMsg('0' + currentExtension + "-" + DateTime.Now.Millisecond);
            string str1 = "<CoCall><hC><cID>" + heldcallid + "</cID><dID>" + currentExtension + "</dID></hC><atC><cID>" + transferedCallid + "</cID><dID>" + destAddrConsult + "</dID></atC><cInID>" + GetInvokeId() + "</cInID></CoCall>";
            SendMsg(str1);
            Logger.Logger.Log.Info(string.Format("CtiCall info>>> {0}", str1));
        }

        public void GetLogicalDeviceInfo()
        {
            string str1 = "<GLDI><dvc>" + currentExtension + "</dvc><cInID>" + 1 + "</cInID></GLDI>";
            SendMsg(str1);
            Logger.Logger.Log.Info(string.Format("CtiCall info>>> {0}", str1));

        }

        public void SetAgentState(int agentState, string reasonCode = "0")
        {
            SendMsg('0' + currentExtension + "-" + DateTime.Now.Millisecond);
            string msg = "<AState><dvc>" + currentExtension + "</dvc><ID>" + currentAgentId + "</ID><pwd>" + currentAgentsPassword + "</pwd><grp>" + currentAgentsQueue + "</grp><AS>" + agentState + "</AS><RC>" + reasonCode + "</RC><cInID>" + GetInvokeId() + "</cInID></AState>";
            SendMsg(msg);
            Logger.Logger.Log.Info(string.Format("CtiCall info>>> {0}", msg));
        }

        public void SetActiveCall(int activeCallId)
        {
            this.activeCall = activeCallId;
           // callIds.Add(activeCallId);
        }

        public void SetActiveCall(string activeCallId)
        {            
            this.activeCall = callData.FirstOrDefault(x => x.Key == Convert.ToInt32(activeCallId)).Key;
        }

        public void SetCallData(int activeCallId, string ani)
        {
            
            if (!callData.Keys.Contains(activeCallId))
            {
                callData.Add(activeCallId, ani);
            }
            else
            {
                callData[activeCallId] = ani;
            }
        }

        public void SetExtensionAniData(string ani,string extension)
        {
            if (!ExtensionAniData.Keys.Contains(ani))
            {
                ExtensionAniData.Add(ani, extension);
            }
            else
            {
                ExtensionAniData[ani] = extension;
            }
        }

        public void ReSetExtensionAniData(string ani)
        {
            if ((!string.IsNullOrEmpty(ani)) && ExtensionAniData.Keys.Contains(ani))
                ExtensionAniData.Remove(ani);
        }

        public string GetExtensionAniData(string extension)
        {
            string ani = string.Empty;
            ani = ExtensionAniData.FirstOrDefault(x => x.Value == extension).Key;
            return ani;
        }

        public void ReSetCallData(string callid)
        {
            if (callData.Keys.Contains(Convert.ToInt32(callid)))
                callData.Remove(Convert.ToInt32(callid));       
        }

        public string GetAni(int callId)
        {            
            string ani = string.Empty;
            ani = callData.FirstOrDefault(x => x.Key == callId).Value;
            return ani;
        }

        public bool IsBlindTransferInProgress()
        {
            return blindTransfer;
        }

        public void QueryAgentState(int extension)
        {
            string str1 = "<QASS><dID>" + extension + "</dID><cInID>" + GetInvokeId() + "</cInID></QASS>";
            SendMsg(str1);
            Logger.Logger.Log.Info(string.Format("CtiCall info>>> {0}", str1));
        }

        public void SnapshotDevice(int extension)
        {
            string str1 = "<SSD><sO>" + extension + "</dID><cInID>" + GetInvokeId() + "</cInID></sO> </SSD>";
            SendMsg(str1);
            Logger.Logger.Log.Info(string.Format("CtiCall info>>> {0}", str1));
        }

        public int GetCallDataCount()
        {
            return callData.Count();
        }

        public void SendDTMF(string callid, string entered_digits) {
            string str1 = "<DTMF><dID>" + currentExtension + "</dID><cID>" + callid+ "</cID><Digits>" + entered_digits + "</Digits><cInID>" + GetInvokeId() + "</cInID></DTMF>";
            SendMsg(str1);
            Logger.Logger.Log.Info(string.Format("CtiCall info>>> {0}", str1));
        }

        private int GetInvokeId()
        {
            
            if (invokeId > 9995)
                invokeId = 0;

            invokeId++;
            //localStorage.invokeId = invokeId + 1;
            return invokeId;
        }

        private void SendMsg(string msg)
        {
            try
            {
                if (websocket.State == WebSocketState.Open)
                    websocket.Send(msg);                
            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Debug(string.Format("Failed Sending Msg {0}", msg));             
            }
        }
    }
}
