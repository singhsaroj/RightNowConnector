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
using System.Linq;
using System.Text;
using Oracle.RightNow.Cti.Providers.CtiServiceProvider;
using Oracle.RightNow.Cti.Providers.CtiServiceProvider.Messaging;
using Oracle.RightNow.Cti.CtiServiceProvider.Messaging.Messages;

namespace Oracle.RightNow.Cti.CtiServiceProvider.Messaging.MessageHandlers {
    [Export(typeof(IMessageHandler))]
    public class InteractionTransferMessageHandler : IMessageHandler {
        public SwitchMessageType MessageType {
            get {
                return SwitchMessageType.InteractionTransfer;
            }
        }

        public void HandleMessage(CtiServiceSwitch @switch, Message message) {
            var interactionTransferMessage = message as InteractionTransferMessage;
            if (interactionTransferMessage != null) {
                var interaction = @switch.Interactions.FirstOrDefault(i => i.Id == interactionTransferMessage.InteractionId);

                var transferInteraction = new SwitchInteraction {
                    Id = Guid.NewGuid(),
                    State = SwitchInteractionState.Queued,
                    SourceAddress = interaction.SourceAddress,
                    ReferenceId = interaction.ReferenceId,
                    Queue = interaction.Queue,
                    Properties = interaction.Properties,
                    Type = interaction.Type,
                    ConferencedCall=interaction.ConferencedCall
                };

                var originalTransfers = transferInteraction.Properties.Where(p => p.Key.StartsWith("OriginalInteraction-")).Count();
                transferInteraction.Properties.Add(string.Format("OriginalInteraction-{0}", originalTransfers), interaction.Id.ToString());

                if (interactionTransferMessage.TargetAddress == "queue") {
                    @switch.QueueInteraction(transferInteraction);
                }
                else {
                    var device = @switch.Devices.FirstOrDefault(d => d.Address == interactionTransferMessage.TargetAddress);

                    if (device != null) {
                        @switch.Interactions.Add(transferInteraction);
                        @switch.AssignInteraction(device, transferInteraction);
                    }
                }

                notifyInteractionState(@switch, interaction, SwitchInteractionState.Disconnected);

                interaction.Agent.State = SwitchAgentState.WrapUp;
                notifyAcw(interaction, @switch);
            }

            var deviceInfoMessage = message as GetDeviceInfoMessage;
            if (deviceInfoMessage != null) {
                var device = @switch.Devices.FirstOrDefault(d => d.Id == deviceInfoMessage.DeviceId);
                var targetDevice = @switch.Devices.FirstOrDefault(d => d.Id == deviceInfoMessage.TargetDeviceId);
                var response = new DeviceInfoMessage { TargetDevice = targetDevice };

                @switch.SendMessage(device, response);
            }
        }

        private void notifyAcw(SwitchInteraction interaction, CtiServiceSwitch @switch) {
            //var agentStateMessage = new AgentStateMessage {
            //    AgentId = interaction.Agent.Id,
            //    DeviceId = interaction.Agent.Device.Id,
            //    State = SwitchAgentState.WrapUp,
            //};
            var agentStateMessage = new AgentStateMessage
            {
                AgentId = interaction.Agent.Id,
                DeviceId = interaction.Agent.Device.Id,
                State = SwitchAgentState.Available,
            };
            @switch.SendMessage(interaction.Agent.Device, agentStateMessage);
        }

        private void notifyInteractionState(CtiServiceSwitch @switch, SwitchInteraction interaction, SwitchInteractionState state) {
            interaction.State = state;
            @switch.SendMessage(interaction.Agent.Device, new InteractionMessage {
                Interaction = interaction,
                Action = InteractionMessageAction.StateChanged
            });
        }
    }
}