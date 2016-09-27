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
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Oracle.RightNow.Cti.MediaBar.Properties;
using Oracle.RightNow.Cti.MediaBar.Views;
using Oracle.RightNow.Cti.Model;
using System.Windows.Interop;
using Oracle.RightNow.Cti.CtiServiceLibrary;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;
//using Oracle.RightNow.Cti.ChatSync;

namespace Oracle.RightNow.Cti.MediaBar.ViewModels
{
    [Export]
    public class MediaBarViewModel : RightNowMediaBar
    {
        private readonly DispatcherTimer _timer = new DispatcherTimer();
        private bool _fromlogin = false;
        private int _callCount;
        private bool _IsEnhancedUI;
        private bool _isNotEnhanced;
        private int _emailCount;
        private int _webIncidentCount;
        private bool _isImpliclogin = false;
        private bool _canChangeState;
        private bool _canChangeConnectionState = true;
        private bool _canCompleteInteraction;
        private bool _hasInteractions;
        private bool _showDialOptions;
        private string _conferenceParties = string.Empty;
        private object _AsynMVM = new object();
        private CtiCallInfo ctiCallInfoObject;
        private List<string> confPartiesList;
        private string[] conParties;
        string currentAgentId = string.Empty;
        string currentAgentPassword = string.Empty;
        string currentExtension = string.Empty;
        string currentQueue = string.Empty;
        private bool canCompleteTransfer = false;
        private bool canCompleteConference = false;

        ManualResetEvent _reset = new ManualResetEvent(false);
        AutoResetEvent _autoReset = new AutoResetEvent(false);
        //AutoResetEvent _iterationAutoReset = new AutoResetEvent(false);
        private IInteraction conferencedCall = null;

        public MediaBarViewModel()
        {
            initializeCommands();

            EnableContextSynchronization(SynchronizationContext.Current ?? new SynchronizationContext());
            _timer.Tick += timerTick;
            ctiCallInfoObject = CtiCallInfo.GetCtiCallInfoObject();
            ctiCallInfoObject.OnMessageReceived += ctiCallInfoObject_OnMessageReceived;
            ctiCallInfoObject.OnErrorReceived += ctiCallInfoObject_OnErrorReceived;
            ctiCallInfoObject.OnSocketOpended += ctiCallInfoObject_OnSocketOpended;
            confPartiesList = new List<string>();

           
        }

        void ctiCallInfoObject_OnSocketOpended()
        {
            if (isFailed)
            {
                MessageBox.Show("Successfully reconnected.");
                isFailed = false;
            }
            count = 0;
            ctiCallInfoObject.StartMonitor(currentExtension, currentAgentId, currentAgentPassword, currentQueue);
        }
        int count = 0;
        bool isFailed = false;
        void ctiCallInfoObject_OnErrorReceived(string message)
        {
            lock (count as object)
            {
                count += 1;
                ctiCallInfoObject.AgentForceLogout();
                ctiCallInfoObject.SocketClose();
                
                if (count > 6)
                {
                    //MessageBox.Show(message);
                    Logger.Logger.Log.Info("MediaBarViewModel : " + message);
                    count = 0;
                    if (MessageBox.Show("CTI Engine not Responsed, Would you like to reconnect again?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        isFailed = true;
                        ctiCallInfoObject.ReConnect(count);
                    }
                    else
                    {
                        MessageBox.Show("Please Contact System Administrator!", "Alert", MessageBoxButton.OK, MessageBoxImage.None);
                        ctiCallInfoObject.AgentForceLogout();
                        ctiCallInfoObject.SocketClose();
                    }

                }
                else if (message.Contains("Reconnect"))
                {
                    ctiCallInfoObject.ReConnect(count);
                }
            }
        }

        void ctiCallInfoObject_OnMessageReceived(string message)
        {
            string result = string.Empty;
            result = ParseMessage(message.Substring(1));
            lock (result)
            {
                ProcessMessage(result);
            }
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
                if (str.IndexOf("<MonitorR>") != -1)
                {
                    OnPropertyChanged("IsACD");
                    if (ctiCallInfoObject.IsACD)
                        ctiCallInfoObject.QueryAgentState(InteractionProvider.Credentials.Extension);
                    else
                        ctiCallInfoObject.SnapshotDevice(InteractionProvider.Credentials.Extension);

                    Logger.Logger.Log.Info(string.Format("MonitorR:<<< {0}", str));
                }
                else if (str.IndexOf("<DE>") != -1)
                {
                    OnCallDelivered(str);
                    Logger.Logger.Log.Info(string.Format("DE:<<< {0}", str));
                }
                else if (str.IndexOf("<Est>") != -1)
                {
                    OnCallEstablished(str);

                }
                else if (str.IndexOf("<Dr>") != -1)
                {
                    OnCallEnd(str);
                }
                else if (str.IndexOf("<Con>") != -1)
                {
                    OnCallConferenced(str);
                }
                else if (str.IndexOf("<TE>") != -1)
                {
                    OnCallTransfered(str);
                }
                else if (str.IndexOf("<HE>") != -1)
                {
                    XmlDocument xdoc = new XmlDocument();
                    xdoc.LoadXml(str);
                    XmlNode node = xdoc.SelectSingleNode("HE/hconn/dID/text()");
                    XmlNode callIdnode = xdoc.SelectSingleNode("HE/hconn/cID/text()");

                    if (node.Value == currentExtension)
                    {
                        ChangeUIToHoldState(callIdnode.Value);
                        notifyInteractionStateProperties();
                        EnableComplete();
                    }
                    Logger.Logger.Log.Info(string.Format("HE:<<< {0}", str));
                }
                else if (str.IndexOf("<Re>") != -1)
                {
                    XmlDocument xdoc = new XmlDocument();
                    xdoc.LoadXml(str);
                    XmlNode node = xdoc.SelectSingleNode("Re/rconn/dID/text()");
                    XmlNode callIdnode = xdoc.SelectSingleNode("Re/rconn/cID/text()");

                    if (node.Value == currentExtension)
                    {
                        ChangeUIToRetrieveState(callIdnode.Value);
                        notifyInteractionStateProperties();
                        ResetDropButtonStatus();
                        CurrentInteraction.CanCompleteTransfer = false;
                    }
                    Logger.Logger.Log.Info(string.Format("Re:<<< {0}", str));
                }
                else if (str.IndexOf("<AStateR>") != -1)
                {
                    OnAgentStateProcess();
                    Logger.Logger.Log.Info(string.Format("AStateR:<<< {0}", str));
                }
                else if (str.IndexOf("<ALOEt>") != -1 || str.IndexOf("<ANRE>") != -1 || str.IndexOf("<ARE>") != -1 || str.IndexOf("<AWNRE>") != -1 || str.IndexOf("<AWRE>") != -1 || str.IndexOf("<ALOE>") != -1)
                {
                    if (str.IndexOf("<ALOEt>") != -1)
                    {

                        System.Threading.Tasks.Task.Factory.StartNew(() =>
                        {
                            ChangeUIToAgentLoggedIn();
                        }).ContinueWith((s) =>
                        {
                            ctiCallInfoObject.QueryAgentState(Convert.ToInt32(currentExtension));
                        });

                    }
                    else if (str.IndexOf("<ANRE>") != -1 || str.IndexOf("<ARE>") != -1 || str.IndexOf("<AWNRE>") != -1 || str.IndexOf("<AWRE>") != -1)
                    {
                        _frmAPI = true;
                        if (str.IndexOf("<ANRE>") != -1)
                        {
                            AgentState temp = AgentStates.FirstOrDefault(p => p.SwitchMode == AgentSwitchMode.NotReady);
                            if (CurrentAgentStateRcCode != null && temp.SwitchMode == CurrentAgentStateRcCode.SwitchMode)
                            {
                                temp.Description = CurrentAgentStateRcCode.Description;
                                CurrentAgentState = temp;
                                CurrentAgentStateRcCode = null;
                                TempAgentState = null;
                            }
                            else
                            {
                                CurrentAgentState = temp;
                                CurrentAgentStateRcCode = null;
                                TempAgentState = null;

                            }
                            //ChatAutomationClient.CallSetAgentState("NotReady");
                        }
                        if (str.IndexOf("<ARE>") != -1)
                        {
                            CurrentAgentState = AgentStates.FirstOrDefault(p => p.SwitchMode == AgentSwitchMode.Ready);
                            CurrentAgentStateRcCode = null;
                            TempAgentState = null;
                           //ChatAutomationClient.CallSetAgentState("Ready");
                        }
                        if (str.IndexOf("<AWNRE>") != -1)
                        {
                            CurrentAgentState = AgentStates.FirstOrDefault(p => p.SwitchMode == AgentSwitchMode.WrapUp);
                            CurrentAgentStateRcCode = null;
                            TempAgentState = null;
                            SynchronizationContext.Post(p => AgentStateChange(StandardAgentStates.WrapUp, false), null);
                           //ChatAutomationClient.CallSetAgentState("WrapUp");
                        }
                        
                    }
                    else if (str.IndexOf("<ALOE>") != -1)
                    {
                        ToggleLogOut();
                    }
                    Logger.Logger.Log.Info(string.Format("Agent State:<<< {0}", str));
                }
                else if (str.IndexOf("<UF>") != -1)
                {
                    XmlDocument xdoc = new XmlDocument();
                    xdoc.LoadXml(str);
                    XmlNode UFErrNode = xdoc.SelectSingleNode("UF/Err/text()");
                    var errorCode = UFErrNode.Value;
                    XmlNode UFNode = xdoc.SelectSingleNode("UF/text()");
                    var ufCode = UFNode.Value;
                    XmlNode cInIDNode = xdoc.SelectSingleNode("UF/text()");
                    var clientId = cInIDNode.Value;

                    ProcessFailureMessage(ufCode, errorCode, clientId);
                    Logger.Logger.Log.Info(string.Format("UF:<<< {0}", str));
                }
                else if (str.IndexOf("<ALOE>") != -1)
                {
                    ToggleLogOut();
                    Logger.Logger.Log.Info(string.Format("ALOE:<<< {0}", str));
                }
                else if (str.IndexOf("<QASSR>") != -1)
                {
                    OnQueryAgentStateResponse(str);
                    Logger.Logger.Log.Info(string.Format("QASSR:<<< {0}", str));
                }
                else if (str.IndexOf("<CoCallR>") != -1)
                {
                    ResetDropButtonStatus();
                }
                else if (str.IndexOf("<GLDIR>") != -1)
                {
                    OnLogicalDeviceInfo(str);
                    Logger.Logger.Log.Info(string.Format("GLDIR:<<< {0}", str));
                }
                else if (str.IndexOf("<Div>") != -1)
                {
                    OnCallDiverted(str);
                }
                else if (str.IndexOf("<Fail>") != -1)
                {
                    onCallFailed(str);
                }
                else if (str.IndexOf("<SSDR>") != -1)
                {
                    _frmAPI = true;

                    bool isAgentLoggedIn = IsAlreadyLoggedIn();
                    if (!isAgentLoggedIn)
                    {
                        System.Threading.Tasks.Task.Factory.StartNew(() =>
                        {
                            if (_isImpliclogin)
                            {
                                AESImplictLogin();
                            }
                            else
                            {
                                ToggleLogin(InteractionProvider.Credentials.AgentID.ToString(), InteractionProvider.Credentials.Password, InteractionProvider.Credentials.Extension.ToString(), InteractionProvider.Credentials.Queue.ToString());
                            }
                            _isImpliclogin = false;
                        }).ContinueWith((s) =>
                        {
                            _frmAPI = true;
                            CurrentAgentState = StandardAgentStates.Available;
                            _frmAPI = false;
                        });

                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Error("MediaBarViewModel: ", ex);
            }
        }

        private void onCallFailed(string str)
        {


        }

        private void OnCallDiverted(string str)
        {
            try
            {
                Logger.Logger.Log.Debug("Diverted event received " + str);

                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(str);
                XmlNode callIdNode = xdoc.SelectSingleNode("Div/conn/cID/text()");
                string callid = callIdNode.Value;
                string ani = currentExtension;
                XmlNode IdNode = xdoc.SelectSingleNode("Div/mCRI/text()");
                if (IdNode.Value == ctiCallInfoObject.GetMonitorID().ToString())
                {
                    var call = InteractionProvider.Interactions.FirstOrDefault(p => p != null && p.CallId == callid) as ICall;
                    ctiCallInfoObject.ReSetCallData(callid);
                    if (call != null && (call as IInteraction).State == InteractionState.Ringing)
                    {
                        _autoReset.Reset();
                        call.Accept();
                        _autoReset.WaitOne(1000);
                    }

                    call.HangUp();
                }
                //NextStateChange(CurrentAgentState);
            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Error(ex);
            }
        }

        void OnLogicalDeviceInfo(string str)
        {
            try
            {
                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(str);
                XmlNode sFDInode = xdoc.SelectSingleNode("GLDIR/sFDI/text()");
                int val;
                int.TryParse(sFDInode != null ? sFDInode.Value : "", out val);
                int agentId = val;
                XmlNode hPEnode = xdoc.SelectSingleNode("GLDIR/hPE/text()");
                val = 0;
                int.TryParse(hPEnode.Value, out val);
                int hasAgent = val;

                if ((hasAgent) > 0)
                {
                    if (agentId == InteractionProvider.Credentials.AgentID)
                    {
                        //OutputLog("Agent " + agentId + " is already logged in");
                        //SetAgentState(localStorage.agentId, localStorage.password, localStorage.extension, AgentMode.ReLogin, "", localStorage.queue);
                        ctiCallInfoObject.SetAgentState((int)AgentSwitchMode.LoggedOut, "");
                    }
                    else
                    {
                        MessageBox.Show("Agent " + agentId + " is already logged in this device");
                        //DestroySession();
                    }
                }
                else
                {
                    MessageBox.Show("Login Failed!");
                    //DestroySession();
                }
                Logger.Logger.Log.Info(string.Format("OnLogicalDeviceInfo: {0}", str));
            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Error("OnLogicalDeviceInfo", ex);
                //OutputLog(ex.Message + ex);
            }
        }
       
        
        private void OnAgentStateProcess()
        {

            if (CurrentAgentState.SwitchMode == AgentSwitchMode.HandlingInteraction)
            {

                CurrentAgentState.Description = "Current State: " + OldAgentState.Name;

                if (TempAgentState == null)
                {
                    TempAgentState = AgentStates.FirstOrDefault(p => p.SwitchMode == OldAgentState.SwitchMode && p.Code != "");
                    if (TempAgentState != null && OldAgentState.SwitchMode == AgentSwitchMode.NotReady && OldAgentState.Description != StandardAgentStates.NotReady.Description)
                        TempAgentState.Description = OldAgentState.Description;
                    return;
                }


                if (TempAgentState.SwitchMode == OldAgentState.SwitchMode)
                {
                    CurrentAgentState.Description = string.Format("Current State:{0}", TempAgentState.Name);
                }
                else
                {
                    if (TempAgentState.Name == StandardAgentStates.Available.Name || TempAgentState.Name == StandardAgentStates.WrapUp.Name)
                    {
                        CurrentAgentState.Description = string.Format("Current State: {0} /" + Environment.NewLine + " Next State: {1}", OldAgentState.Name, TempAgentState.Name);
                    }
                    else
                    {
                        if (CurrentAgentStateRcCode != null && TempAgentState.SwitchMode == CurrentAgentStateRcCode.SwitchMode)
                            TempAgentState = CurrentAgentStateRcCode;

                        if (TempAgentState != null && TempAgentState != null && TempAgentState.Description != null)
                            CurrentAgentState.Description = string.Format("Current State: {0} /" + Environment.NewLine + " Next State: {1} ({2})", OldAgentState.Name, TempAgentState.Name, TempAgentState.Description);
                        else
                            CurrentAgentState.Description = string.Format("Current State: {0} /" + Environment.NewLine + " Next State: {1}", OldAgentState.Name, TempAgentState.Name);
                    }
                }
            }
        }

        private void OnCallDelivered(string str)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(str);

            XmlNode node = xdoc.SelectSingleNode("DE/callingD/extn/dID/text()");
            XmlNode node1 = xdoc.SelectSingleNode("DE/calledD/extn/dID/text()");
            XmlNode callIdNode = xdoc.SelectSingleNode("DE/con/cID/text()");
            XmlNode UUINode = xdoc.SelectSingleNode("DE/uD/text()");
            XmlNode Didnode = xdoc.SelectSingleNode("DE/con/dID/text()");
            XmlNode isconsultcalling = xdoc.SelectSingleNode("DE/orgCinfo/callingD/text()");
            XmlNode isconsultcalled = xdoc.SelectSingleNode("DE/orgCinfo/calledD/text()");

            string UUI = UUINode.Value.Split('|')[0];
            string ani = node.Value;
            string dnis = node1.Value;

            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("UUI", UUI);
            CallType calltype = CallType.Inbound;
            

            IInteraction interaction = InteractionProvider.Interactions.FirstOrDefault(p => p.Address == ani && p.CallId == callIdNode.Value);
            MediaType currentType = CISpecification.cIInfoType(interaction);
            if (interaction != null)
            {
                Logger.Logger.Log.Debug("interaction is not null.");
                if (currentType == MediaType.Chat) interaction.Type = MediaType.Chat;
                else if (currentType == MediaType.Incident) interaction.Type = MediaType.Incident;
            }
            else
            {
                Logger.Logger.Log.Debug("interaction is null.");
            }

            var call = CurrentInteraction as ICall;
            
            if (call != null)
            {
                Logger.Logger.Log.Debug("call is not null.");
            }
            else Logger.Logger.Log.Debug("call is null.");


            if (isconsultcalling != null)
            {
                string consultcalling = isconsultcalling != null ? isconsultcalling.Value : "";
                string consultcalled = isconsultcalled != null ? isconsultcalled.Value : "";
                dict.Add("Consult", consultcalling + "|" + consultcalled);
                calltype = CallType.Consult;
                IInteraction cur = InteractionProvider.Interactions.FirstOrDefault(p => p.CallId == callIdNode.Value);
            }
        }

        void OnCallEstablished(string str)
        {
            try
            {
                Logger.Logger.Log.Debug(string.Format("Est:<<< {0}", str));
                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(str);
                XmlNode node = xdoc.SelectSingleNode("Est/callingD/extn/dID/text()");
                XmlNode node1 = xdoc.SelectSingleNode("Est/calledD/extn/dID/text()");
                XmlNode callIdNode = xdoc.SelectSingleNode("Est/econ/cID/text()");
                XmlNode orgincallingdNode = xdoc.SelectSingleNode("Est/orgCinfo/callingD/text()");

                string ani = node.Value;
                string dnis = node1.Value;

                if (currentExtension == ani)
                    ani = dnis;

                if ((!canCompleteConference && !canCompleteTransfer) && orgincallingdNode != null && currentExtension != node1.Value)
                {
                    ani = orgincallingdNode.Value;
                }

                var call = InteractionProvider.Interactions.FirstOrDefault(p => p.Address == ani && p.CallId == callIdNode.Value) as ICall;
                if (call == null)
                {
                    int callId = Convert.ToInt32(callIdNode.Value);
                    ani = ctiCallInfoObject.GetAni(callId);
                }

                IInteraction activecall = InteractionProvider.Interactions.FirstOrDefault(itr => itr.State == InteractionState.Held);

                if (activecall != null)
                {
                    IInteraction currentInteractionVal = InteractionProvider.CurrentInteraction as IInteraction;
                    MediaType curType = CISpecification.cIInfoType(currentInteractionVal);
                    Logger.Logger.Log.Debug(string.Format("ActiveCall State: {0} cIInformation Type: {1} currentInteraction.CallId: {2} activeCall.CallId: {3} ", activecall.State, curType, currentInteractionVal.CallId, activecall.CallId));
                    if ((curType == MediaType.Chat || curType == MediaType.Incident) && activecall.State == InteractionState.Held)
                    {
                        try
                        {
                            //Hold the Phantom call
                            ctiCallInfoObject.OnCallHoldOrRetrieve(true, currentInteractionVal.CallId);

                            //Retrieve the Voice Call
                            ctiCallInfoObject.OnCallHoldOrRetrieve(false, activecall.CallId);
                        }
                        catch (Exception ex)
                        {
                            Logger.Logger.Log.Debug("OnCallEstablished : Exception: " + ex.ToString());
                        }

                    }
                }
                else
                {
                    Logger.Logger.Log.Debug("active call was null.");
                }


                updateUIOnAnswerHangUpCall(false, ani, callIdNode.Value, xmlstr: str);
                SetCallConferenced();
                EnableComplete();
            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Error(string.Format("Est: Error {0}", ex.StackTrace));
                Logger.Logger.Log.Error("Exception", ex);
            }
        }

        private void SetCallConferenced()
        {
            ICall call = CurrentInteraction as ICall;
            bool bCanCompleteTransfer = false;
            foreach (IInteraction interactions in Interactions)
            {
                if (interactions.CanCompleteTransfer)
                {
                    bCanCompleteTransfer = true;
                    break;
                }
            }
        }

        public void ChangeUIToAgentLoggedIn()
        {
            ToggleLogin(InteractionProvider.Credentials.AgentID.ToString(), InteractionProvider.Credentials.Password, InteractionProvider.Credentials.Extension.ToString(), InteractionProvider.Credentials.Queue.ToString());
            Logger.Logger.Log.Info(String.Format("MediaBarViewModel {0}", InteractionProvider));
        }

        void OnCallEnd(string str)
        {
            try
            {
                if (CISpecification.cIInfoType(CurrentInteraction) == MediaType.Incident)
                {
                    Logger.Logger.Log.Debug("Calling RunLastIncident");
                    RunLatestIncident();
                }
                Logger.Logger.Log.Debug(string.Format("Dr:<<< {0}", str));
                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(str);
                XmlNode releasingDeviceId = xdoc.SelectSingleNode("Dr/releaseD/extn/dID/text()");
                string releasingDevice = releasingDeviceId.Value.Trim();

                XmlNode node = xdoc.SelectSingleNode("Dr/drconn/cID/text()");
                string callid = node.Value;
                string ani = ctiCallInfoObject.GetAni(Convert.ToInt32(callid));

                foreach (IInteraction inter in InteractionProvider.Interactions.Where(p => p.CanCompleteTransfer))
                {
                    inter.CanCompleteTransfer = false;
                    inter.ConferencedCall = false;
                    canCompleteTransfer = false;
                    canCompleteConference = false;
                }

                if (releasingDevice == currentExtension)
                {
                    if (CurrentInteraction.isCallFailed)
                    {
                        if (CurrentInteraction.State == InteractionState.Ringing)
                        {
                            _autoReset.Reset();
                            CurrentInteraction.Accept();
                            _autoReset.WaitOne(1000);
                        }

                        (CurrentInteraction as ICall).HangUp();
                        ctiCallInfoObject.ReSetCallData(callid);
                        ResetDropButtonStatus();
                    }
                    else
                    {
                        if (ani != null)
                            ctiCallInfoObject.ReSetCallData(callid);
                        
                        updateUIOnAnswerHangUpCall(true, ani, callid);
                        ResetDropButtonStatus();
                    }
                }
                addedPartiesList.Remove(releasingDevice);
                UpdatePartiesInConference();
                Logger.Logger.Log.Info(string.Format("Successfully Dropped {0}", releasingDevice));
            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Error(string.Format("Drop failed  {0}", ex.StackTrace));
                Logger.Logger.Log.Error("Drop Failed Exception ", ex);
            }
        }

        void ResetDropButtonStatus()
        {
            if (addedPartiesList.Count > 0)
            {
                if (CurrentInteraction.State != InteractionState.Held)
                {
                    EnableDropLastConfParty = true;
                }
            }
            else
            {
                EnableDropLastConfParty = false;
            }
            OnPropertyChanged("EnableDropLastConfParty");
        }

        void OnCallConferenced(string str)
        {
            try
            {
                Logger.Logger.Log.Debug(string.Format("Conferenced :<<< {0}", str));
                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(str);
                XmlNode callIdNode = xdoc.SelectSingleNode("Con/conn/dID/text()");
                XmlNode addedParty = xdoc.SelectSingleNode("Con/aParty/extn/dID/text()");

                XmlNode primaryCallId = xdoc.SelectSingleNode("Con/priOC/cID/text()");
                XmlNode secondaryCallId = xdoc.SelectSingleNode("Con/secOC/cID/text()");
                XmlNode condev = xdoc.SelectSingleNode("Con/conD/extn/dID/text()");

                XmlNode UUINode = xdoc.SelectSingleNode("Con/orgCinfo/ui/text()");

                string confParties = callIdNode.Value.Trim();
                conParties = confParties.Split('|');
                Thread.Sleep(1000);
                if (condev.Value == currentExtension)
                {
                    var priInteraction = InteractionProvider.Interactions.SingleOrDefault(p => p.CallId == primaryCallId.Value);
                    addedPartiesList.Clear();
                    conParties.ToList().ForEach(p => addedPartiesList.Add(p));
                    addedPartiesList.Remove("");
                    addedPartiesList.Remove(currentExtension);
                    addedPartiesList.Remove(addedParty.Value);
                    addedPartiesList.Add(addedParty.Value);
                    UpdatePartiesInConference();
                    ResetDropButtonStatus();
                    IInteraction tempdelete = OnCallCompletedTest(addedParty.Value, primaryCallId.Value, secondaryCallId.Value);
                    priInteraction.CallId = secondaryCallId.Value;
                    foreach (var interaction in InteractionProvider.Interactions)
                    {
                        if (interaction.CallId == secondaryCallId.Value)
                        {
                            interaction.EnableDropLastParty = true;
                        }
                        else
                        {
                            interaction.EnableDropLastParty = false;
                        }
                    }
                    if (tempdelete != null)
                    {
                        (tempdelete as ICall).HangUp();
                        priInteraction = InteractionProvider.Interactions.FirstOrDefault(p => p.CallId == secondaryCallId.Value && p.Id != tempdelete.Id);
                    }
                    else
                    {
                        priInteraction = InteractionProvider.Interactions.FirstOrDefault(p => p.CallId == secondaryCallId.Value);
                    }

                    priInteraction.ConferencedCall = true;
                    priInteraction.CanCompleteTransfer = false;
                    (priInteraction as ICall).CallType = CallType.Consult;
                    if ((priInteraction as ICall).State != InteractionState.Active)
                    {
                        if (priInteraction.State == InteractionState.Ringing)
                        {
                            _autoReset.Reset();
                            (priInteraction as ICall).Accept();
                            (priInteraction as ICall).StartTime = DateTime.Now;
                            _autoReset.WaitOne(1000);
                        }

                        (priInteraction as ICall).Conferenced();
                        OnPropertyChanged("CurrentInteraction");
                    }
                    conferencedCall = priInteraction;
                }
                else
                {
                    var tranfInteraction = InteractionProvider.Interactions.FirstOrDefault(p => p.CallId == primaryCallId.Value);
                    if (tranfInteraction == null)
                        tranfInteraction = InteractionProvider.Interactions.FirstOrDefault(p => p.CallId == secondaryCallId.Value);

                    if (tranfInteraction != null)
                    {
                        tranfInteraction.CallId = secondaryCallId.Value;
                        tranfInteraction.ConferencedCall = true;
                        (tranfInteraction as ICall).ConferencedCall = true;
                        (tranfInteraction as ICall).CallType = CallType.Consult;

                        if ((tranfInteraction as ICall).State == InteractionState.Active)
                            (tranfInteraction as ICall).Conferenced();

                        (tranfInteraction as ICall).RemoveUserData("UUI");
                        (tranfInteraction as ICall).AttachUserData("UUI", UUINode != null ? UUINode.Value : string.Empty);

                        ScreenPopHandler(tranfInteraction);
                        tranfInteraction.BlindCall = InteractionBlindType.Conference;
                    }

                    ctiCallInfoObject.ReSetCallData(primaryCallId.Value);
                    ctiCallInfoObject.SetCallData(Convert.ToInt32(secondaryCallId.Value), condev.Value);

                    if (CurrentInteraction.Address == condev.Value)
                        SetCurrentInteractionData();

                    addedPartiesList.Clear();
                    conParties.ToList().ForEach(p => addedPartiesList.Add(p));
                    addedPartiesList.Remove("");
                    addedPartiesList.Remove(currentExtension);
                    UpdatePartiesInConference();
                }
                Logger.Logger.Log.Info(string.Format("Conferenced Success:<<< {0}", str));
            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Error(string.Format("OnCall conferenced failed {0}", ex.StackTrace));
                Logger.Logger.Log.Error("OnCall conferenced failed", ex);
            }
        }


        void OnCallTransfered(string str)
        {
            try
            {
                Logger.Logger.Log.Debug(string.Format("TE:<<< {0}", str));
                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(str);
                XmlNode node = xdoc.SelectSingleNode("TE/transferringDe/extn/dID/text()");
                XmlNode node1 = xdoc.SelectSingleNode("TE/transferredDe/extn/dID/text()");
                XmlNode TEnode = xdoc.SelectSingleNode("TE/conn/cLI/text()");
                XmlNode Didnode = xdoc.SelectSingleNode("TE/priOC/dID/text()");
                XmlNode orgincallingnode = xdoc.SelectSingleNode("TE/orgCinfo/callingD/text()");
                XmlNode orgincallednode = xdoc.SelectSingleNode("TE/orgCinfo/calledD/text()");
                XmlNode pricall = xdoc.SelectSingleNode("TE/priOC/cID/text()");
                XmlNode seccall = xdoc.SelectSingleNode("TE/secOC/cID/text()");
                XmlNode UUINode = xdoc.SelectSingleNode("TE/orgCinfo/ui/text()");
                Thread.Sleep(1000);
                if (Didnode.Value == currentExtension)
                {
                    string[] list = TEnode.Value.Split('|');
                    List<string> exten = new List<string>(list);
                    List<string> exten1 = new List<string>(list);
                    foreach (string ext in exten1)
                    {
                        exten.Add(ctiCallInfoObject.GetExtensionAniData(ext));
                    }
                    exten = exten.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
                    var call = InteractionProvider.Interactions.FirstOrDefault(p => (p.CallId == pricall.Value)) as ICall;
                    if (call != null)
                    {
                        _autoReset.Reset();
                        call.HangUp();
                        _autoReset.WaitOne(1000);
                        ctiCallInfoObject.ReSetCallData(call.CallId);
                    }
                    call = InteractionProvider.Interactions.FirstOrDefault(p => (p.CallId == seccall.Value)) as ICall;
                    if (call != null)
                    {
                        if (call.State == InteractionState.Ringing)
                        {
                            _autoReset.Reset();
                            call.Accept();
                            _autoReset.WaitOne(1000);
                        }

                        _autoReset.Reset();
                        call.HangUp();
                        _autoReset.WaitOne(1000);
                        ctiCallInfoObject.ReSetCallData(call.CallId);
                    }

                    if (ctiCallInfoObject.GetCallDataCount() == 0)
                    {
                        addedPartiesList.Clear();
                        UpdatePartiesInConference();
                    }
                    var inter = InteractionProvider.Interactions.FirstOrDefault(p => p.CallId != pricall.Value && p.CallId != seccall.Value);
                    if (inter != null)
                    {
                        inter.CanCompleteTransfer = false;
                        inter.ConferencedCall = false;
                        if (InteractionProvider.Interactions.Count() == 1)
                        {
                            CurrentInteraction.CanCompleteTransfer = false;
                        }
                        OnPropertyChanged("CurrentInteraction");
                    }
                }
                else
                {
                    var tranfInteraction = InteractionProvider.Interactions.FirstOrDefault(p => p.CallId == pricall.Value);
                    if (tranfInteraction == null)
                        tranfInteraction = InteractionProvider.Interactions.FirstOrDefault(p => p.CallId == seccall.Value);

                    if (tranfInteraction != null)
                    {
                        tranfInteraction.CallId = seccall.Value;
                        (tranfInteraction as ICall).RemoveUserData("UUI");
                        (tranfInteraction as ICall).AttachUserData("UUI", UUINode != null ? UUINode.Value : string.Empty);

                        ScreenPopHandler(tranfInteraction);
                        tranfInteraction.BlindCall = InteractionBlindType.Transfere;
                    }

                    ctiCallInfoObject.ReSetCallData(pricall.Value);
                    ctiCallInfoObject.SetCallData(Convert.ToInt32(seccall.Value), Didnode.Value);

                    if (CurrentInteraction.Address == Didnode.Value)
                        SetCurrentInteractionData();

                }
                Logger.Logger.Log.Info(string.Format("TE success:<<< {0}", str));
            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Error(string.Format("Transfered failed stack trace {0}", ex.StackTrace));
                Logger.Logger.Log.Error("Transfered failed", ex);

            }
        }

        void UpdatePartiesInConference()
        {
            _conferenceParties = string.Empty;

            foreach (string party in addedPartiesList)
            {
                if (party.Trim() != "" && party != currentExtension)
                {
                    if (_conferenceParties != string.Empty)
                    {
                        _conferenceParties = _conferenceParties + "," + party.Trim();
                    }
                    else
                    {
                        _conferenceParties = party.Trim();
                    }
                }
            }

            if (_conferenceParties != string.Empty)
            {
                _conferenceParties = Resources.PartiesConference + _conferenceParties;
            }
            OnPropertyChanged("ConferencePartiesList");
        }

        void ProcessFailureMessage(string ufCode, string errorCode, string clientId)
        {
            if (ufCode == "10")
            {
                if (errorCode == "1")
                {
                    ctiCallInfoObject.GetLogicalDeviceInfo();
                }
                else if (errorCode == "0")
                {
                    MessageBox.Show("Invalid password. Please try again!");
                }
                else if (errorCode == "12")
                {
                    MessageBox.Show("Invalid login credentials. Please try again!");
                }
                else if (errorCode == "22")
                {
                    //MessageBox.Show("Invalid password. Please try again!");
                }
                else if (errorCode == "33")
                {
                    MessageBox.Show("Device is busy");
                }
                else if (errorCode == "63")
                {
                    MessageBox.Show("PAC violated");
                }

            }
            else if (ufCode == "11" || ufCode == "13")
            {
                //OnLinkDown();
            }
            else if (ufCode == "8")
            {
                MessageBox.Show("Invalid parameters");
            }
        }

        private void ChangeToAgentLoggedIn()
        {
            ToggleLogin(currentAgentId, currentAgentPassword, currentExtension, currentQueue);
            ctiCallInfoObject.StartMonitor(currentExtension);
        }

        public string AgentName
        {
            get
            {
                if (InteractionProvider.Agent != null)
                {
                    return InteractionProvider.Agent.Name;
                }

                return string.Empty;
            }
        }

        public int CallCount
        {
            get
            {
                return _callCount;
            }
            set
            {
                if (_callCount != value)
                {
                    _callCount = value;
                    OnPropertyChanged("CallCount");
                }
            }
        }
        // Added By Saroj For Enhanced Toolbar UI
        public bool IsEnhancedUI
        {
            get
            {
                if (InteractionManager.AgentScreenPop.IsEnhanced)
                {
                    return InteractionManager.AgentScreenPop.IsEnhanced;
                }

                return false;
            }
            set
            {
                if (_IsEnhancedUI != value)
                {
                    _IsEnhancedUI = false;
                    OnPropertyChanged("IsEnhancedUI");
                }
            }
        }

        public bool IsNotEnhanced
        {
            get
            {
                if (!InteractionManager.AgentScreenPop.IsEnhanced)
                {
                    return true;
                }
                return false;
            }
            set
            {
                if (_isNotEnhanced != value)
                {
                    _isNotEnhanced = false;
                    OnPropertyChanged("IsEnhanced");
                }
            }
        }

        // Added By Saroj For Enhanced Toolbar UI

        public bool HasInteractions
        {
            get
            {
                return _hasInteractions;
            }
            set
            {
                if (_hasInteractions != value)
                {
                    _hasInteractions = value;
                    OnPropertyChanged("HasInteractions");
                }
            }
        }

        public int EmailCount
        {
            get
            {
                return _emailCount;
            }
            set
            {
                if (_emailCount != value)
                {
                    _emailCount = value;
                    OnPropertyChanged("EmailCount");
                }
            }
        }

        public string Extension
        {
            get
            {
                if (InteractionProvider.Device != null)
                {
                    return InteractionProvider.Device.Address;
                }

                return string.Empty;
            }
        }

        public bool CanChangeState
        {
            get
            {
                return _canChangeState;
            }
            set
            {
                _canChangeState = value;
                OnPropertyChanged("CanChangeState");
            }
        }

        public bool CanChangeConnectionState
        {
            get
            {
                return _canChangeConnectionState;
            }
            set
            {
                _canChangeConnectionState = value;
                OnPropertyChanged("CanChangeConnectionState");
            }
        }

        public bool IsACD
        {
            get
            {
                return ctiCallInfoObject.IsACD;

            }
        }

        public string InteractionTime
        {
            get
            {
                if (CurrentInteraction != null)
                {
                    return (CurrentInteraction.Duration).ToString(@"hh\:mm\:ss");
                }

                return string.Empty;
            }
        }

        public string AnswerHangupImage
        {
            get
            {
                if (CurrentInteraction != null) Logger.Logger.Log.Debug("CurrentInteraction State: " + CurrentInteraction.State.ToString());
                var call = CurrentInteraction as ICall;
                string imageUri = "";
                MediaType currentMediaType = CISpecification.cIInfoType(CurrentInteraction);
                if (CurrentInteraction != null && (call.CallType == CallType.Inbound || call.CallType == CallType.Consult))
                {
                    Logger.Logger.Log.Debug("CurrentInteraction State:" + call.Type.ToString());
                    if (CurrentInteraction.State == InteractionState.Ringing)
                    {
                        if (currentMediaType == MediaType.Chat) imageUri = Resources.ChatImageUri;
                        else if (currentMediaType == MediaType.Incident)
                        {
                            imageUri = Resources.IncidentImageUri;
                        }
                        else if (currentMediaType == MediaType.Voice) imageUri = Resources.AnswerImageUri;
                        else
                        {
                            imageUri = Resources.HangupImageUri;

                        }
                    }
                    else if (CurrentInteraction.State != InteractionState.Ringing && (currentMediaType != MediaType.Chat && currentMediaType != MediaType.Incident))
                    {
                        imageUri = Resources.HangupImageUri;
                    }
                }
                else if (CurrentInteraction != null && call.CallType == CallType.Outbound)
                {
                    imageUri = Resources.HangupImageUri;
                }
                return imageUri;
            }
        }

        public string AnswerHangupTooltip
        {
            get
            {
                var call = CurrentInteraction as ICall;
                string imageTT = "";
                MediaType currentMediaType = CISpecification.cIInfoType(CurrentInteraction);
                if (CurrentInteraction != null && (call.CallType == CallType.Inbound || call.CallType == CallType.Consult))
                {
                    Logger.Logger.Log.Debug("CurrentInteraction State:" + call.State.ToString());
                    if (CurrentInteraction.State == InteractionState.Ringing)
                    {
                        if (currentMediaType == MediaType.Chat) imageTT = Resources.ChatTooltip;
                        else if (currentMediaType == MediaType.Incident) imageTT = Resources.IncidentTooltip;
                        else if (currentMediaType == MediaType.Voice) imageTT = Resources.AnswerTooltip;
                        else
                        {
                            imageTT = Resources.HangupTooltip;
                        }
                    }
                    else if (CurrentInteraction.State != InteractionState.Ringing && (currentMediaType != MediaType.Chat && currentMediaType != MediaType.Incident))
                    {
                        imageTT = Resources.HangupTooltip;
                    }
                }
                else if (CurrentInteraction != null && call.CallType == CallType.Outbound)
                {
                    imageTT = Resources.HangupTooltip;
                }
                return imageTT;
            }
        }

        public string HoldRetrieveImage
        {
            get
            {
                return (CurrentInteraction != null && CurrentInteraction.State == InteractionState.Held)
                       ? Resources.RetrieveImageUri
                       : Resources.HoldImageUri;
            }
        }

        public string HoldRetrieveTooltip
        {
            get
            {
                return (CurrentInteraction != null && CurrentInteraction.State == InteractionState.Held)
                       ? Resources.RetriveTooltip
                       : Resources.HoldTooltip;
            }
        }

        public string TransferImage
        {
            get
            {
                var call = CurrentInteraction as ICall;
                return (CurrentInteraction != null && CurrentInteraction.CanCompleteTransfer == false)
                       ? Resources.TransferImageUri
                       : Resources.CompleteTransferImageUri;
            }
        }

        public string BlindTransferImage
        {
            get
            {
                return (CurrentInteraction != null && CurrentInteraction.CanCompleteTransfer == false)
                    ? Resources.BlindTransferImageUri
                    : Resources.CompleteTransferImageUri;
            }
        }

        public string BlindConferenceImage
        {
            get
            {
                return (CurrentInteraction != null && CurrentInteraction.CanCompleteTransfer == false)
                    ? Resources.BlindConferenceImageUri
                    : Resources.CompleteConferenceImageUri;
            }
        }

        public string TransferTooltip
        {
            get
            {
                return Resources.TransferToolTip;

            }
        }

        public string ConsultTransferTooltip
        {
            get
            {
                var call = CurrentInteraction as ICall;
                return (CurrentInteraction != null && CurrentInteraction.CanCompleteTransfer == false)
                       ? Resources.ConsultTransferToolTip
                       : Resources.CompleteTransferToolTip;
            }
        }

        public string ConferenceImage
        {
            get
            {
                var call = CurrentInteraction as ICall;
                return (CurrentInteraction != null && CurrentInteraction.CanCompleteTransfer == false)
                       ? Resources.ConferenceImageUri
                       : Resources.CompleteConferenceImageUri;
            }
        }

        public string ConferenceTooltip
        {
            get
            {
                return Resources.ConferenceToolTip;
            }
        }

        public string ConsultConferenceTooltip
        {
            get
            {
                var call = CurrentInteraction as ICall;
                return (CurrentInteraction != null && CurrentInteraction.CanCompleteTransfer == false)
                       ? Resources.ConsultConferenceToolTip
                       : Resources.CompleteConferenceToolTip;
            }
        }

        public string ConnectionTooltip
        {
            get
            {
                return (IsAgentLoggedIn == false)
                       ? Resources.AgentLogin
                       : Resources.AgentLogout;
            }
        }

        public bool IsAgentLoggedIn
        {
            get
            {
                return CurrentAgentState != null && CurrentAgentState.SwitchMode != AgentSwitchMode.LoggedOut;
            }
        }

        public string ConferencePartiesList
        {
            get
            {
                return _conferenceParties;
            }
            set
            {
                _conferenceParties = value;
            }
        }

        [Import]
        public IContactProvider ContactProvider { get; set; }

        #region Commands

        public ICommand LoginToggleCommand { get; set; }

        public ICommand AnswerHangUpCallCommand { get; set; }

        public ICommand HoldRetrieveCallCommand { get; set; }

        public ICommand CompleteInteractionCommand { get; set; }

        public ICommand ShowTransferDialogCommand { get; set; }

        public ICommand ShowDialOptionsCommand { get; set; }

        public ICommand DialCommand { get; set; }

        public ICommand ShowDialPadComand { get; set; }

        public ICommand ShowAgentLoginCommand { get; set; }

        public ICommand InitiateConferenceCommand { get; set; }

        public ICommand DropLastPartyFromConferenceCommand { get; set; }


        #endregion Commands

        public int WebIncidentCount
        {
            get
            {
                return _webIncidentCount;
            }
            set
            {
                if (_webIncidentCount != value)
                {
                    _webIncidentCount = value;
                    OnPropertyChanged("WebIncidentCount");
                }
            }
        }

        public bool CanCompleteInteraction
        {
            get
            {
                return _canCompleteInteraction;
            }
            set
            {
                if (_canCompleteInteraction != value)
                {
                    _canCompleteInteraction = value;
                    OnPropertyChanged("CanCompleteInteraction");
                }
            }
        }

        public bool ShowDialOptions
        {
            get
            {
                return _showDialOptions;
            }
            set
            {

                if (_showDialOptions != value)
                {
                    _showDialOptions = value;
                    OnPropertyChanged("ShowDialOptions");
                }
            }
        }

        public override void OnImportsSatisfied()
        {
            base.OnImportsSatisfied();
            InteractionProvider.Agent.StateChanged += agentManagerStateChanged;
            InteractionProvider.Agent.NameChanged += agentNameChanged;
            InteractionProvider.Device.AddressChanged += deviceAddressChanged;
            ToastManager.Initialize(InteractionProvider);
        }

        private void InteractionProvider_ScreenPopEvent(object sender, InteractionEventArgs e)
        {
        }

        protected override void InteractionConnectedHandler(object sender, InteractionEventArgs e)
        {
            base.InteractionConnectedHandler(sender, e);
            if (e.Interaction.IsRealTime)
                CanCompleteInteraction = false;
        }

        protected override void NewInteractionHandler(object sender, InteractionEventArgs e)
        {
            lock (_AsynMVM)
            {

                base.NewInteractionHandler(sender, e);
                if (!_timer.IsEnabled)
                {
                    _timer.Interval = TimeSpan.FromSeconds(1);
                    _timer.Start();
                }
                Logger.Logger.Log.Info(string.Format("MediaBarViewModel 1>> {0}", e.Interaction));
                //InteractionState state = cIInformation(e.Interaction);
                //Logger.Logger.Log.Info(string.Format("Current interaction state: {0} ", state));
                //if (state != InteractionState.Chat && state != InteractionState.Incident)
                //{
                updateInteractionCount(new InteractionCountUpdateInfo
                {
                    MediaType = e.Interaction.Type,
                    UpdateType = UpdateType.Add
                });
                //}
                Logger.Logger.Log.Info(string.Format("MediaBarViewModel 2>> {0}", e.Interaction));
                e.Interaction.StateChanged += interactionStateChangedHandler;
                CanChangeConnectionState = false;
                HasInteractions = true;
                OnPropertyChanged("Interactions");
                Logger.Logger.Log.Info(string.Format("MediaBarViewModel 3>> {0}", e.Interaction));
            }
        }

        private void interactionStateChangedHandler(object sender, InteractionStateChangedEventArgs e)
        {
            ChangeUIToRetrieveStateSyn();
            notifyInteractionStateProperties();
            _autoReset.Set();
        }

        private void notifyInteractionStateProperties()
        {
            OnPropertyChanged("HoldRetrieveImage");
            OnPropertyChanged("HoldRetrieveTooltip");
            OnPropertyChanged("AnswerHangupImage");
            OnPropertyChanged("AnswerHangupTooltip");
            OnPropertyChanged("TransferImage");
            OnPropertyChanged("ConferenceImage");
            OnPropertyChanged("BlindTransferImage");
            OnPropertyChanged("BlindConferenceImage");
            OnPropertyChanged("TransferTooltip");
            OnPropertyChanged("ConferenceTooltip");
            OnPropertyChanged("ConnectionTooltip");
            OnPropertyChanged("ConsultTransferTooltip");
            OnPropertyChanged("ConsultConferenceTooltip");
            OnPropertyChanged("Interactions");
            Logger.Logger.Log.Info("notifyInteractionStateProperties");
        }

        protected override void CurrentInteractionChangedHandler(object sender, InteractionEventArgs e)
        {
            EnableComplete();
            base.CurrentInteractionChangedHandler(sender, e);
            notifyInteractionStateProperties();
            SetCurrentInteractionData();
            if (CurrentInteraction != null && (!CurrentInteraction.IsRealTime || CurrentInteraction.State == InteractionState.Disconnected))
            {
                CanCompleteInteraction = true;
            }
            else
            {
                CanCompleteInteraction = false;
            }
            Logger.Logger.Log.Info(string.Format("MediaBarViewModel: {0}", e.Interaction));
        }

        private void EnableComplete()
        {
            IEnumerable<IInteraction> list = InteractionProvider.Interactions.Where(p => p.CanCompleteTransfer == false);
            Logger.Logger.Log.Info(string.Format("Interaction list Count: ", list.ToList().Count()));
            if (list != null && list.ToList().Count() > 1)
            {
                IInteraction comheld = list.FirstOrDefault(p => p.State == InteractionState.Held);
                IInteraction nonheld = list.FirstOrDefault(p => p.State != InteractionState.Held);
                if (comheld != null)
                {
                    Logger.Logger.Log.Info(string.Format("Held Interaction CallID: ", comheld.CallId));
                    MediaType curType = CISpecification.cIInfoType(nonheld);
                    if (curType != MediaType.Chat && curType != MediaType.Incident)
                    {
                        comheld.CanCompleteTransfer = true;
                        (comheld as ICall).CallType = CallType.Consult;
                        IInteraction com = list.FirstOrDefault(p => p.CallId != comheld.CallId);
                    }
                }
                if (nonheld != null)
                {
                    Logger.Logger.Log.Info(string.Format("Non- Held Interaction CallID: ", nonheld.CallId));
                }
            }
        }

        private bool canConsult(string callid)
        {
            IEnumerable<IInteraction> list = InteractionProvider.Interactions.Where(p => p.CallId != callid);
            if (list != null && list.ToList().Count() >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        private void SetCurrentInteractionData()
        {
            try
            {
                if (CurrentInteraction != null)
                    ctiCallInfoObject.SetActiveCall(CurrentInteraction.CallId);
                else
                {
                    Logger.Logger.Log.Debug("currenInteraction was null.");
                }
            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Error("MediaBarViewModel: ", ex);
            }
        }

        protected override void InteractionDisconnectedHandler(object sender, Oracle.RightNow.Cti.InteractionEventArgs e)
        {
            base.InteractionDisconnectedHandler(sender, e);
            CanCompleteInteraction = true;
            _reset.Set();
            Logger.Logger.Log.Info(string.Format("MediaBarViewModel: {0}", e.Interaction));
        }

        protected override void InteractionCompletedHandler(object sender, Oracle.RightNow.Cti.InteractionEventArgs e)
        {
            try
            {
                base.InteractionCompletedHandler(sender, e);
                CanCompleteInteraction = false;
                _reset.Set();
                updateInteractionCount(new InteractionCountUpdateInfo
                {
                    MediaType = e.Interaction.Type,
                    UpdateType = UpdateType.Subtract
                });

                HasInteractions = InteractionProvider.Interactions.Count != 0;
                CanChangeConnectionState = !HasInteractions;
                AgentStates.FirstOrDefault(h => h.SwitchMode == AgentSwitchMode.HandlingInteraction).Description = StandardAgentStates.InCall.Description;
                if (!HasInteractions)
                {
                    NextStateChange(CurrentAgentState);
                }

                Logger.Logger.Log.Info(string.Format("MediaBarViewModel: {0}", e.Interaction));
            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Error("MediabarViewModel", ex);
            }
        }

        private void agentManagerStateChanged(object sender, AgentStateChangedEventArgs e)
        {
            OnPropertyChanged("IsAgentLoggedIn");
            OnPropertyChanged("ConnectionTooltip");
            OldAgentState = e.OldState;
            CanChangeState = e.NewState.SwitchMode != AgentSwitchMode.LoggedOut;
            if (e.NewState.SwitchMode == AgentSwitchMode.HandlingInteraction && InteractionProvider.Credentials.AgentID != 0)
                OnAgentStateProcess();

            Logger.Logger.Log.Info(string.Format("MediaBarViewModel: {0}", e.NewState));
        }

        private void agentNameChanged(object sender, EventArgs e)
        {
        }

        private void deviceAddressChanged(object sender, ExtensionChangedEventArgs e)
        {
            OnPropertyChanged("Extension");
        }

        private void initializeCommands()
        {
            AnswerHangUpCallCommand = new DelegateCommand(o => answerHangUpCall());
            //ChatCallCommand = new DelegateCommand(o => answerHangUpCall());
            //IncidentCallCommand = new DelegateCommand(o => answerHangUpCall());
            HoldRetrieveCallCommand = new DelegateCommand(o => HoldOrRetrieveCall());
            CompleteInteractionCommand = new DelegateCommand(o => CompleteInteraction());
            ShowTransferDialogCommand = new DelegateCommand(showTransferDialog);
            ShowDialOptionsCommand = new DelegateCommand(o => showDialOptions());
            DialCommand = new DelegateCommand(o => Dial(o as Contact));
            ShowDialPadComand = new DelegateCommand(showDialPad);
            ShowAgentLoginCommand = new DelegateCommand(showAgentLoginDialog);
            InitiateConferenceCommand = new DelegateCommand(showConferenceDialog);
            DropLastPartyFromConferenceCommand = new DelegateCommand(dropLastPary);
            ScreenPopHandlers = new List<IScreenPopHandler>();
            
        }

        private void answerHangUpCall()
        {
            try
            {
                var call = InteractionProvider.CurrentInteraction as ICall;
                if (call != null)
                {
                    if (call.CallType == CallType.Outbound)
                    {
                        if (call.State == InteractionState.Active)
                        {
                            ResetCanCompleteStatus();
                            ctiCallInfoObject.OnCallDropped(call.CallId);
                            Logger.Logger.Log.Info(string.Format("MediaBarViewModel: call Dropped{0}", call.CallType.ToString()));
                            return;
                        }
                        else
                        {
                            // Need to drop outbound call. 
                            // We cannot drop outbound call without accepting.
                            // So accept the call and then drop
                            call.Accept();
                            ctiCallInfoObject.OnCallDropped(call.CallId);
                            Logger.Logger.Log.Info(string.Format("MediaBarViewModel: call Accept & Dropped{0}", call.CallType.ToString()));

                            ResetCanCompleteStatus();
                            return;
                        }
                    }
                    else if (call.CallType == CallType.Inbound || call.CallType == CallType.Consult)
                    {
                        if (call.State == InteractionState.Ringing)
                        {
                            IInteraction activecalllist = InteractionProvider.Interactions.FirstOrDefault(itr => itr.State == InteractionState.Active || itr.State == InteractionState.Conferenced || itr.State == InteractionState.Consulting);
                            if (activecalllist != null)
                                ctiCallInfoObject.OnHoldCall(Convert.ToInt32(activecalllist.CallId));

                            ctiCallInfoObject.OnCallAnswered(call.CallId);

                            Logger.Logger.Log.Info(string.Format("MediaBarViewModel: call Answerd{0}", call.CallType.ToString()));
                        }
                        else
                        {
                            ctiCallInfoObject.OnCallDropped(call.CallId);
                            Logger.Logger.Log.Info(string.Format("MediaBarViewModel: call callDropped {0}", call.CallType.ToString()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Error("MediabarViewModel: AnsweHangUp:", ex);
            }
        }

        private void updateUIOnAnswerHangUpCall(bool callDropped, string ani = "", string callid = "0", string xmlstr = "")
        {
            try
            {
                Logger.Logger.Log.Debug(string.Format("Call dropped {0} Ani {1} count{2}", callDropped, ani, InteractionProvider.Interactions.Count()));
                var call = InteractionProvider.Interactions.FirstOrDefault(p => p != null && p.Address == ani && p.CallId == callid) as ICall;

                if (call == null)
                {
                    string exten = string.Empty;
                    call = InteractionProvider.Interactions.FirstOrDefault(p => p != null && p.Id == conferencedCall.Id) as ICall;
                    ctiCallInfoObject.ReSetExtensionAniData(ani);
                }

                if (call != null)
                {
                    if (!callDropped)
                    {
                        // Call answered and got call established message from CTI.
                        // Update UI accordingly    
                        XmlDocument xdoc = new XmlDocument();
                        xdoc.LoadXml(xmlstr);

                        XmlNode node = xdoc.SelectSingleNode("Est/callingD/extn/dID/text()");
                        XmlNode node1 = xdoc.SelectSingleNode("Est/calledD/extn/dID/text()");
                        XmlNode callIdNode = xdoc.SelectSingleNode("Est/econ/cID/text()");
                        XmlNode isconsultcalling = xdoc.SelectSingleNode("Est/orgCinfo/callingD/text()");
                        XmlNode isconsultcalled = xdoc.SelectSingleNode("Est/orgCinfo/calledD/text()");
                        XmlNode userUUI = xdoc.SelectSingleNode("Est/userinfo/text()");
                        XmlNode userUUI2 = xdoc.SelectSingleNode("Est/orgCinfo/ui/text()");
                        string UUIvalue = (userUUI != null) ? userUUI.Value : (userUUI2 != null) ? userUUI2.Value : string.Empty;

                        if (node.Value == ctiCallInfoObject.CurrentStation)
                        {

                            if (isconsultcalling != null || canConsult(callid))
                            {
                                string consultcalling = isconsultcalling != null ? isconsultcalling.Value : "";
                                string consultcalled = isconsultcalled != null ? isconsultcalled.Value : "";

                                call.CallType = CallType.Consult;
                                _autoReset.Reset();
                                call.Accept();
                                _autoReset.WaitOne(1000);
                                if ((call as IInteraction).ConferencedCall)
                                {
                                    call.Conferenced();
                                    OnPropertyChanged("CurrentInteraction");
                                }
                                else
                                {
                                    call.Consulting();
                                    OnPropertyChanged("CurrentInteraction");
                                }
                            }
                            else
                            {
                                call.Accept();
                            }
                        }
                        else
                        {
                            _autoReset.Reset();
                            call.Accept();
                            _autoReset.WaitOne(1000);
                            if (isconsultcalling != null || canConsult(callid))
                            {
                                IInteraction temp = call;
                                if (temp.ConferencedCall)
                                {
                                    call.Conferenced();
                                    OnPropertyChanged("CurrentInteraction");
                                }
                                else
                                {
                                    call.Consulting();
                                    OnPropertyChanged("CurrentInteraction");
                                }
                            }
                        }
                        call.StartTime = DateTime.Now;
                        Logger.Logger.Log.Info(string.Format("MediaBarViewModel: call Accept {0}", call.CallType.ToString()));
                    }
                    else
                    {
                        // Call hanged up and got call end message from CTI
                        // Update UI accordingly

                        var cal = InteractionProvider.Interactions.FirstOrDefault(p => p != null && p.CallId == callid) as ICall;

                        if (cal == null)
                            cal = InteractionProvider.Interactions.FirstOrDefault(p => p != null && p.Address == ani && p.CallId == callid) as ICall;

                        if (cal == null && conferencedCall != null)
                        {
                            cal = InteractionProvider.Interactions.FirstOrDefault(p => p != null && p.Id == conferencedCall.Id) as ICall;
                            conferencedCall = null;
                        }
                        if (call != null && cal.CallType == CallType.Outbound)
                        {
                            ResetCanCompleteStatus();
                        }
                        if (call != null && (cal as IInteraction).State == InteractionState.Ringing)
                        {
                            _autoReset.Reset();
                            call.Accept();
                            _autoReset.WaitOne(1000);
                        }

                        if (call == null)
                        {
                            //UI yet to progress.
                            Logger.Logger.Log.Info(string.Format("MediaBarViewModel: Interaction is failed {0} ", callDropped));
                            return;
                        }

                        if (call.ConferencedCall)
                            addedPartiesList.Clear();

                        _autoReset.Reset();
                        call.HangUp();
                        _autoReset.WaitOne(1000);


                        //NextStateChange(CurrentAgentState);
                        Logger.Logger.Log.Info(string.Format("MediaBarViewModel: call HangUp {0}", call.CallType.ToString()));
                    }
                }
                Logger.Logger.Log.Info(string.Format("UpdateUI success on Established {0}", ani));
            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Error(string.Format("UpdateUI failed on Established {0}", ex.StackTrace));
                Logger.Logger.Log.Error("UpdateUI failed on Established", ex);
            }
            finally
            {
                _autoReset.Set();
            }
        }


        private void ResetCanCompleteStatus()
        {
            foreach (IInteraction interaction in InteractionProvider.Interactions)
            {
                var call = CurrentInteraction as ICall;
                var itcall = interaction as ICall;
                if (itcall != null)
                {
                    itcall.CanCompleteTransfer = false;
                    canCompleteTransfer = false;
                    canCompleteConference = false;
                }
            }
        }

        private void HoldOrRetrieveCall()
        {
            if (CurrentInteraction.State == InteractionState.Active || CurrentInteraction.State == InteractionState.Conferenced || CurrentInteraction.State == InteractionState.Consulting)
            {
                ctiCallInfoObject.OnCallHoldOrRetrieve(true, CurrentInteraction.CallId);
            }
            else if (CurrentInteraction.State == InteractionState.Held)
            {
                ctiCallInfoObject.OnCallHoldOrRetrieve(false, CurrentInteraction.CallId);
            }

            Logger.Logger.Log.Info(string.Format("MediaBarViewModel: call HangUp {0}", CurrentInteraction.State.ToString()));
        }

        public void ChangeUIToTransferedState()
        {
            foreach (IInteraction interaction in InteractionProvider.Interactions)
            {
                var call = interaction as ICall;
                if (call != null)
                {
                    call.HangUp();
                }
            }
            ChanageUIToDisconnected();

        }

        public void ChangeUIToOutBoundRinging()
        {
            ChanageToOutboundRinging();
        }

        public void AgentStateChange(AgentState state, bool canSendCti = true)
        {
            try
            {
                Logger.Logger.Log.Debug("Agent State:" + state);
                _frmAPI = false;
                int code;
                int.TryParse(state.Code, out code);
                string reason = "";

                ScreenPopConfig conf = InteractionManager.AgentScreenPop;
              
                
                if (!_fromlogin && ((state.SwitchMode == AgentSwitchMode.NotReady || state.SwitchMode == AgentSwitchMode.NewReason) && conf.AUXReasonEnabled) || (state.SwitchMode == AgentSwitchMode.NewReason && conf.AUXReasonEnabled) || (state.SwitchMode == AgentSwitchMode.WrapUp && conf.WrapupReasonEnabled) || (state.SwitchMode == AgentSwitchMode.LoggedOut && conf.LogoutReasonEnabled))
                {
                    List<AgentState> va = new List<AgentState>();
                    List<ScreenPopOptions> opts = new List<ScreenPopOptions>();
                    int agentstate = 0;
                    if (state.SwitchMode == AgentSwitchMode.NotReady || state.SwitchMode == AgentSwitchMode.NewReason)
                    {
                        int.TryParse(state.Code, out code);
                        agentstate = code;
                        opts = conf.ScreenPopOptionsList.Where(p => p.Type == (int)ReasonCodes.AUXReason).ToList();
                        reason = "Not Ready";
                        TempAgentState = StandardAgentStates.NotReady;
                        AgentSwitchMode temp = state.SwitchMode == AgentSwitchMode.NotReady ? AgentSwitchMode.NotReady : AgentSwitchMode.NewReason;
                        opts.ForEach((p) => va.Add(new AgentState() { Name = StandardAgentStates.NotReady.Name, Code = p.Name, SwitchMode = AgentSwitchMode.NotReady, Description = p.Description }));
                    }
                    else if (state.SwitchMode == AgentSwitchMode.WrapUp)
                    {
                        int.TryParse(state.Code, out code);
                        agentstate = code;
                        opts = conf.ScreenPopOptionsList.Where(p => p.Type == (int)ReasonCodes.WrapupReason).ToList();
                        reason = "WrapUp";
                        TempAgentState = StandardAgentStates.WrapUp;
                        opts.ForEach((p) => va.Add(new AgentState() { Name = StandardAgentStates.WrapUp.Name, Code = p.Name, SwitchMode = AgentSwitchMode.WrapUp, Description = p.Description }));
                    }
                    else if (state.SwitchMode == AgentSwitchMode.LoggedOut)
                    {
                        int.TryParse(state.Code, out code);
                        agentstate = code;
                        opts = conf.ScreenPopOptionsList.Where(p => p.Type == (int)ReasonCodes.LogoutReason).ToList();
                        reason = "Logout";
                        TempAgentState = StandardAgentStates.LoggedOut;
                        opts.ForEach((p) => va.Add(new AgentState() { Name = StandardAgentStates.LoggedOut.Name, Code = p.Name, SwitchMode = AgentSwitchMode.LoggedOut, Description = p.Description }));
                    }


                    var dialog = new Window();
                    dialog.ResizeMode = ResizeMode.NoResize;
                    dialog.ShowInTaskbar = false;
                    dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    dialog.Height = 300;
                    dialog.Width = 300;
                    dialog.Title = string.Format("{0} - Reason Code(s)", reason);
                    var UCdialog = new UCReasonCodeView();
                    if (!(state.SwitchMode == AgentSwitchMode.WrapUp && canSendCti) && opts.Count() > 0)
                    {
                        WindowInteropHelper helper = new WindowInteropHelper(dialog);
                        if (System.Windows.Forms.Application.OpenForms.Count>0)
                        {
                            helper.Owner = System.Windows.Forms.Application.OpenForms[0].Handle;
                        }
                        

                        UCdialog.DataContext = new UCReasonCodeViewModel(va, (result, contact) =>
                        {
                            if (result)
                            {
                                StringBuilder str = new StringBuilder(contact.Code);
                                string strcode = contact.Code.Substring(contact.Code.Length - 2);
                                int.TryParse(strcode, out code);
                                if (canSendCti)
                                {
                                    CurrentAgentStateRcCode = contact;
                                    TempAgentState = contact;
                                    ctiCallInfoObject.SetAgentState(agentstate, code.ToString());
                                }

                            }
                            dialog.Close();
                        });

                        dialog.Content = UCdialog;
                        dialog.ShowDialog();
                    }
                    else
                    {

                        ctiCallInfoObject.SetAgentState(agentstate);
                        TempAgentState = StandardAgentStates.WrapUp;
                        CurrentAgentStateRcCode = TempAgentState;

                    }
                }
                else
                {
                    _fromlogin = false;
                    if (state.SwitchMode != AgentSwitchMode.WrapUp)
                    {
                        TempAgentState = AgentStates.FirstOrDefault(p => p.SwitchMode == state.SwitchMode);
                        CurrentAgentStateRcCode = TempAgentState;
                        ctiCallInfoObject.SetAgentState(code);

                    }
                    else
                    {
                        if (CurrentAgentState.SwitchMode == AgentSwitchMode.HandlingInteraction)
                        {
                            TempAgentState = StandardAgentStates.WrapUp;
                            CurrentAgentStateRcCode = TempAgentState;
                            if (canSendCti)
                                ctiCallInfoObject.SetAgentState(code);
                        }
                        else
                        {
                            TempAgentState = StandardAgentStates.WrapUp;
                            CurrentAgentStateRcCode = TempAgentState;

                            if (canSendCti)
                                ctiCallInfoObject.SetAgentState(code);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Logger.Log.Error("MediaBarViewModel: ", exception);
            }
        }

        private void NextStateChange(AgentState nextState)
        {
            Logger.Logger.Log.Debug(string.Format("Call Count Cti {0}, Interaction {1}", ctiCallInfoObject.GetCallDataCount(), InteractionProvider.Interactions.Count()));
            if (ctiCallInfoObject.GetCallDataCount() == 0 || InteractionProvider.Interactions.Count() == 0)
            {
                ctiCallInfoObject.QueryAgentState(InteractionProvider.Credentials.Extension);
            }
        }

        public AgentState CurrentAgentStateRcCode
        {
            get;
            set;
        }

        public AgentState OldAgentState
        {
            get;
            set;
        }

        public AgentState TempAgentState
        {
            get;
            set;
        }

        public AgentState CurrentAgentState
        {
            get
            {
                return _currentAgentState;
            }
            set
            {
                if (_frmAPI)
                {
                    if (setAgentState(value))
                    {
                        InteractionProvider.Agent.SetState(value);
                    }
                    _frmAPI = false;
                }
                else
                {
                    SynchronizationContext.Post(p => AgentStateChange(value), null);
                }
            }
        }

        private void showDialPad(object obj)
        {

            string caption = "Dial";
            bool IsDTMF = false;
            var dialog = new TransferDialog();
            var contacts = ContactProvider.GetContacts();

            WindowInteropHelper helper = new WindowInteropHelper(dialog);
            if (System.Windows.Forms.Application.OpenForms.Count > 0)
            {
                helper.Owner = System.Windows.Forms.Application.OpenForms[0].Handle;
            }

            if (CurrentInteraction != null && (CurrentInteraction.State == InteractionState.Active || CurrentInteraction.State == InteractionState.Conferenced || CurrentInteraction.State == InteractionState.Consulting))
            {
                caption = "Send DTMF";
                IsDTMF = true;
            }

            dialog.DataContext = new TransferDialogViewModel(contacts, (result, contact) =>
            {
                if (result)
                {
                    string targetNumber = dialog.cbTargetNumber.Text.Trim();
                    if (targetNumber != string.Empty)
                    {
                        if (!IsDTMF)
                        {
                            ctiCallInfoObject.MakeCall(targetNumber);
                            dialog.cbTargetNumber.Text = "";
                            dialog.Close();
                        }
                        else
                        {
                            ctiCallInfoObject.SendDTMF(CurrentInteraction.CallId, targetNumber);
                        }
                    }
                }
                else
                {
                    dialog.Close();
                }


            }, caption);

            dialog.ShowDialog();
        }

        private void showAgentLogin(object obj)
        {
            var dialog = new AgentLogin();
            var contacts = ContactProvider.GetContacts();

            WindowInteropHelper helper = new WindowInteropHelper(dialog);
            if (System.Windows.Forms.Application.OpenForms.Count > 0)
            { helper.Owner = System.Windows.Forms.Application.OpenForms[0].Handle; }

            dialog.DataContext = new AgentLoginViewModel(contacts, (result, contact) =>
            {
                if (result)
                {
                    Dial(contact);
                }

                dialog.Close();
            }, "Agent Login");

            dialog.ShowDialog();
        }

        private void showDialOptions()
        {
            ShowDialOptions = true;
        }

        private void showTransferDialog(object obj)
        {
            
            if (CurrentInteraction.CanCompleteTransfer != true)
            {
                var dialog = new TransferDialog();
                var contacts = ContactProvider.GetContacts();

                WindowInteropHelper helper = new WindowInteropHelper(dialog);
                if (System.Windows.Forms.Application.OpenForms.Count > 0)
                {
                    helper.Owner = System.Windows.Forms.Application.OpenForms[0].Handle;
                }
                

                dialog.DataContext = new TransferDialogViewModel(contacts, (result, contact) =>
                {
                    if (result)
                    {
                        string errmsg;
                        if (!ValidateDialNumber(contact.Number, out errmsg))
                        {
                            dialog.error.Text = errmsg;
                            dialog.error.Visibility = Visibility.Visible;
                            return;
                        }
                        string UUI = CurrentInteraction.InteractionData.Keys.Contains("UUI") ? CurrentInteraction.InteractionData["UUI"] : "";
                        if ((Convert.ToBoolean(obj) == true))
                        {
                            ctiCallInfoObject.OnCallTransferred(CurrentInteraction.CallId, contact.Number, true, UUI);
                            canCompleteTransfer = false;
                            canCompleteConference = false;
                            dialog.error.Text = "";
                        }
                        else
                        {
                            ctiCallInfoObject.OnCallTransferred(CurrentInteraction.CallId, contact.Number, false, UUI);
                            canCompleteTransfer = true;
                            canCompleteConference = true;
                            CurrentInteraction.CanCompleteTransfer = true;
                            OnPropertyChanged("TransferImage");
                            dialog.error.Text = "";
                        }

                    }

                    dialog.Close();
                });

                dialog.ShowDialog();
            }
            else
            {
                IInteraction inter = InteractionProvider.Interactions.FirstOrDefault(tr => !tr.CanCompleteTransfer);
                if (inter != null)
                {
                    ctiCallInfoObject.CompleteTransfer(CurrentInteraction.CallId, inter.CallId);
                    canCompleteTransfer = false;
                    canCompleteConference = false;
                    ChangeUIToRetrieveStateSyn();
                }
            }
        }

        private void showConferenceDialog(object obj)
        {
            if (CurrentInteraction.CanCompleteTransfer != true)
            {
                var dialog = new TransferDialog();
                var contacts = ContactProvider.GetContacts();
                WindowInteropHelper helper = new WindowInteropHelper(dialog);
                if (System.Windows.Forms.Application.OpenForms.Count>0)
                {
                    helper.Owner = System.Windows.Forms.Application.OpenForms[0].Handle;
                }
                
                dialog.DataContext = new TransferDialogViewModel(contacts, (result, contact) =>
                {
                    dialog.error.Visibility = Visibility.Collapsed;
                    if (result)
                    {
                        //if (string.IsNullOrEmpty(contact.Number))
                        //{
                        //    dialog.error.Text = "Please enter the number.";
                        //    dialog.error.Visibility = Visibility.Visible;
                        //    return;
                        //}
                        string errmsg;
                        if (!ValidateDialNumber(contact.Number, out errmsg))
                        {
                            dialog.error.Text = errmsg;
                            dialog.error.Visibility = Visibility.Visible;
                            return;
                        }
                        string UUI = CurrentInteraction.InteractionData.Keys.Contains("UUI") ? CurrentInteraction.InteractionData["UUI"] : "";
                        if ((Convert.ToBoolean(obj) == true))
                        {
                            ctiCallInfoObject.OnCallConferenced(CurrentInteraction.CallId, contact.Number, true, UUI);
                            canCompleteConference = false;
                            canCompleteTransfer = false;
                            dialog.error.Text = "";
                        }
                        else
                        {
                            ctiCallInfoObject.OnCallConferenced(CurrentInteraction.CallId, contact.Number, false, UUI);
                            canCompleteConference = true;
                            canCompleteTransfer = true;
                            CurrentInteraction.CanCompleteTransfer = true;
                            dialog.error.Text = "";
                        }
                    }
                    dialog.Close();
                }, "Conference");

                dialog.ShowDialog();
            }
            else
            {
                IInteraction inter = InteractionProvider.Interactions.FirstOrDefault(tr => !tr.CanCompleteTransfer);//&& (tr.State!=InteractionState.Dialing||tr.State!=InteractionState.Ringing)
                if (inter != null)
                {
                    ctiCallInfoObject.CompleteConference(CurrentInteraction.CallId, inter.CallId);
                }
            }
        }

        private IInteraction OnCallCompletedTest(string addedParty, string pri, string sec)
        {
            IInteraction Icall = null;
            if (CurrentInteraction.CanCompleteTransfer)
            {
                canCompleteConference = false;
                canCompleteTransfer = false;
                string exten = addedParty;
                Icall = InteractionProvider.Interactions.FirstOrDefault(p => p.CallId == pri);
                if (Icall == null)
                {
                    exten = ctiCallInfoObject.GetExtensionAniData(addedParty);
                    Icall = InteractionProvider.Interactions.FirstOrDefault(p => p.Address == exten && p.State != InteractionState.Held);
                }

                if (Icall != null)
                {
                    var call = Icall as ICall;
                    CurrentInteraction.CanCompleteTransfer = false;
                    CurrentInteraction.State = InteractionState.Active;
                    CurrentInteraction.EnableDropLastParty = true;
                    OnPropertyChanged("EnableDropLastParty");
                    if (CurrentInteraction.Address != exten)
                    {
                        ctiCallInfoObject.ReSetCallData(pri);

                    } ctiCallInfoObject.ReSetExtensionAniData(exten);
                }
                ChangeUIToRetrieveStateSyn();
                ResetDropButtonStatus();
            }
            else
            {
                //blind conference remove when successfull.
                var tempcall = InteractionProvider.Interactions.FirstOrDefault(p => p.CallId == sec);
                if (tempcall != null)
                {
                    _autoReset.Reset();
                    (tempcall as ICall).Accept();
                    _autoReset.WaitOne(1000);

                    _autoReset.Reset();
                    (tempcall as ICall).HangUp();
                    _autoReset.WaitOne(1000);

                }
            }

            return Icall;
        }

        private void dropLastPary(object obj)
        {
            if (addedPartiesList.Count > 0)
            {
                ctiCallInfoObject.OnDropLastParty(addedPartiesList[addedPartiesList.Count - 1], CurrentInteraction.CallId);
            }
        }

        private void showAgentLoginDialog(object obj)
        {
            bool isAgentLoggedIn = IsAlreadyLoggedIn();

            if (isAgentLoggedIn)
            {

                if (ctiCallInfoObject.IsACD)
                    AgentStateChange(StandardAgentStates.LoggedOut);
                else
                    ToggleLogOut();

                return;
            }
            var dialog = new AgentLogin();
            var contacts = ContactProvider.GetContacts();

            WindowInteropHelper helper = new WindowInteropHelper(dialog);
            if (System.Windows.Forms.Application.OpenForms.Count > 0)
            {
                helper.Owner = System.Windows.Forms.Application.OpenForms[0].Handle;
            }
            else
            {
                
            }
            
            dialog.DataContext = new AgentLoginViewModel(contacts, (result, contact) =>
            {
                if (result)
                {
                    string validationResult = Validate(dialog);
                    if (validationResult == string.Empty)
                    {
                        string agentId = dialog.agentIdText.Text.Trim();
                        string password = dialog.passwordText.Password.Trim();
                        string extension = dialog.extensionText.Text.Trim();
                        string queue = dialog.queueText.Text.Trim();
                        currentExtension = extension;
                        currentAgentId = agentId;
                        currentAgentPassword = password;
                        currentQueue = queue;
                        InteractionProvider.Credentials.AgentID = string.IsNullOrEmpty(agentId) ? 0 : int.Parse(agentId);
                        InteractionProvider.Credentials.Password = password;
                        InteractionProvider.Credentials.Extension = int.Parse(extension);
                        InteractionProvider.Credentials.Queue = int.Parse(string.IsNullOrEmpty(queue) ? "0" : queue);

                        ctiCallInfoObject.ConnectToServer(InteractionManager.AgentScreenPop.PrimaryCTIEngine, InteractionManager.AgentScreenPop.SecondaryCTIEngine);
                    }
                    else
                    {
                        dialog.errLabel.Text = validationResult;
                        dialog.errLabel.Visibility = Visibility.Visible;
                        return;
                    }
                }

                dialog.Close();
            }, isQueueEnabled: InteractionManager.AgentScreenPop.IsQueueEnabled);

            dialog.ShowDialog();
        }

        private string Validate(AgentLogin dialog)
        {
            string result = string.Empty;
            string missingFields = string.Empty;
            if (dialog.extensionText.Text.Trim() == "")
            {
                if (missingFields == string.Empty)
                {
                    missingFields = "Extension";
                }
                else
                {
                    missingFields = missingFields + "," + "Extension";
                }
            }

            if (InteractionManager.AgentScreenPop.IsQueueEnabled && dialog.queueText.Text.Trim() == "")
            {
                if (missingFields == string.Empty)
                {
                    missingFields = "Queue";
                }
                else
                {
                    missingFields = missingFields + " and " + "Queue";
                }
            }

            if (missingFields != string.Empty)
            {
                result = "Missing field(s): " + missingFields;
            }

            return result;
        }

        private bool ValidateDialNumber(string number, out string msg)
        {
            int diaNumber;
            if (string.IsNullOrEmpty(number))
            {
                msg = "Please enter the number!.";
                return false;
            }
            //else if (!int.TryParse(number, out diaNumber))
            //{
            //    msg = "Please enter valid number!.";
            //    return false;
            //}
            else
            {
                msg = "";
                return true;
            }
        }
        private void updateInteractionCount(InteractionCountUpdateInfo info)
        {

            Logger.Logger.Log.Info("Begin UpdateInteractioncount >> " + info);
            Application.Current.Dispatcher.BeginInvoke(new Action<InteractionCountUpdateInfo>((updateInfo) =>
            {
                Logger.Logger.Log.Info("Progress UpdateInteractionCount");
                var updateValue = updateInfo.UpdateType == UpdateType.Add ? 1 : -1;
                switch (updateInfo.MediaType)
                {
                    case MediaType.Voice:
                        CallCount += updateValue;
                        if (CallCount < 0)
                            CallCount = 0;
                        break;
                    case MediaType.Email:
                        EmailCount += updateValue;
                        break;
                    case MediaType.Chat:
                        break;
                    case MediaType.Incident:
                        break;
                    case MediaType.Web:
                        WebIncidentCount += updateValue;
                        break;
                    default:
                        break;
                }
            }), info);
            Logger.Logger.Log.Info("Completed UpdateInteractionCount >>" + info);
        }

        private void timerTick(object sender, EventArgs e)
        {
            OnPropertyChanged("InteractionTime");
        }

        private class InteractionCountUpdateInfo
        {
            public MediaType MediaType { get; set; }

            public UpdateType UpdateType { get; set; }
        }

        private enum UpdateType
        {
            Add,
            Subtract
        }


        public void ImplictLogin()
        {
            bool isAgentLoggedIn = IsAlreadyLoggedIn();

            if (isAgentLoggedIn)
            {
                ToggleLogOut();
                ctiCallInfoObject.AgentLogout();
                return;
            }

            var contacts = ContactProvider.GetContacts();
            if (InteractionProvider.Credentials.Extension == 0)
            {
                return;
            }
            currentAgentId = InteractionProvider.Credentials.AgentID.ToString();
            currentAgentPassword = InteractionProvider.Credentials.Password;
            currentExtension = InteractionProvider.Credentials.Extension.ToString();
            currentQueue = InteractionProvider.Credentials.Queue.ToString();
            ctiCallInfoObject.ConnectToServer(InteractionManager.AgentScreenPop.PrimaryCTIEngine, InteractionManager.AgentScreenPop.SecondaryCTIEngine);
            _isImpliclogin = true;

        }

        private void OnQueryAgentStateResponse(string str)
        {
            try
            {
                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(str);
                XmlNodeList callIdNode = xdoc.GetElementsByTagName("WorkMode");
                int state = Convert.ToInt32(callIdNode.Item(0).InnerText);
                AgentState agentState = state == (int)AgentWorkMode.Aux ? StandardAgentStates.NotReady : (state == (int)AgentWorkMode.AutoIn || state == (int)AgentWorkMode.ManualIn) ? StandardAgentStates.Available : state == (int)AgentWorkMode.None ? StandardAgentStates.LoggedOut : state == (int)AgentWorkMode.ACW ? StandardAgentStates.WrapUp : StandardAgentStates.LoggedOut;
                if (agentState.SwitchMode == AgentSwitchMode.LoggedOut)
                {
                    ctiCallInfoObject.AgentLogin(InteractionProvider.Credentials.AgentID.ToString(), InteractionProvider.Credentials.Password, InteractionProvider.Credentials.Extension.ToString(), InteractionProvider.Credentials.Queue.ToString());
                }
                else
                {
                    _frmAPI = true;

                    bool isAgentLoggedIn = IsAlreadyLoggedIn();
                    if (!isAgentLoggedIn)
                    {
                        System.Threading.Tasks.Task.Factory.StartNew(() =>
                        {
                            if (_isImpliclogin)
                            {
                                AESImplictLogin();
                            }
                            else
                            {
                                ToggleLogin(InteractionProvider.Credentials.AgentID.ToString(), InteractionProvider.Credentials.Password, InteractionProvider.Credentials.Extension.ToString(), InteractionProvider.Credentials.Queue.ToString());
                            }
                        }).ContinueWith((s) =>
                        {
                            _frmAPI = true;
                            //AgentStates.SingleOrDefault(p =>p.Code p.SwitchMode == agentState.SwitchMode).Description = agentState.Description;
                            CurrentAgentState = agentState;
                            _frmAPI = false;
                            synchronizeAndRaiseOnPropertyChanged("CurrentAgentState");
                            _isImpliclogin = false;
                        });
                    }
                    else
                    {

                        _frmAPI = true;
                        if (CurrentAgentState.SwitchMode != agentState.SwitchMode)
                        {
                            if (agentState.SwitchMode == AgentSwitchMode.NotReady && TempAgentState != null && agentState.SwitchMode == TempAgentState.SwitchMode)
                            {
                                AgentStates.FirstOrDefault(p => p.SwitchMode == TempAgentState.SwitchMode && !string.IsNullOrEmpty(p.Code) && Convert.ToInt32(p.Code) > 0).Description = TempAgentState.Description;
                                CurrentAgentState = agentState;
                                //CurrentAgentState.Description = CurrentAgentStateRcCode.Description;
                                CurrentAgentStateRcCode = null;
                                TempAgentState = null;
                            }
                            else
                            {
                                AgentStates.SingleOrDefault(p => p.SwitchMode == agentState.SwitchMode && !string.IsNullOrEmpty(p.Code) && Convert.ToInt32(p.Code) > 0).Description = agentState.Description;
                                CurrentAgentState = agentState;
                                CurrentAgentStateRcCode = null;
                                TempAgentState = null;
                            }
                        }
                        synchronizeAndRaiseOnPropertyChanged("CurrentAgentState");
                        _frmAPI = false;
                    }
                }
                Logger.Logger.Log.Info(string.Format("Agent State {0} {1}", str, state));
            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Error(string.Format("Agent State {0}", ex.StackTrace));
                Logger.Logger.Log.Error("Agent State ", ex);
            }
        }


        [ImportMany(AllowRecomposition = true)]
        public ICollection<IScreenPopHandler> ScreenPopHandlers { get; set; }

        private void ScreenPopHandler(IInteraction e)
        {
            ScreenPopOptions aniscreenpop = InteractionManager.AgentScreenPop.ScreenPopOptionsList.SingleOrDefault(p => p.Type == 0 && p.Name.Equals("TranPop", StringComparison.OrdinalIgnoreCase));
            if (InteractionProvider.CurrentInteraction != null && ((InteractionProvider.CurrentInteraction as ICall).CallType == CallType.Consult && aniscreenpop != null && aniscreenpop.Description == "D"))
            {
                //lock (_Imgrobject)
                {
                    foreach (var handler in ScreenPopHandlers)
                    {
                        if (SynchronizationContext != null)
                            SynchronizationContext.Post(o => ((InvocationTarget)o).HandleInteraction(), new InvocationTarget
                            {
                                Interaction = e,
                                Handler = handler
                            });
                        else
                            handler.HandleInteraction(e);
                    }
                    Logger.Logger.Log.Info(string.Format("Interaction >> {0}", e));
                }
            }

        }

        public void RunLatestIncident()
        {
            foreach (var _handler in ScreenPopHandlers)
            {
                if (SynchronizationContext != null)
                {
                    SynchronizationContext.Post(o => ((InvocationTarget)o).RunIncidents(), new InvocationTarget
                    {
                        Handler = _handler
                    });
                }
                else
                    _handler.RunIncidents();
            }
        }

        private class InvocationTarget
        {
            public IInteraction Interaction { get; set; }

            public IScreenPopHandler Handler { get; set; }

            public void HandleInteraction()
            {
                Handler.HandleInteraction(Interaction);
            }

            public void RunIncidents()
            {
                Handler.RunIncidents();
            }
        }
    }
}