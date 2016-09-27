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
using Oracle.RightNow.Cti.Model;
using Oracle.RightNow.Cti.CtiServiceProvider;
using Oracle.RightNow.Cti.CtiServiceProvider.Messaging.Messages;

namespace Oracle.RightNow.Cti.Providers.CtiServiceProvider {
    [Export(typeof(IDevice))]
    public class CtiServiceDevice : IDevice, ISwitchMessageConsumer {
        private string _address;
        private IInteractionProvider _interactionProvider;

        public event EventHandler<ExtensionChangedEventArgs> AddressChanged;
        public event EventHandler DialComplete;
        public event EventHandler Dialing;
        public event EventHandler<TransferCompletedEventArgs> TransferCompleted;
        public event EventHandler TransferStarted;

        public string Address {
            get {
                return _address;
            }
            set {
                if (string.Compare(_address, value) != 0) {
                    _address = value;
                    OnAddressChanged();
                }
            }
        }

        [Import]
        public IInteractionProvider InteractionProvider {
            get {
                return _interactionProvider;
            }
            set {
                if (_interactionProvider != value) {
                    if (_interactionProvider is ISwitchMessageProducer) {
                        ((ISwitchMessageProducer)_interactionProvider).Unsubscribe(this);
                    }

                    _interactionProvider = value;

                    if (_interactionProvider is ISwitchMessageProducer) {
                        ((ISwitchMessageProducer)_interactionProvider).Subscribe(this);
                    }
                }
            }
        }

        public SwitchClient SwitchClient {
            get {
                var provider = _interactionProvider as CtiServiceProvider;

                if (provider != null) {
                    return provider.Client;
                }
                return null;
            }
        }

        public void CancelTransfer(ICall originalCall, ICall targetCall, TransferTypes transferType) {
            
        }

        public void CompleteTransfer(ICall originalCall, ICall targetCall, TransferTypes transferType) {
            
        }

        public void Dial(string address) {
            // Not currently supported in the CtiServiceProvider.
        }

        public void InitiateTransfer(ICall call, TransferTypes transferType, string address) {
            // Here, we're only supporting one transfer type. In a real implementation,
            // that flag would indicate the type of transfer that should be performed.

            SwitchClient.Request(new InteractionTransferMessage {
                InteractionId = new Guid(call.Id),
                TargetAddress = address,
            });
        }

        public void CancelConference(ICall originalCall, ICall targetCall, TransferTypes transferType)
        {

        }

        public void CompleteConference(ICall originalCall, ICall targetCall, TransferTypes transferType)
        {

        } 

        public void InitiateConference(ICall call, TransferTypes transferType, string address)
        {
            // Here, we're only supporting one conference type. In a real implementation,
            // that flag would indicate the type of transfer that should be performed.

            SwitchClient.Request(new InteractionTransferMessage
            {
                InteractionId = new Guid(call.Id),
                TargetAddress = address,
            });
        }

        public void SendDtmf(string dtmfDigit) {
            
        }

        protected virtual void OnAddressChanged() {
            var temp = AddressChanged;
            if (temp != null) {
                temp(this, new ExtensionChangedEventArgs(Address));
            }
        }

        void ISwitchMessageConsumer.HandleMessage(Cti.CtiServiceProvider.Messaging.Messages.Message message) {
        }
    }
}