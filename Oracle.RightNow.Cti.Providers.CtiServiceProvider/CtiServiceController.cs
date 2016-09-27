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
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using Oracle.RightNow.Cti.Providers.CtiServiceProvider;
using Oracle.RightNow.Cti.CtiServiceProvider.Messaging;
using Oracle.RightNow.Cti.CtiServiceProvider.Messaging.Messages;
using RightNow.AddIns.AddInViews;
using Oracle.RightNow.Cti.Model;

namespace Oracle.RightNow.Cti.CtiServiceProvider.Controller.ViewModels {
    public class CtiServiceController : IRemoteMessagingClient, INotifyPropertyChanged {
        private SwitchClient _client;
        private object _clientSyncRoot = new object();
        private Device _controllerDevice;

        private Device _selectedDevice;

        private SwitchInteraction _selectedInteraction;

        private IGlobalContext _rightnowGlobalContext;

        private SwitchInteraction _intraction;

        public event PropertyChangedEventHandler PropertyChanged;

        public delegate void InteractionQueued(InteractionMessage message);

        public event InteractionQueued OnInteractionQueued;

        [ImportingConstructor]
        public CtiServiceController(IGlobalContext context)
        {
            _controllerDevice = new Device {
                Address = "CTI Controller",
                Id = Guid.NewGuid(),
                Agent = new Agent {
                    Id = Guid.NewGuid(),
                    DisplayName = context.Login,
                    IsSystemProcess = true
                }
            };

            _rightnowGlobalContext = context;

            Devices = new ObservableCollection<Device>();
            Interactions = new ObservableCollection<SwitchInteraction>();
            initializeSwitchClient();
        }

        public ObservableCollection<Device> Devices { get; set; }

        public ObservableCollection<SwitchInteraction> Interactions { get; set; }

        public Device SelectedDevice {
            get {
                return _selectedDevice;
            }
            set {
                if (_selectedDevice != value) {
                    _selectedDevice = value;
                    OnPropertyChanged("SelectedDevice");
                }
            }
        }

        public SwitchInteraction SelectedInteraction {
            get {
                return _selectedInteraction;
            }
            set {
                if (_selectedInteraction != value) {
                    _selectedInteraction = value;
                    OnPropertyChanged("SelectedInteraction");
                }
            }
        }

        public void CreateCall(string ani, string dnis, Dictionary<string,string> dict, CallType calltype,int callid,string currentextension,bool conference,bool _isCallFailed=false) {//CallType calltype
            //Dictionary<string, string> dict = new Dictionary<string, string>();
            //dict.Add("UUI", uui);
            Logger.Logger.Log.Debug(string.Format("Create Call ani {0} dnis {1} data {2} callType {3}", ani, dnis, dict, calltype));

            try
            {
                var interaction = new SwitchInteraction
                {
                    Id = Guid.NewGuid(),
                    Properties = dict,
                    SourceAddress = ani,
                    Queue = dnis,
                    Type = InteractionType.Call,
                    //Direction=direction,
                    CallID=callid,
                    CallType = calltype,
                    isCallFailed=_isCallFailed,
                    CurrentExtension=currentextension,
                    ConferencedCall=conference
                    
                };
                _intraction = interaction;
                _client.Request(new CreateInteractionMessage { Interaction = interaction });
                Logger.Logger.Log.Info("Create Call" + interaction);
            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Error(string.Format("Create Call Exception {0} StackTrace {1}", ex, ex.StackTrace));
            }
            
        }

        public void CreateOutboundCall(string ani, string dnis, string uui)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("UUI", uui);
            var interaction = new SwitchInteraction
            {
                Id = Guid.NewGuid(),
                Properties = dict,
                SourceAddress = ani,
                Queue = dnis,
                Type= InteractionType.Call
            };

            _intraction = interaction;
            interaction.State = SwitchInteractionState.RingingOut;

            _client.Request(new CreateInteractionMessage { Interaction = interaction });
        }

        public SwitchInteraction GetInteration()
        {
            return _intraction;
        }

        public void HandleMessage(Message message) {
            switch (message.Type)
            {
                case SwitchMessageType.AgentState:
                    handleAgentStateMessage((AgentStateMessage)message);
                    break;
                case SwitchMessageType.Interaction:
                    handleInteractionState((InteractionMessage)message);
                    break;
                case SwitchMessageType.Disconnected:
                    handleDisconnectedMessage((SwitchMessage)message);
                    break;
                case SwitchMessageType.SwitchState:
                    updateSwitchState((SwitchStateMessage)message);
                    break;
                case SwitchMessageType.DeviceInfo:
                    updateDeviceInfo((DeviceInfoMessage)message);
                    break;
                default:
                    break;
            }
        }
  
        protected virtual void OnPropertyChanged(string propertyName) {
            var temp = PropertyChanged;
            if (temp != null) {
                temp(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void getSwitchState() {
            _client.Request(new SwitchMessage(SwitchMessageType.GetSwitchState) { DeviceId = _controllerDevice.Id });
        }

        private void handleAgentStateMessage(AgentStateMessage message) {
            Application.Current.Dispatcher.Invoke(new Action(() => {
                if (message.State == SwitchAgentState.LoggedIn) {
                    if (message.DeviceId == _controllerDevice.Id)
                        getSwitchState();
                    else {
                        _client.Request(new GetDeviceInfoMessage {
                            DeviceId = _controllerDevice.Id,
                            TargetDeviceId = message.DeviceId
                        });
                    }
                }
                else {
                    if (message.DeviceId != _controllerDevice.Id) {
                        var device = Devices.FirstOrDefault(d => d.Id == message.DeviceId);

                        if (device != null) {
                            device.Agent.State = message.State;
                        }
                    }
                }
            }));
        }
  
        private void handleDisconnectedMessage(SwitchMessage message) {
            Application.Current.Dispatcher.Invoke(new Action(() => {
                var device = Devices.FirstOrDefault(d => d.Id == message.DeviceId);

                if (device != null) {
                    Devices.Remove(device);
                }
            }));
        }
  
        private void handleInteractionState(InteractionMessage message) {
            var interaction = Interactions.FirstOrDefault(i => i.Id == message.Interaction.Id);

            Application.Current.Dispatcher.Invoke(new Action(() => {
                if (interaction != null) {
                    interaction.State = message.Interaction.State;

                    switch (message.Action)
                    {
                        case InteractionMessageAction.Queued:
                            break;
                        case InteractionMessageAction.Created:
                            break;
                        case InteractionMessageAction.Assigned:
                            interaction.Agent = message.Interaction.Agent;
                            break;
                        case InteractionMessageAction.StateChanged:
                            if (message.Interaction.State == SwitchInteractionState.Completed) {
                                Interactions.Remove(interaction);
                            }
                            break;
                        default:
                            break;
                    }
                }
                else {
                    Interactions.Add(message.Interaction);
                    if (OnInteractionQueued != null)
                    {
                        OnInteractionQueued(message);
                    }
                }
            }));
        }
  
        private SwitchClient initializeClient() {
            var client = new SwitchClient(new InstanceContext(this));
            try {
                client.Open();
                (client as ICommunicationObject).Faulted += CtiServiceController_Faulted;
                _client = client;
            }
            catch (EndpointNotFoundException) {
                CtiServiceSwitch.Run(_rightnowGlobalContext);
                client = initializeClient();
            }

            return client;
        }

        void CtiServiceController_Faulted(object sender, EventArgs e)
        {
                 
        }

        private void initializeSwitchClient() {
            Task.Factory.StartNew(() => {
                if (_client == null) {
                    lock (_clientSyncRoot) {
                        if (_client == null) {
                            var client = initializeClient();
                            _client = client;
                            client.Connect(_controllerDevice, true);                          
                        }
                    }
                }
            });
        }
  
        private void updateDeviceInfo(DeviceInfoMessage message) {
            Application.Current.Dispatcher.Invoke(new Action(() => {
                var device = Devices.FirstOrDefault(d => d.Id == message.TargetDevice.Id);

                if (device == null) {
                    Devices.Add(message.TargetDevice);
                }
                else {
                    device.Address = message.TargetDevice.Address;
                    device.Agent.Interactions = message.TargetDevice.Agent.Interactions;
                    device.Agent.State = message.TargetDevice.Agent.State;
                    device.Agent.Device = device;
                }
            }));
        }
  
        private void updateSwitchState(SwitchStateMessage message) {
            Application.Current.Dispatcher.Invoke(new Action(() => {
                Devices.Clear();
                var devices = message.Devices.Where(d => d.Agent != null && !d.Agent.IsSystemProcess);
                foreach (var device in devices) {
                    Devices.Add(device);
                }
            }));
        }
    }
}