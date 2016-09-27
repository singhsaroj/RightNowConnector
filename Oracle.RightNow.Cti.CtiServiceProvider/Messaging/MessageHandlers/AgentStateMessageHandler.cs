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
using Oracle.RightNow.Cti.Providers.CtiServiceProvider.Messaging;
using Oracle.RightNow.Cti.CtiServiceProvider.Messaging.Messages;

namespace Oracle.RightNow.Cti.CtiServiceProvider.Messaging.MessageHandlers {
    [Export(typeof(IMessageHandler))]
    public class AgentStateMessageHandler : IMessageHandler {
        public SwitchMessageType MessageType {
            get {
                return SwitchMessageType.AgentState;
            }
        }

        public void HandleMessage(CtiServiceSwitch @switch, Message message) {
            var stateMessage = message as AgentStateMessage;
            if (stateMessage != null) {
                var device = @switch.Devices.FirstOrDefault(d => d.Id == stateMessage.DeviceId && d.Agent.Id == stateMessage.AgentId);

                device.Agent.State = stateMessage.State;
                @switch.SendMessage(device, message);

                if (device.Agent.State == SwitchAgentState.Available)
                    @switch.AssignInteraction(device);
            }
        }
    }
}