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
using Oracle.RightNow.Cti.Providers.CtiServiceProvider;
using Oracle.RightNow.Cti.Providers.CtiServiceProvider.Messaging;
using Oracle.RightNow.Cti.CtiServiceProvider.Messaging.Messages;

namespace Oracle.RightNow.Cti.CtiServiceProvider.Messaging.MessageHandlers {
    [Export(typeof(IMessageHandler))]
    public class InteractionRequestMessageHandler : IMessageHandler {

        static SwitchInteraction sInteraction;
        public void HandleMessage(CtiServiceSwitch @switch, Message message) {           
                var stateMessage = message as InteractionRequestMessage;
                if (stateMessage != null)
                {
                    var interaction = @switch.Interactions.FirstOrDefault(i => i.Id == stateMessage.InteractionId);

                    switch (stateMessage.Action)
                    {
                        case InteractionRequestAction.Accept:
                            if (interaction.Agent != null)
                            {
                                sInteraction = interaction;
                            }
                            else
                            {
                                interaction.Agent = sInteraction.Agent;
                            }
                            interaction.Agent.State = SwitchAgentState.HandlingInteraction;
                            notifyAgentState(interaction.Agent.Device, SwitchAgentState.HandlingInteraction, @switch);

                            interaction.State = SwitchInteractionState.Active;
                            @switch.SendMessage(interaction.Agent.Device, new InteractionMessage
                            {
                                Interaction = interaction,
                                Action = InteractionMessageAction.Assigned
                            });
                            break;
                        case InteractionRequestAction.Hold:
                            notifyInteractionState(@switch, interaction, SwitchInteractionState.Held);
                            break;
                        case InteractionRequestAction.Conferenced:
                            notifyInteractionState(@switch, interaction, SwitchInteractionState.Conferenced);
                            break;
                        case InteractionRequestAction.Consulting:
                            notifyInteractionState(@switch, interaction, SwitchInteractionState.Consulting);                            
                            break;
                        case InteractionRequestAction.Retrieve:                            
                            notifyInteractionState(@switch, interaction, SwitchInteractionState.Active);
                            break;
                        case InteractionRequestAction.Disconnect:
                      
                            interaction.State = Providers.CtiServiceProvider.SwitchInteractionState.Disconnected;
                            notifyInteractionState(@switch, interaction, SwitchInteractionState.Disconnected);

                            //interaction.Agent.State = SwitchAgentState.Available;
                            //notifyAgentState(interaction.Agent.Device, SwitchAgentState.Available, @switch);
                            break;
                        case InteractionRequestAction.Complete:
                            if (interaction.State != SwitchInteractionState.Disconnected)
                            {
                                interaction.Agent.State = SwitchAgentState.Available;
                                notifyAgentState(interaction.Agent.Device, SwitchAgentState.Available, @switch);
                            }

                            interaction.Agent.Interactions.Remove(interaction);
                            @switch.Interactions.Remove(interaction);
                            notifyInteractionState(@switch, interaction, SwitchInteractionState.Completed);
                            break;
                        default:
                            break;
                    }
                }          
        }
  
        private void notifyAgentState(Device device, SwitchAgentState state, CtiServiceSwitch @switch) {
            var agentStateMessage = new AgentStateMessage {
                AgentId = device.Agent.Id,
                DeviceId = device.Id,
                State = state,
            };

            @switch.SendMessage(device, agentStateMessage);
        }

        private void notifyInteractionState(CtiServiceSwitch @switch, SwitchInteraction interaction, SwitchInteractionState state) {
            interaction.State = state;
            if(interaction.Agent!=null)
            @switch.SendMessage(interaction.Agent.Device, new InteractionMessage { Interaction = interaction, Action = InteractionMessageAction.StateChanged });
        }

        public SwitchMessageType MessageType {
            get {
                return SwitchMessageType.InteractionRequest;
            }
        }
    }
}