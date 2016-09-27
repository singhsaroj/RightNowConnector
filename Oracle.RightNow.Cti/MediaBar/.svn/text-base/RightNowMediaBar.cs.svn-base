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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Oracle.RightNow.Cti.Model;
using RightNow.AddIns.AddInViews;
using RightNow.AddIns.Common;
using System.Text.RegularExpressions;

namespace Oracle.RightNow.Cti.MediaBar
{
    public class RightNowMediaBar : NotifyingObject, IPartImportsSatisfiedNotification
    {
        protected AgentState _currentAgentState;
        private int _enabledButtons;
        private object _lockRN = new object();
        private object _lockRN_state = new object();
        private Mutex _mutex = new Mutex();
        private bool _canAssociateToRecord;
        private bool _canMakeCall;
        private InteractionManager _interactionManager;
        ObservableCollection<IInteraction> ocInteractions;
        public bool _frmAPI = false;
        public List<string> addedPartiesList;
        //public AutoResetEvent _waitHandle = new AutoResetEvent(false);
        public RightNowMediaBar()
        {
            Interactions = new ObservableCollection<IInteraction>();
            InteractionsList = new ObservableCollection<IInteraction>();
            ContextContacts = new ObservableCollection<Contact>();
            addedPartiesList = new List<string>();
        }

        public SynchronizationContext SynchronizationContext { get; set; }

        [Import]
        internal IGlobalContext RightNowGlobalContext { get; set; }

        public IEnumerable<AgentState> AgentStates
        {
            get
            {
                return InteractionManager.AgentStates;
            }
        }

        public IInteraction CurrentInteraction
        {
            get
            {
                return InteractionProvider.CurrentInteraction;
            }
            set
            {
                InteractionProvider.CurrentInteraction = value;
                OnPropertyChanged("CurrentInteraction");

                synchronizeAndRaiseOnPropertyChanged("CurrentInteraction");
                synchronizeButtonStates();
            }
        }

        [Import]
        public InteractionManager InteractionManager
        {
            get
            {
                return _interactionManager;
            }
            set
            {
                _interactionManager = value;
                _interactionManager.SynchronizationContext = this.SynchronizationContext;
            }
        }

        public IInteractionProvider InteractionProvider
        {
            get
            {
                return InteractionManager.InteractionProvider;
            }
        }

        public ObservableCollection<IInteraction> Interactions
        {
            get;
            set;
        }

        public ObservableCollection<IInteraction> InteractionsList
        {
            get;
            set;
        }

        public int EnabledButtons
        {
            get
            {
                return _enabledButtons;
            }
            set
            {
                _enabledButtons = value;
                OnPropertyChanged("EnabledButtons");
            }
        }

        public bool EnableDropLastConfParty
        {
            get;
            set;
        }


        public bool CanAssociateToRecord
        {
            get
            {
                return _canAssociateToRecord;
            }
            set
            {
                if (_canAssociateToRecord != value)
                {
                    _canAssociateToRecord = value;
                    OnPropertyChanged("CanAssociateToRecord");
                }
            }
        }

        public bool CanMakeCall
        {
            get
            {
                return _canMakeCall;
            }
            set
            {
                if (_canMakeCall != value)
                {
                    _canMakeCall = value;
                    OnPropertyChanged("CanMakeCall");
                }
            }
        }

        public void EnableButtons(MediaBarButtons buttons)
        {
            synchronizeAndInvoke(o => EnabledButtons |= (int)o, buttons);
        }

        public void DisableButtons(MediaBarButtons buttons)
        {
            synchronizeAndInvoke(o => EnabledButtons &= ~((int)o), buttons);
        }

        public virtual void OnImportsSatisfied()
        {
            if (InteractionManager != null)
            {
                // Interaction events
                InteractionProvider.CurrentInteractionChanged += CurrentInteractionChangedHandler;
                InteractionProvider.InteractionConnected += InteractionConnectedHandler;
                InteractionProvider.InteractionCompleted += InteractionCompletedHandler;
                InteractionProvider.InteractionDisconnected += InteractionDisconnectedHandler;
                InteractionProvider.NewInteraction += NewInteractionHandler;

                //Agent events
                InteractionProvider.Agent.StateChanged += agentStateChangedHandler;

                setAgentState(InteractionProvider.Agent.CurrentState);
            }

            if (RightNowGlobalContext != null)
            {
                RightNowGlobalContext.AutomationContext.CurrentEditorTabChanged += currentEditorTabChanged;
            }

        }

        private void currentEditorTabChanged(object sender, EditorTabChangedEventArgs e)
        {
            try
            {
                ContextContacts.Clear();
                WorkspaceRecordType? workspaceType = null;
                if (RightNowGlobalContext.AutomationContext.CurrentWorkspace != null)
                    workspaceType = RightNowGlobalContext.AutomationContext.CurrentWorkspace.WorkspaceType;

                if (CurrentInteraction != null && workspaceType != null)
                {
                    CanAssociateToRecord = workspaceType.Value == WorkspaceRecordType.Contact || workspaceType.Value == WorkspaceRecordType.Incident;
                }
                else
                    CanAssociateToRecord = false;

                if (workspaceType != null && workspaceType.Value == WorkspaceRecordType.Contact)
                {
                    var contact = RightNowGlobalContext.AutomationContext.CurrentWorkspace.GetWorkspaceRecord(WorkspaceRecordType.Contact) as IContact;
                    if (contact == null)
                    {
                        RightNowGlobalContext.AutomationContext.CurrentWorkspace.DataLoaded += currentWorkspaceDataLoaded;

                        contact = RightNowGlobalContext.AutomationContext.CurrentWorkspace.GetWorkspaceRecord(WorkspaceRecordType.Contact) as IContact;
                        if (contact != null)
                        {
                            RightNowGlobalContext.AutomationContext.CurrentWorkspace.DataLoaded -= currentWorkspaceDataLoaded;
                            loadContextNumbers(contact);
                        }
                    }
                    else
                        loadContextNumbers(contact);
                }
            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Error("Error on currentEditorTabChanged :", ex);
            }
           
        }

        private void loadContextNumbers(IContact contact)
        {
            addContextContact("Home", contact.PhHome);
            addContextContact("Mobile", contact.PhMobile);
            addContextContact("Office", contact.PhOffice);
        }

        private void currentWorkspaceDataLoaded(object sender, EventArgs e)
        {
            RightNowGlobalContext.AutomationContext.CurrentWorkspace.DataLoaded -= currentWorkspaceDataLoaded;
            var contact = RightNowGlobalContext.AutomationContext.CurrentWorkspace.GetWorkspaceRecord(WorkspaceRecordType.Contact) as IContact;
            if (contact != null)
            {
                loadContextNumbers(contact);
            }
        }


        private void addContextContact(string p1, string p2)
        {
            if (string.IsNullOrEmpty(p2))
                return;
            var name = string.Format("{0} ({1})", p1, p2);
            ContextContacts.Add(new Contact
            {
                Name = name,
                Number = p2,
                TransferType = TransferTypes.OutboundDialing,
                Description = name,
            });
        }

        /// <summary>
        /// Toggles the agent login state.
        /// </summary>
        public void ToggleLogin()
        {
            Task.Factory.StartNew(() =>
            {
                if (InteractionProvider.Agent.CurrentState != null &&
                    InteractionProvider.Agent.CurrentState.SwitchMode == AgentSwitchMode.LoggedOut)
                {
                    InteractionProvider.Login();
                }
            });
        }

        public bool IsAlreadyLoggedIn()
        {
            if (InteractionProvider.Agent.CurrentState != null &&
                 InteractionProvider.Agent.CurrentState.SwitchMode == AgentSwitchMode.LoggedOut)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void ToggleLogin(string agentId, string password, string extension, string queue)
        {
            InteractionProvider.ReLogin(agentId, password, extension, queue);
            Logger.Logger.Log.Info("Toggle Login");
        }

        public void ToggleLogOut()
        {
            InteractionProvider.Logout();
        }

        public void AESImplictLogin()
        {

            InteractionProvider.Login(InteractionProvider.Credentials.AgentID.ToString(), InteractionProvider.Credentials.Password, InteractionProvider.Credentials.Extension.ToString(), InteractionProvider.Credentials.Queue.ToString());
            Logger.Logger.Log.Info("Implict Login");
        }

        /// <summary>
        /// Initiates an outbound dial request.
        /// </summary>
        /// <param name="contact">The contact to be dialed.</param>
        public void Dial(Contact contact)
        {
            Task.Factory.StartNew(() =>
            {
                InteractionProvider.Device.Dial(contact.Number);
            });
        }

        /// <summary>
        /// Answers the current interaction
        /// </summary>
        public void AnswerCall()
        {
            Task.Factory.StartNew(() =>
            {
                var call = InteractionProvider.CurrentInteraction as ICall;
                if (call != null && call.State == InteractionState.Ringing)
                {
                    call.Accept();
                }
            });
        }

        /// <summary>
        /// Disconnects the current interaction, if it is a call.
        /// </summary>
        public void HangUpCall()
        {
            Task.Factory.StartNew(() =>
            {
                var call = InteractionProvider.CurrentInteraction as ICall;
                if (call != null)
                {
                    call.HangUp();
                }
            });
        }

        /// <summary>
        /// Transfers the current interaction, if it is a call.
        /// </summary>
        public void TransferCall(Contact contact)
        {
            Task.Factory.StartNew(c =>
            {

                var call = InteractionProvider.CurrentInteraction as ICall;
                if (call != null)
                {
                    InteractionProvider.Device.InitiateTransfer(call, contact.TransferType, contact.Number);
                }
            }, contact, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
        }

        /// <summary>
        /// Conferences the current interaction, if it is a call.
        /// </summary>
        public void ConferenceCall(Contact contact)
        {
            Task.Factory.StartNew(c =>
            {
                var call = InteractionProvider.CurrentInteraction as ICall;
                if (call != null)
                {
                    InteractionProvider.Device.InitiateConference(call, TransferTypes.Conference, contact.Number);
                }
            }, contact, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
        }

        /// <summary>
        /// If the current interaction is a call, it will be placed on hold if active or retrieved if on hold.
        /// </summary>
        public void HoldRetrieveCall()
        {
            Task.Factory.StartNew(() =>
            {
                var call = InteractionProvider.CurrentInteraction as ICall;
                if (call != null)
                {
                    if (call.State == InteractionState.Active)
                    {
                        call.Hold();
                    }
                    else if (call.State == InteractionState.Held)
                    {
                        call.Retrieve();
                    }
                }
            });
        }

        public void ChanageUIToDisconnected()
        {
            synchronizeAndInvoke(o =>
            {
                EnabledButtons = (int)MediaBarButtons.None;
            });
        }

        public void ChanageToOutboundRinging()
        {
            synchronizeAndInvoke(o =>
            {
                EnabledButtons = (int)MediaBarButtons.AnswerHangup;
            });
        }

        public void ChangeUIToConferencedState()
        {
            SetButtonHoldRetrieveStates(false);
        }

        public  void ChangeUIToHoldState(string callid)
        {
            //foreach (IInteraction interaction in InteractionProvider.Interactions)
            //{
            //    var call = interaction as ICall;
            //    if (call != null)
            //    {
            //        call.Hold();
            //        SetButtonHoldRetrieveStates(true);
            //    }
            //}

            //var call = InteractionProvider.CurrentInteraction as ICall;
            IInteraction call = InteractionProvider.Interactions.FirstOrDefault(itr => itr.CallId == callid);
            if (call != null)
            { 
                (call as ICall).Hold();             
            }

            //synchronizeButtonStates();
        }

       

        public void ChangeUIToRetrieveState(string callid)
        {
            IInteraction call = InteractionProvider.Interactions.FirstOrDefault(itr => itr.CallId == callid);
            
            if (call != null)
            {
                if ((call as ICall).CallType == CallType.Consult && call.ConferencedCall)
                {
                    (call as ICall).Conferenced();                    
                }
                else if ((call as ICall).CallType == CallType.Consult && !call.ConferencedCall && call.BlindCall == InteractionBlindType.Normal && InteractionProvider.Interactions.Count() > 1)
                {
                    (call as ICall).Consulting();
                    
                }
                else
                {
                    (call as ICall).Retrieve();
                }

                SetButtonHoldRetrieveStates(false);
            }
        }

        public void ChangeUIToRetrieveStateSyn()
        {
            synchronizeButtonStates();
        }

        public ObservableCollection<Contact> ContextContacts { get; set; }

        public void CompleteInteraction()
        {
            Task.Factory.StartNew(() =>
            {
                InteractionProvider.CompleteInteraction();
            });
        }

        public virtual void InitiateTransfer()
        {
        }

        public virtual void InitiateConference()
        {
        }

        protected void EnableContextSynchronization(SynchronizationContext context)
        {
            SynchronizationContext = context;
        }

        private void agentStateChangedHandler(object sender, AgentStateChangedEventArgs e)
        {
            switch (e.NewState.SwitchMode)
            {
                case AgentSwitchMode.LoggedIn:
                    CanMakeCall = true;
                    break;
                case AgentSwitchMode.Ready:
                    CanMakeCall = true;
                    break;
                case AgentSwitchMode.NotReady:
                    CanMakeCall = true;
                    break;
                case AgentSwitchMode.LoggedOut:
                    CanMakeCall = false;
                    break;
                case AgentSwitchMode.WrapUp:
                    break;
                case AgentSwitchMode.HandlingInteraction:
                    break;
                case AgentSwitchMode.NewReason:
                    break;
                default:
                    break;
            }

            setAgentState(e.NewState);
        }

        protected virtual void CurrentInteractionChangedHandler(object sender, InteractionEventArgs e)
        {
            synchronizeAndRaiseOnPropertyChanged("CurrentInteraction");
            synchronizeButtonStates();
        }

        protected virtual void InteractionCompletedHandler(object sender, InteractionEventArgs e)
        {
            synchronizeAndInvoke(o => Interactions.Remove((IInteraction)o), e.Interaction);
            synchronizeButtonStates();
            CanAssociateToRecord = CanAssociateToRecord && Interactions.Count > 0;
        }

        private void synchronizeButtonStates()
        {
            //Regex chatIncidentRegex = new Regex("^UUI[1-2]", RegexOptions.IgnoreCase);
            
            MediaType currentInteractionType = MediaType.Voice;
            if (CurrentInteraction != null)
                currentInteractionType = CISpecification.cIInfoType(CurrentInteraction);
            else Logger.Logger.Log.Debug("CurrentInteraction is null.");

            Logger.Logger.Log.Debug("CurrectInteraction MediaType: " + currentInteractionType.ToString());

            synchronizeAndInvoke(o =>
            {
                if (CurrentInteraction == null || CurrentInteraction.State == InteractionState.Disconnected)
                {
                    EnabledButtons = (int)MediaBarButtons.None;
                }
                else
                {
                    //switch (CurrentInteraction.Type)
                    switch (currentInteractionType)
                    {
                        case MediaType.Voice:
                            if (((ICall)CurrentInteraction).State == InteractionState.Ringing)
                                EnabledButtons = (int)MediaBarButtons.AnswerHangup;
                            else if (((ICall)CurrentInteraction).State == InteractionState.Held)
                            {
                                if (CurrentInteraction.CanCompleteTransfer == true)
                                {
                                    //if(intera
                                    EnabledButtons = (int)MediaBarButtons.ConsultConferenced;
                                }
                                else
                                {
                                    EnabledButtons = (int)MediaBarButtons.HoldRetrieve;
                                }
                            }
                            //else if(((ICall)CurrentInteraction).State == InteractionState.Chat){
                            //    EnabledButtons = (int)MediaBarButtons.Chat;
                            //}
                            //else if (((ICall)CurrentInteraction).State == InteractionState.Incident) {
                            //    EnabledButtons = (int)MediaBarButtons.Incident;
                            //}
                            else if (((ICall)CurrentInteraction).State == InteractionState.RingingOut)
                                EnabledButtons = (int)MediaBarButtons.AnswerHangup;
                            else
                                if (CurrentInteraction.CanCompleteTransfer)
                                {
                                    EnabledButtons = (int)MediaBarButtons.ConsultConferenced;
                                }
                                //else if (CurrentInteraction.ConferencedCall)
                                //{
                                //    EnabledButtons = (int)MediaBarButtons.ConferencedCall;
                                //}
                                else
                                {
                                    EnabledButtons = (int)MediaBarButtons.Voice;
                                }
                            break;
                        case MediaType.Chat:
                            Logger.Logger.Log.Debug("Chat Interaction.");
                            if (((ICall)CurrentInteraction).State == InteractionState.Ringing)
                                EnabledButtons = (int)MediaBarButtons.AnswerHangup;
                            else
                                EnabledButtons = (int)MediaBarButtons.None;
                            break;
                        case MediaType.Incident:
                            Logger.Logger.Log.Debug("Incident Interaction.");
                            if (((ICall)CurrentInteraction).State == InteractionState.Ringing)
                                EnabledButtons = (int)MediaBarButtons.AnswerHangup;
                            else
                                EnabledButtons = (int)MediaBarButtons.None;
                            break;
                    }
                    ResetDropLastParty();
                }
            });
        }

        private void ResetDropLastParty()
        {
            if (CurrentInteraction.EnableDropLastParty == true && addedPartiesList.Count > 0)
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

        private void SetButtonHoldRetrieveStates(bool hold)
        {
            if (hold)
            {
                synchronizeAndInvoke(o =>
                {
                    EnabledButtons = (int)MediaBarButtons.HoldRetrieve;
                });
            }
            else
            {
                synchronizeAndInvoke(o =>
                {
                    EnabledButtons = (int)MediaBarButtons.Voice;
                });
            }
            
        }

        protected virtual void InteractionConnectedHandler(object sender, InteractionEventArgs e)
        {
            synchronizeButtonStates();
        }

        protected virtual void InteractionDisconnectedHandler(object sender, InteractionEventArgs e)
        {
            synchronizeAndInvoke(o => EnabledButtons &= ~((int)MediaBarButtons.All));
        }

        protected virtual void NewInteractionHandler(object sender, InteractionEventArgs e)
        {
            synchronizeAndInvoke(o => Interactions.Add((IInteraction)o), e.Interaction);
        }

        protected bool setAgentState(AgentState state)
        {
            if (_currentAgentState != state)
            {
                _currentAgentState = state;
                synchronizeAndRaiseOnPropertyChanged("CurrentAgentState");
                return true;
            }
            return false;
        }

        private void synchronizeAndInvoke(Action<object> action, object state = null)
        {
            lock (_lockRN_state)
            {
                if (SynchronizationContext != null)
                {
                    SynchronizationContext.Post(new SendOrPostCallback(action), state);
                }
                else
                {
                    action(state);
                }
            }
        }

        protected void synchronizeAndRaiseOnPropertyChanged(string propertyName)
        {

            lock (_lockRN)
            {
                if (SynchronizationContext != null)
                {
                    SynchronizationContext.Post(new SendOrPostCallback(o => OnPropertyChanged(o.ToString())), propertyName);
                }
                else
                {
                    OnPropertyChanged(propertyName);
                }
            }
        }
    }
}