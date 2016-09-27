// ===========================================================================================
//  Oracle RightNow Connect


//  CTI Sample Code
// ===========================================================================================
//  Copyright © Oracle Corporation.  All rights reserved.
// 
//  Sample code for training only. This sample code is provided "as is" with no warranties 
//  of any kind express or implied. Use of this sample code is pursuant to the applicable
//  non-disclosure agreement and or end user agreement and or partner agreement between
//  you and Oracle Corporation. You acknowledge Oracle Corporation is the exclusive
//  owner of the object code, source code, results, findings, ideas and any works developed
//  in using this sample code.
// ===========================================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using Oracle.RightNow.Cti.Model;
using Oracle.RightNow.Cti.Providers.CtiServiceProvider.Properties;
using Oracle.RightNow.Cti.CtiServiceProvider;
using Oracle.RightNow.Cti.CtiServiceProvider.Messaging;
using Oracle.RightNow.Cti.CtiServiceProvider.Messaging.Messages;
using RightNow.AddIns.AddInViews;
using Oracle.RightNow.Cti.CtiServiceProvider.Controller.ViewModels;
using System.Xml;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;


namespace Oracle.RightNow.Cti.Providers.CtiServiceProvider
{
    [Export(typeof(IInteractionProvider))]
    public class CtiServiceProvider : ITelephonyProvider, IRemoteMessagingClient, ISwitchMessageProducer
    {
        private readonly object _clientRootSync = new object();
        private readonly object _newInter = new object();
        private readonly List<ISwitchMessageConsumer> _messageConsumers = new List<ISwitchMessageConsumer>();

        private readonly SynchronizationContext _synchronizationContext;

        private SwitchClient _client;
        private IInteraction _currentInteraction;
        private AsyncObservableCollection<IInteraction> _interactions = new AsyncObservableCollection<IInteraction>();
        private AsyncObservableCollection<IInteraction> _interactionsList = new AsyncObservableCollection<IInteraction>();
        private Device _switchDevice;

        public event EventHandler<InteractionEventArgs> CurrentInteractionChanged;
        public event EventHandler<CustomEventArgs> CustomProviderEvent;
        public event EventHandler<InteractionEventArgs> InteractionCompleted;
        public event EventHandler<InteractionEventArgs> InteractionConnected;
        public event EventHandler<InteractionEventArgs> InteractionDisconnected;
        public event EventHandler<InteractionEventArgs> NewInteraction;
        public event EventHandler<InteractionEventArgs> ScreenPopEvent;

        private CtiServiceLibrary.CtiCallInfo ctiCallInfo;
        private CtiServiceController ctiSimCtrl;
        private int activeCallId = 0;
        private Dictionary<int, string> callsList;
        static readonly object _object = new object();
        private List<int> confList = new List<int>();
        private bool bConferencing = false;
        private int previousCall = 0;

        AutoResetEvent _autoReset = new AutoResetEvent(false);        

        public CtiServiceProvider()
        {
            //_synchronizationContext = SynchronizationContext.Current;
            _synchronizationContext = SynchronizationContext.Current ?? new SynchronizationContext();
            ctiCallInfo = CtiServiceLibrary.CtiCallInfo.GetCtiCallInfoObject();
            string server = Oracle.RightNow.Cti.AddIn.MediaBarAddIn.PrimaryEngine;

            // ctiCallInfo.ConnectToServer(server, Oracle.RightNow.Cti.AddIn.MediaBarAddIn.SecondaryEngine);
            ctiCallInfo.OnMessageReceived += ctiCallInfo_OnMessageReceived;
            ctiCallInfo.OnErrorReceived += ctiCallInfo_OnErrorReceived;
            callsList = new Dictionary<int, string>();
        }

        private void ctiCallInfo_OnMessageReceived(string message)
        {
            if (ctiSimCtrl == null)
            {
                ctiSimCtrl = new CtiServiceController(RightNowGlobalContext);
                ctiSimCtrl.OnInteractionQueued += ctiSimCtrl_OnInteractionQueued;
            }
            System.Console.WriteLine(message);
            if (ctiSimCtrl != null)
            {

                string result = string.Empty;
                result = ParseMessage(message.Substring(1));
                lock (result)
                {
                    ProcessMessage(result);
                }
            }
        }

        private void ctiCallInfo_OnErrorReceived(string message)
        {

        }

        private void ctiSimCtrl_OnInteractionQueued(InteractionMessage message)
        {
            // Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            CreateNewInteraction(message);
        }

        string ParseMessage(string message)
        {
            message = message.Replace("&lt;", "<");
            message = message.Replace("&gt;", ">");
            return message;
        }

        private void ProcessMessage(string str)
        {
            try
            {
                if (str.IndexOf("<echo>") == -1)
                {
                    Logger.Logger.Log.Debug(string.Format("Process Message Received, Yet to process respective action: <<< {0}", str));
                }

                if (str.IndexOf("<MonitorR>") != -1)
                {
                    OnMonitorResponse(str);
                }
                else if (str.IndexOf("<Org>") != -1)
                {
                    OnCallOriginated(str);
                }
                else if (str.IndexOf("<DE>") != -1)
                {
                    OnCallDelivered(str);
                }
                else if (str.IndexOf("<Est>") != -1)
                {
                    OnCallEstablished(str);
                }
                else if (str.IndexOf("<Dr>") != -1)
                {
                    OnCallEnd(str);

                }
                else if (str.IndexOf("<CnCR>") != -1)
                {
                    OnConsultCallResponse(str);

                }
                else if (str.IndexOf("<TE>") != -1)
                {
                    XmlDocument xdoc = new XmlDocument();
                    xdoc.LoadXml(str);
                    XmlNode curnode = xdoc.SelectSingleNode("TE/orgCinfo/callingD/text()");
                    XmlNode node = xdoc.SelectSingleNode("TE/transferringDe/extn/dID/text()");
                    XmlNode node1 = xdoc.SelectSingleNode("TE/transferredDe/extn/dID/text()");
                    string ani = node.Value;
                    string dnis = node1.Value;
                    XmlNode Didnode = xdoc.SelectSingleNode("TE/priOC/dID/text()");

                    XmlNode pricall = xdoc.SelectSingleNode("TE/priOC/cID/text()");
                    XmlNode seccall = xdoc.SelectSingleNode("TE/secOC/cID/text()");

                    if (Didnode.Value == ctiCallInfo.CurrentStation)
                    {
                        if (previousCall != 0)
                            callsList.Remove(previousCall);

                        ctiCallInfo.ReSetCallData(pricall.Value);
                        ctiCallInfo.ReSetCallData(seccall.Value);
                    }

                    Logger.Logger.Log.Info(string.Format("<TE> {0}", str));

                }
                else if (str.IndexOf("<Con>") != -1)
                {
                    XmlDocument xdoc = new XmlDocument();
                    xdoc.LoadXml(str);
                    XmlNode node = xdoc.SelectSingleNode("Con/orgCinfo/callingD/text()");
                    XmlNode addedParty = xdoc.SelectSingleNode("Con/aParty/extn/dID/text()");
                    XmlNode prinode = xdoc.SelectSingleNode("Con/priOC/cID/text()");
                    XmlNode secinode = xdoc.SelectSingleNode("Con/secOC/cID/text()");
                    XmlNode secdev = xdoc.SelectSingleNode("Con/secOC/dID/text()");

                    XmlNode condev = xdoc.SelectSingleNode("Con/conD/extn/dID/text()");

                    string ani = node.Value;
                    if (previousCall != 0)
                        callsList.Remove(previousCall);

                    if (condev.Value == ctiCallInfo.CurrentStation)
                    {
                        ctiCallInfo.SetCallData(Convert.ToInt32(secinode.Value), ani);

                        if (ani != addedParty.Value)
                            ctiCallInfo.ReSetCallData(prinode.Value);

                        ctiCallInfo.SetActiveCall(secinode.Value);

                    }
                    else
                    {
                        ctiCallInfo.SetCallData(Convert.ToInt32(secinode.Value), secdev.Value);

                        if (secdev.Value != addedParty.Value)
                            ctiCallInfo.ReSetCallData(prinode.Value);

                        ctiCallInfo.SetActiveCall(secinode.Value);
                    }


                    Logger.Logger.Log.Info(string.Format("<Con> {0}", str));
                }
                else if (str.IndexOf("<AStateR>") != -1)
                {

                }
                else if (str.IndexOf("<Fail>") != -1)
                {
                    OnCallFailed(str);
                }
                else if (str.IndexOf("<GLDIR>") != -1)
                {
                    Logger.Logger.Log.Info(string.Format("GLDIR:<<< {0}", str));
                }
            }
            catch (Exception ex)
            {

                Logger.Logger.Log.Error(string.Format("CtiServiceProver: Message {0}", str));
                Logger.Logger.Log.Error("CtiServiceProver: Failed ", ex);
            }
        }
        
        private void OnCallFailed(string str)
        {
            Logger.Logger.Log.Debug("<<< Call Failed:"+str);
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(str);
            XmlNode Id = xdoc.SelectSingleNode("Fail/mCRI/text()");
            XmlNode callIdNode = xdoc.SelectSingleNode("Fail/failConn/cID/text()");
            XmlNode Didnode = xdoc.SelectSingleNode("Fail/failConn/dID/text()");            
            Dictionary<string, string> dict = new Dictionary<string, string>();
            IInteraction failedCall = Interactions.FirstOrDefault(inter => inter.CallId == callIdNode.Value);
            if (failedCall == null)
            {
                ///ctiSimCtrl.CreateCall(ctiCallInfo.CurrentStation, "", dict, CallType.Outbound, callIdNode.Value, "", false);// false
                ctiSimCtrl.CreateCall("", "", dict, CallType.Outbound, Convert.ToInt32(callIdNode.Value), "", false,true);
            }
        }
        
        private void OnMonitorResponse(string str)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(str);

            XmlNode node = xdoc.SelectSingleNode("MonitorR/mCRI/text()");
            int id = Convert.ToInt32(node.Value);
            ctiCallInfo.SetMonitorID(id);
        }

        private void OnCallDelivered(string str)
        {
            try
            {
                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(str);

                XmlNode node = xdoc.SelectSingleNode("DE/callingD/extn/dID/text()");
                XmlNode node1 = xdoc.SelectSingleNode("DE/calledD/extn/dID/text()");
                XmlNode callIdNode = xdoc.SelectSingleNode("DE/con/cID/text()");
                XmlNode UUINode = xdoc.SelectSingleNode("DE/uD/text()");
                XmlNode UUINode2 = xdoc.SelectSingleNode("DE/orgCinfo/ui/text()");
                XmlNode Didnode = xdoc.SelectSingleNode("DE/con/dID/text()");
                XmlNode isconsultcalling = xdoc.SelectSingleNode("DE/orgCinfo/callingD/text()");
                XmlNode isconsultcalled = xdoc.SelectSingleNode("DE/orgCinfo/calledD/text()");
                string UUI = "";
                if (UUINode.Value == "|")
                    UUI = UUINode2 != null ? UUINode2.Value : "";
                else
                    UUI = UUINode.Value.Split('|')[0];


                string ani = node.Value;
                string dnis = node1.Value;
                if (activeCallId != 0)
                    previousCall = activeCallId;
                activeCallId = Int32.Parse(callIdNode.Value);

                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("UUI", UUI);
                CallType calltype = CallType.Inbound;
                bool ConferencedCall = false;
                if (isconsultcalling != null)
                {
                    string consultcalling = isconsultcalling != null ? isconsultcalling.Value : "";
                    string consultcalled = isconsultcalled != null ? isconsultcalled.Value : "";
                    dict.Add("Consult", consultcalling + "|" + consultcalled);
                    calltype = CallType.Consult;
                    //ConferencedCall = true;
                }

                if (ani != ctiCallInfo.CurrentStation)
                {
                    if (callsList.FirstOrDefault(x => x.Key == activeCallId).Value != ani)
                    {
                        callsList.Add(activeCallId, ani);
                       
                        ctiCallInfo.SetActiveCall(activeCallId);
                        ctiCallInfo.SetCallData(activeCallId, ani);

                        ctiCallInfo.SetExtensionAniData(ani, Didnode.Value);
                        ctiSimCtrl.CreateCall(ani, dnis, dict, calltype, activeCallId, ani, ConferencedCall);//true
                        Logger.Logger.Log.Info(string.Format("CtiServiceProvider: Creating {0}", calltype.ToString()));
                    }
                }
                else
                {
                    if (ctiCallInfo.blindTransfer)
                    {
                        ctiCallInfo.blindTransfer = false;
                        return;
                    }

                    if (ctiCallInfo.blindConference)
                    {
                        ctiCallInfo.blindConference = false;
                        return;
                    }

                    if (callsList.FirstOrDefault(x => x.Key == activeCallId).Value != dnis)
                    {
                        callsList.Add(activeCallId, dnis);
                        
                        ctiCallInfo.SetActiveCall(activeCallId);
                        ctiCallInfo.SetCallData(activeCallId, dnis);                    
                        Logger.Logger.Log.Info(string.Format("CtiServiceProvider: Creating {0}", CallType.Outbound.ToString()));
                    }                    
                    ctiCallInfo.SetExtensionAniData(dnis, Didnode.Value);
                    SwitchInteraction inter = ctiSimCtrl.Interactions.FirstOrDefault(p => p.CallID == activeCallId);
                    if (inter != null)
                    {
                        inter.CallType = calltype;
                        inter.ConferencedCall = ConferencedCall;
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Error("CtiServiceProvider", ex);
            }
        }

        private void OnCallOriginated(string str)
        {
            try
            {
                Logger.Logger.Log.Debug(string.Format(" Ctiservice Service provider<<: {0}", str));

                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(str);
                XmlNode Id = xdoc.SelectSingleNode("Org/mCRI/text()");
                XmlNode callIdNode = xdoc.SelectSingleNode("Org/orconn/cID/text()");
                XmlNode aninode = xdoc.SelectSingleNode("Org/callingD/extn/dID/text()");
                XmlNode calleddnis = xdoc.SelectSingleNode("Org/calledD/extn/dID/text()");
                XmlNode Didnode = xdoc.SelectSingleNode("Org/orconn/dID/text()");
                string dnis = calleddnis.Value;
                string ani = aninode.Value;
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("UUI", "");
                activeCallId = Int32.Parse(callIdNode.Value);
                if (ctiCallInfo.blindTransfer)
                {
                    ctiCallInfo.blindTransfer = false;
                    //return;
                }

                if (ctiCallInfo.blindConference)
                {
                    ctiCallInfo.blindConference = false;
                    //return;
                }

                if (callsList.FirstOrDefault(x => x.Key == activeCallId).Value != dnis)
                {
                    callsList.Add(activeCallId, dnis);
                    ctiCallInfo.SetActiveCall(activeCallId);
                    ctiCallInfo.SetCallData(activeCallId, dnis);
                    _autoReset.Reset();
                    ctiSimCtrl.CreateCall(dnis, ani, dict, CallType.Outbound, activeCallId, ani, false);// false
                    _autoReset.WaitOne(1000);
                    

                    Logger.Logger.Log.Info(string.Format("CtiServiceProvider: Oncallorginated {0}", CallType.Outbound.ToString()));                    
                }
            }
            catch (Exception ex)
            {                
                Logger.Logger.Log.Error(string.Format("CtiServiceProvider: Oncallorginated failed {0}", ex.StackTrace));
                Logger.Logger.Log.Error("CtiServiceProvider: Oncallorginated ", ex);                
            }
        }

        private void OnCallEstablished(string str)
        {
            try
            {
                Logger.Logger.Log.Debug(string.Format("CtiServiceProvider:  <<{0}", str));
                // Call is established in phone. Need to handle
                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(str);

                XmlNode node = xdoc.SelectSingleNode("Est/callingD/extn/dID/text()");
                XmlNode node1 = xdoc.SelectSingleNode("Est/calledD/extn/dID/text()");
                XmlNode callIdNode = xdoc.SelectSingleNode("Est/econ/cID/text()");

                XmlNode isconsultcalling = xdoc.SelectSingleNode("Est/orgCinfo/callingD/text()");
                XmlNode isconsultcalled = xdoc.SelectSingleNode("Est/orgCinfo/calledD/text()");

                string ani = node.Value;
                string dnis = node1.Value;
                activeCallId = Int32.Parse(callIdNode.Value);
                ctiCallInfo.SetActiveCall(activeCallId);            
                Logger.Logger.Log.Info(string.Format("CtiServiceProvider: activeCallId {0}", activeCallId));
            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Error("CtiServiceProvider: activeCallId ", ex);
            }
        }

        private void OnCallEnd(string str)
        {
            // Call is dropped from phone.
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(str);
            XmlNode node = xdoc.SelectSingleNode("Dr/drconn/cID/text()");
            string callID = node.Value;
            int callId = Convert.ToInt32(callID);
            string ani = ctiCallInfo.GetAni(callId);
            callsList.Remove(callId);
            
            Logger.Logger.Log.Info(string.Format("CtiServiceProvider: ani {0}", ani));
        }

        private void OnConsultCallResponse(string str)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(str);
            XmlNode callIdNode = xdoc.SelectSingleNode("CnCR/iC/cID/text()");
            string activeCall = callIdNode.Value.Trim();

            ctiCallInfo.OnConsultCallResponse(activeCall);
            Logger.Logger.Log.Info(string.Format("CnCR:<<< {0}", str));
            Logger.Logger.Log.Info(string.Format("CtiServiceProvider: activeCallId {0}", activeCallId));
        }



        [Import]
        public IAgent Agent { get; set; }

        public SwitchCredentials Credentials { get; private set; }

        public ScreenPopConfig ScreenPop { get; private set; }

        [Import]
        public ICredentialsProvider CredentialsProvider { get; set; }

        public IInteraction CurrentInteraction
        {
            get
            {
                return _currentInteraction;
            }
            set
            {
                if (_currentInteraction != value)
                {
                    _currentInteraction = value;
                    OnCurrentInteractionChanged();
                }
            }
        }

        [Import]
        public IDevice Device { get; set; }

        public IList<DnsEndPoint> Endpoints { get; set; }

        public AsyncObservableCollection<IInteraction> Interactions
        {
            get
            {
                return _interactions;
            }
            private set
            {
                _interactions = value;
            }
        }

        public AsyncObservableCollection<IInteraction> InteractionsList
        {
            get
            {
                return _interactionsList;
            }
            private set
            {
                _interactionsList = value;
            }
        }

        public string Name
        {
            get
            {
                return Resources.ProviderName;
            }
        }

        public string Platform
        {
            get
            {
                return Resources.ProviderPlatform;
            }
        }

        public Dictionary<string, string> Properties
        {
            get
            {
                return null;
            }
        }

        [Import]
        public IGlobalContext RightNowGlobalContext { get; set; }

        public TransferTypes SupportedTransferTypes
        {
            get
            {
                return TransferTypes.Warm | TransferTypes.Cold | TransferTypes.Conference | TransferTypes.OutboundDialing;
            }
        }

        internal SwitchClient Client
        {
            get
            {
                if (_client == null)
                {
                    lock (_clientRootSync)
                    {
                        if (_client == null)
                        {
                            initializeClient();
                        }
                    }
                }

                return _client;
            }
        }

        ICall IInteractionProvider<ICall>.CurrentInteraction
        {
            get
            {
                return CurrentInteraction as ICall;
            }
        }

        AsyncObservableCollection<ICall> IInteractionProvider<ICall>.Interactions
        {
            get
            {
                //return _interactions.OfType<ICall>().ToList();
                return new AsyncObservableCollection<ICall>( _interactions.OfType<ICall>().ToList());
            }
        }

        public void CompleteInteraction()
        {
            var interaction = CurrentInteraction;

            if (interaction != null)
            {
                Client.Request(new InteractionRequestMessage
                {
                    Action = InteractionRequestAction.Complete,
                    InteractionId = new Guid(interaction.Id),
                });
            }
        }


        public void HandleMessage(Message message)
        {
            Logger.Logger.Log.Info("CtiServiceProvider Message Handle" + message.Type);
            Parallel.ForEach(_messageConsumers, c => c.HandleMessage(message));

            switch (message.Type)
            {
                case SwitchMessageType.Interaction:
                    handleInteractionMessage((InteractionMessage)message);
                    break;
                case SwitchMessageType.Disconnected:
                    _client = null;
                    break;
            }
            Logger.Logger.Log.Info("CtiServiceProvider Message Handle completed " + message.Type);
        }

        public void Initialize()
        {
            if (Credentials == null && CredentialsProvider != null)
            {

                var authenticationResult = CredentialsProvider.GetCredentials();
                Credentials = authenticationResult.AgentCredentials;

                Logger.Logger.Log.Info("Inside Initialize() authenticationResult.AgentCredentials.Name :->" + authenticationResult.AgentCredentials.Name);
                Logger.Logger.Log.Info("Inside Initialize() authenticationResult.AgentCredentials.Password :->" + authenticationResult.AgentCredentials.Password);
                Logger.Logger.Log.Info("Inside Initialize() authenticationResult.AgentCredentials.Id :->" + authenticationResult.AgentCredentials.Id);
                Logger.Logger.Log.Info("Inside Initialize() authenticationResult.AgentCredentials.AgentID :->" + authenticationResult.AgentCredentials.AgentID);


                ScreenPop = ((CtiServiceAgent)Agent).InteractionManager.AgentScreenPop;
                Device.Address = authenticationResult.LocationInfo.DeviceAddress;
                ((CtiServiceAgent)Agent).Name = authenticationResult.AgentCredentials.Name;

            }
        }

        public void ReInitialize(string agentID, string password, string location, string queue)
        {
            var authenticationResult = CredentialsProvider.ReSetCredentials(agentID, password, location, queue);

            Logger.Logger.Log.Info("Inside ReInitialize() authenticationResult.AgentCredentials.Name :->" + authenticationResult.AgentCredentials.Name);
            Logger.Logger.Log.Info("Inside ReInitialize() authenticationResult.AgentCredentials.Password :->" + authenticationResult.AgentCredentials.Password);
            Logger.Logger.Log.Info("Inside ReInitialize() authenticationResult.AgentCredentials.Id :->" + authenticationResult.AgentCredentials.Id);
            Logger.Logger.Log.Info("Inside ReInitialize() authenticationResult.AgentCredentials.AgentID :->" + authenticationResult.AgentCredentials.AgentID);

            Credentials = authenticationResult.AgentCredentials;
            ScreenPop = ((CtiServiceAgent)Agent).InteractionManager.AgentScreenPop;
            Device.Address = authenticationResult.LocationInfo.DeviceAddress;
            ((CtiServiceAgent)Agent).Name = "Guest";         
        }

        public void Login()
        {
            _switchDevice = new Device
            {
                Id = Client.Id,
                Address = Device.Address,
                Agent = new Agent
                {
                    Id = ((CtiServiceAgent)Agent).Id,
                    DisplayName = Credentials.Name
                }
            };
            Client.Connect(_switchDevice);
        }

        public void Login(string agentId, string password, string extension, string queue)
        {
            _switchDevice = new Device
            {
                Id = Client.Id,
                Address = Device.Address,
                Agent = new Agent
                {
                    Id = ((CtiServiceAgent)Agent).Id,
                    DisplayName = ""
                }
            };
            Client.Connect(_switchDevice);
        }

        public void Logout()
        {
            Client.Disconnect(_switchDevice);
            ctiCallInfo.AgentLogout();
        }

        public void ReLogin(string agentID, string password, string location, string queue)
        {
            ReInitialize(agentID, password, location, queue);
            _switchDevice = new Device
            {
                Id = Client.Id,
                Address = Device.Address,
                Agent = new Agent
                {
                    Id = ((CtiServiceAgent)Agent).Id,
                    DisplayName = ""
                }
            };
            Client.Connect(_switchDevice);
        }

        public void SendNotification(CtiNotification notification)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnCurrentInteractionChanged()
        {
            var temp = CurrentInteractionChanged;
            if (temp != null)
            {
                temp(this, new InteractionEventArgs(CurrentInteraction));
            }
        }

        protected virtual void OnInteractionCompleted(InteractionEventArgs interactionEventArgs)
        {
            var temp = InteractionCompleted;
            if (temp != null)
            {
                temp(this, interactionEventArgs);
            }
        }

        protected virtual void OnInteractionConnected(InteractionEventArgs interactionEventArgs)
        {
            var temp = InteractionConnected;
            if (temp != null)
            {
                temp(this, interactionEventArgs);
            }
        }

        protected virtual void OnInteractionDisconnected(InteractionEventArgs interactionEventArgs)
        {
            var temp = InteractionDisconnected;
            if (temp != null)
            {
                temp(this, interactionEventArgs);
            }
        }

        private void CreateNewInteraction(InteractionMessage message)
        {
            lock (_object)
            {
                try
                {
                    int matchingCall = 0;
                    foreach (CtiServiceCall serviceCall in Interactions)
                    {
                        if (serviceCall.Address == message.Interaction.SourceAddress && serviceCall.CallType == message.Interaction.CallType)
                        {
                            matchingCall++;
                        }
                    }

                    if (matchingCall > 0)
                    {
                        return;
                    }

                    CtiServiceInteraction interaction = null;

                    interaction = new CtiServiceCall(this)
                    {
                        Dnis = message.Interaction.Queue,
                        CallType = message.Interaction.CallType,
                        Order = Interactions.Count,
                        CallId = message.Interaction.CallID.ToString(),
                        CurrentExtension=message.Interaction.CurrentExtension,
                        BlindCall=message.Interaction.BlindCall,
                        ConferencedCall=message.Interaction.ConferencedCall,
                        isCallFailed=message.Interaction.isCallFailed
                    };

                    if (interaction != null)
                    {
                        interaction.Id = message.Interaction.Id.ToString();
                        interaction.Address = message.Interaction.SourceAddress;
                        //interaction.StartTime = DateTime.Now;
                        interaction.State = interaction.IsRealTime ? InteractionState.Ringing : InteractionState.Active;
                        interaction.Queue = message.Interaction.Queue;
                        interaction.InteractionData = message.Interaction.Properties;
                        interaction.ScreenPopConfiguration = ScreenPop;
                        interaction.CurrentExtension = message.Interaction.CurrentExtension;
                        interaction.BlindCall = message.Interaction.BlindCall;
                        interaction.isCallFailed = message.Interaction.isCallFailed;
                        
                        //InteractionState state = cIInformation(interaction);
                        //Logger.Logger.Log.Debug(string.Format("State Value from CIInformation: {0}", state));
                        //if (state == InteractionState.Chat) interaction.State = InteractionState.Chat;
                        //else if (state == InteractionState.Incident) interaction.State = InteractionState.Incident;
                        //else interaction.State = interaction.IsRealTime ? InteractionState.Ringing : InteractionState.Active;
                        if (message.Interaction.ReferenceId != null)
                        {
                            interaction.AdditionalIdentifiers.Add("ReferenceId", message.Interaction.ReferenceId);
                        }

                        Interactions.Add(interaction);
                        CurrentInteraction = interaction;

                        OnNewInteraction(new InteractionEventArgs(interaction));
                        Logger.Logger.Log.Info(string.Format("Created New Interaction {0}", message));
                    }
                    _autoReset.Set();
                }
                catch (Exception ex)
                {
                    Logger.Logger.Log.Error("Error in" + ex.Message);
                }
            }
        }


        private void handleInteractionMessage(InteractionMessage message)
        {
            switch (message.Action)
            {
                case InteractionMessageAction.Created:
                    {
                        CreateNewInteraction(message);
                    }
                    break;

                case InteractionMessageAction.Assigned:
                    {
                        IInteraction interaction = Interactions.FirstOrDefault(i => string.Compare(i.Id, message.Interaction.Id.ToString()) == 0);

                        if (interaction != null)
                        {
                            ((CtiServiceInteraction)interaction).State = InteractionState.Active;

                            OnInteractionConnected(new InteractionEventArgs(interaction));
                        }
                    }
                    break;
                case InteractionMessageAction.StateChanged:
                    {
                        IInteraction interaction = Interactions.FirstOrDefault(i => string.Compare(i.Id, message.Interaction.Id.ToString()) == 0);

                        switch (message.Interaction.State)
                        {
                            case SwitchInteractionState.Active:
                                break;
                            case SwitchInteractionState.Held:
                                break;
                            case SwitchInteractionState.Disconnected:

                            case SwitchInteractionState.Completed:
                                if (interaction != null)
                                {
                                    OnInteractionDisconnected(new InteractionEventArgs(interaction));
                                }                                                              
                                Interactions.Remove(interaction);
                                OnInteractionCompleted(new InteractionEventArgs(interaction));
                                CurrentInteraction = Interactions.FirstOrDefault();
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void initializeClient()
        {
            _client = new SwitchClient(new InstanceContext(this));
            try
            {
                _client.Open();
                (_client as ICommunicationObject).Faulted += CtiServiceController_Faulted;
            }
            catch (EndpointNotFoundException)
            {
                CtiServiceSwitch.Run(RightNowGlobalContext);
                initializeClient();
            }
        }

        private void CtiServiceController_Faulted(object sender, EventArgs e)
        {
           
        }

        void ISwitchMessageProducer.Subscribe(ISwitchMessageConsumer consumer)
        {
            _messageConsumers.Add(consumer);
        }

        void ISwitchMessageProducer.Unsubscribe(ISwitchMessageConsumer consumer)
        {
            _messageConsumers.Remove(consumer);
        }

        private void OnNewInteraction(InteractionEventArgs newInteractionEventArgs)
        {
            var temp = NewInteraction;
            if (temp != null)
            {
                temp(this, newInteractionEventArgs);
            }
        }

        public void OnScreenPop(InteractionEventArgs screenPopInteractionEventArgs)
        {
            var temp = ScreenPopEvent;
            if (temp != null)
            {
                temp(this, screenPopInteractionEventArgs);
            }
        }
    }
}