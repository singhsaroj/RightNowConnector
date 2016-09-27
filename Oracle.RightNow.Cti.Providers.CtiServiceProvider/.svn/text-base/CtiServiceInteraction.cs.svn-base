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
using System.ComponentModel;
using Oracle.RightNow.Cti.Model;
using Oracle.RightNow.Cti.CtiServiceProvider.Messaging.Messages;
using System.Runtime.CompilerServices;

namespace Oracle.RightNow.Cti.Providers.CtiServiceProvider {
    public abstract class CtiServiceInteraction : NotifyingObject, IInteraction, ISwitchMessageConsumer {
        private string _id;
        private IDictionary<string, string> _interactionData;
        private Guid _internalId;
        private InteractionState _state;

        public event EventHandler Connected;
        public event EventHandler DataChanged;
        public event EventHandler Disconnected;
        public event EventHandler<InteractionStateChangedEventArgs> StateChanged;

        public CtiServiceInteraction(CtiServiceProvider provider) {
            Provider = provider;
            AdditionalIdentifiers = new Dictionary<string, string>();
            ((ISwitchMessageProducer)Provider).Subscribe(this);
            InteractionData = new Dictionary<string, string>();
        }

        public IDictionary<string, string> AdditionalIdentifiers { get; private set; }

        public string Address { get; internal set; }

        public TimeSpan Duration {
            get {
                if (State == InteractionState.Active || State == InteractionState.Held || State == InteractionState.Conferenced || State == InteractionState.Consulting)
                    return DateTime.Now - StartTime;

                return new TimeSpan();
            }
        }

        public DateTime EndTime { get; internal set; }

        public string Id {
            get {
                return _id;
            }
            set {
                _id = value;
                Guid resultId;
                if (Guid.TryParse(value, out resultId)) {
                    _internalId = resultId;
                }
            }
        }

        public string Queue { get; set; }

        public bool CanCompleteTransfer { get; set; }

        public bool ConferencedCall { get; set; }

        public string CurrentExtension { get; set; }

        public InteractionBlindType BlindCall { get; set; }

        public ScreenPopConfig ScreenPopConfiguration { get; set; }
  		
        public string CallId { get; set; }
        public bool isCallFailed { get; set; }

        public bool EnableDropLastParty { get; set; }
        public virtual IDictionary<string, string> InteractionData {
            get {
                return new Dictionary<string, string>(_interactionData);
            }
            internal set {
                if (value != null) {
                    _interactionData = new Dictionary<string, string>(value);
                    OnDataChanged();
                }
            }
        }

        public DateTime StartTime { get; set; }

        public InteractionState State {
            get {
                return _state;
            }
            set {
                if (_state != value) {
                    var oldState = _state;
                    _state = value;
                    OnPropertyChanged("State");
                    OnStateChanged(new InteractionStateChangedEventArgs(oldState, _state));
                }
            }
        }

        //public abstract MediaType Type { get; }

        public abstract MediaType Type { get; set; }

        public abstract bool IsRealTime { get; }

        protected CtiServiceProvider Provider { get; private set; }

        public virtual void AttachUserData(string key, string value) {
            _interactionData.Add(key, value);
            OnDataChanged();
        }

        public virtual void AttachUserData(IDictionary<string, string> data) {
            foreach (var item in data) {
                _interactionData.Add(item);
            }

            OnDataChanged();
        }
                
        public void HandleMessage(Message message) {
            switch (message.Type)
            {
                case Oracle.RightNow.Cti.CtiServiceProvider.Messaging.SwitchMessageType.Interaction:
                    handleInteractionMessage((InteractionMessage)message);
                    break;
                default:
                    break;
            }
        }

        public virtual void RemoveUserData(string key) {
            if (_interactionData.Remove(key))
                OnDataChanged();
        }

        public virtual void RemoveUserData(IList<string> keys) {
            foreach (var key in keys) {
                _interactionData.Remove(key);
            }
            OnDataChanged();
        }

        protected virtual void OnConnected() {
            var temp = Connected;
            if (temp != null) {
                temp(this, EventArgs.Empty);
            }
        }

        protected virtual void OnDataChanged() {
            var temp = DataChanged;
            if (temp != null) {
                temp(this, EventArgs.Empty);
            }
        }

        protected virtual void OnDisconnected() {
            var temp = Disconnected;
            if (temp != null) {
                temp(this, EventArgs.Empty);
            }
        }

        protected virtual void OnStateChanged(InteractionStateChangedEventArgs args) {
            var temp = StateChanged;
            if (temp != null) {
                temp(this, args);
            }
        }

        public virtual void Accept() {
            throw new NotImplementedException();
        }

        private InteractionState getInteractionState(SwitchInteractionState state) {
            InteractionState result = InteractionState.Active;

            switch (state)
            {
                case SwitchInteractionState.Waiting:
                case SwitchInteractionState.Completed:
                case SwitchInteractionState.Queued:

                    break;
                case SwitchInteractionState.Active:
                    result = InteractionState.Active;
                    break;
                case SwitchInteractionState.Held:
                    result = InteractionState.Held;
                    break;
                case SwitchInteractionState.Conferenced:
                    result = InteractionState.Conferenced;
                    break;
                case SwitchInteractionState.Consulting:
                    result = InteractionState.Consulting;
                    break;
                case SwitchInteractionState.Disconnected:
                    result = InteractionState.Disconnected;
                    break;
                case SwitchInteractionState.Offered:
                    result = InteractionState.Ringing;
                    break;
                case SwitchInteractionState.RingingOut:
                    result = InteractionState.RingingOut;
                    break;
                default:
                    throw new InvalidEnumArgumentException();
            }

            return result;
        }

        private void handleInteractionMessage(InteractionMessage message) {
            if (message.Interaction.Id == _internalId) {
                if (message.Action == InteractionMessageAction.StateChanged) {
                    var state = getInteractionState(message.Interaction.State);

                    State = state;

                    switch (state)
                    {
                        case InteractionState.Ringing:

                            break;
                        case InteractionState.Active:
                            OnConnected();
                            break;
                        case InteractionState.Held:
                            break;
                        case InteractionState.Disconnected:
                            EndTime = DateTime.Now;
                            OnDisconnected();
                            break;
                        case InteractionState.Dialing:
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}