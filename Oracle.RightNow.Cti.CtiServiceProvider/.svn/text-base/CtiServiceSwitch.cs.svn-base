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
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.RightNow.Cti.Providers.CtiServiceProvider;
using Oracle.RightNow.Cti.Providers.CtiServiceProvider.Messaging;
using Oracle.RightNow.Cti.CtiServiceProvider;
using Oracle.RightNow.Cti.CtiServiceProvider.Messaging;
using Oracle.RightNow.Cti.CtiServiceProvider.Messaging.Messages;
using RightNow.AddIns.AddInViews;

namespace Oracle.RightNow.Cti.CtiServiceProvider {
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class CtiServiceSwitch {
        private static CompositionContainer _container;

        [Import]
        private IMessagingProvider _messagingHandler;

        public CtiServiceSwitch() {
            MessageHandlers = new List<IMessageHandler>();
            Devices = new List<Device>();
            GlobalSubscribers = new List<Device>();
            Interactions = new List<SwitchInteraction>();
            NotificationClients = new List<INotificationClient>();
        }

        public List<Device> Devices { get; set; }

        public List<Device> GlobalSubscribers { get; set; }

        public List<SwitchInteraction> Interactions { get; set; }

        internal List<INotificationClient> NotificationClients { get; set; }

        [ImportMany]
        ICollection<IMessageHandler> MessageHandlers { get; set; }

        public static void Run(IGlobalContext rightNowGlobalContext) {
            var catalog = new AssemblyCatalog(typeof(CtiServiceSwitch).Assembly);
            
            _container = new CompositionContainer(catalog);
            
            _container.ComposeParts();
            _container.ComposeExportedValue(rightNowGlobalContext);

            _container.GetExport<CtiServiceSwitch>().Value._messagingHandler.Initialize();

            _container.GetExport<IncidentProcessor>().Value.Initialize();
           // _container.GetExport<TwilioVoiceProcessor>().Value.Initialize();
        }

        public void AssignInteraction(Device device, SwitchInteraction interaction = null) {
            if (interaction == null) {
                Func<SwitchInteraction, bool> predicate = null;

                if (device.Agent.Interactions.Any(i => i.Type == InteractionType.Call))
                    predicate = i => i.State == SwitchInteractionState.Queued && i.Type != InteractionType.Call;
                else
                    predicate = i => i.State == SwitchInteractionState.Queued;

                interaction = Interactions.Where(predicate)
                                          .OrderBy(i => i.LastStateChange)
                                          .FirstOrDefault();
            }

            if (interaction != null) {
                interaction.Agent = device.Agent;
                device.Agent.Interactions.Add(interaction);
                // Send the interaction
                SendMessage(device, new InteractionMessage() {
                    Interaction = interaction,
                    Action = InteractionMessageAction.Created
                }, false);

                interaction.State = interaction.Type == InteractionType.Call
                    ? SwitchInteractionState.Offered
                    : SwitchInteractionState.Active;
                
                SendMessage(device, new InteractionMessage {
                    Interaction = interaction,
                    Action = InteractionMessageAction.StateChanged
                });

                if (interaction.State == SwitchInteractionState.Active)
                    SendMessage(device, new AgentStateMessage {
                        AgentId = device.Agent.Id,
                        DeviceId = device.Id,
                        State = SwitchAgentState.HandlingInteraction,
                    });
            }
        }

        public void QueueInteraction(SwitchInteraction interaction) {
            interaction.State = SwitchInteractionState.Queued;
            Interactions.Add(interaction);
           
            var message = new InteractionMessage {
                Interaction = interaction,
                Action = InteractionMessageAction.Queued
            };

            Parallel.ForEach(GlobalSubscribers, d => _messagingHandler.SendMessage(d, message));

            AssignQueuedInteraction(interaction);
        }

        internal void AssignQueuedInteraction(SwitchInteraction interaction) {
            if (interaction == null)
                return;

            Func<Device, bool> predicate = null;

            if (interaction.Type == InteractionType.Call)
                predicate = d => (d.Agent.State == SwitchAgentState.Available || d.Agent.State == SwitchAgentState.Busy || d.Agent.State == SwitchAgentState.WrapUp) && !d.Agent.Interactions.Any(i => i.Type == InteractionType.Call);
            else
                predicate = d => d.Agent.State == SwitchAgentState.Available;

            var device = Devices.Where(predicate)
                                .OrderBy(d => d.Agent.LastStateChange)
                                .FirstOrDefault();

            if (device != null) {
                AssignInteraction(device, interaction);
            }
        }

        internal void BroadcastMessage(Device device, Message message) {
            _messagingHandler.BroadcastMessage(message);
            Parallel.ForEach(NotificationClients, c => c.HandleMessage(message));
        }

        internal void HandleMessage(Message message) {
            var messageHandler = MessageHandlers.FirstOrDefault(h => h.MessageType == message.Type);
            if (messageHandler != null) {
                Task.Factory.StartNew(() => messageHandler.HandleMessage(this, message));
            }
        }

        internal void SendMessage(Device device, Message message, bool notifyGlobalSubscribers = true) {
            if (device != null && !GlobalSubscribers.Any(d => d.Id == device.Id))
                _messagingHandler.SendMessage(device, message);
            
            Parallel.ForEach(GlobalSubscribers, d => _messagingHandler.SendMessage(d, message));
            Parallel.ForEach(NotificationClients, c => c.HandleMessage(message));
        }
    }
}