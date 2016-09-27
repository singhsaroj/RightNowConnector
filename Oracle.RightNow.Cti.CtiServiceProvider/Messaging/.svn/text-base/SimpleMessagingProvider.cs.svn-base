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
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Oracle.RightNow.Cti.Providers.CtiServiceProvider;
using Oracle.RightNow.Cti.CtiServiceProvider.Messaging.Messages;

namespace Oracle.RightNow.Cti.CtiServiceProvider.Messaging {
    public class SimpleMessagingProvider : IMessagingProvider {
        [Import]
        public CtiServiceSwitch Switch { get; set; }
        public void Initialize() {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        public event EventHandler<SwitchMessageEventArgs> NewMessage;
        private Dictionary<Guid, Action<Message>> _subscribers = new Dictionary<Guid, Action<Message>>();

        public SimpleMessagingProvider(CtiServiceSwitch CtiServiceSwitch) {
            this.Switch = CtiServiceSwitch;
        }

        public void SendMessage(Device device, Message message) {
            Action<Message> handler;
            if (this._subscribers.TryGetValue(device.Id, out handler)) {
                Task.Factory.StartNew(d => {
                    var messageData = (MessageData)d;
                    messageData.Handler(messageData.Message);
                }, new MessageData {
                       Handler = handler,
                       Message = message
                   });
            }
        }

        public void BroadcastMessage(Message message) {
            foreach (var device in this.Switch.Devices) {
                this.SendMessage(device, message);
            }
        }

        public void Connect(Device device, Action<Message> messageHandler) {
            if (!this._subscribers.ContainsKey(device.Id)) {
                this._subscribers.Add(device.Id, messageHandler);
            }
        }

        public bool Disconnect(Device device, Action<Message> messageHandler) {
            return this._subscribers.Remove(device.Id);
        }
    }
}