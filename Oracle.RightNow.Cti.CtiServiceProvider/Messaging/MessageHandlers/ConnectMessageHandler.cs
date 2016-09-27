using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Oracle.RightNow.Cti.Providers.CtiServiceProvider.Messaging;
using Oracle.RightNow.Cti.CtiServiceProvider.Messaging.Messages;

namespace Oracle.RightNow.Cti.CtiServiceProvider.Messaging.MessageHandlers {
    [Export(typeof(IMessageHandler))]
    class ConnectMessageHandler : IMessageHandler {
        public void HandleMessage(CtiServiceSwitch @switch, Message message) {
            var connectMessage = message as ConnectMessage;
            if (connectMessage != null) {
                var device = @switch.Devices.FirstOrDefault(d => d.Id == connectMessage.Device.Id);

                if (device != null) {
                    @switch.SendMessage(device, new SwitchMessage(SwitchMessageType.Disconnected));
                }
                
                @switch.Devices.Add(connectMessage.Device);

                if (connectMessage.EnableGlobalSubscription)
                    @switch.GlobalSubscribers.Add(connectMessage.Device);

                connectMessage.Device.Agent.State = SwitchAgentState.LoggedIn;
                connectMessage.Device.Agent.Device = connectMessage.Device;

                var agentStateMessage = new AgentStateMessage{
                    State = connectMessage.Device.Agent.State,
                    AgentId = connectMessage.Device.Agent.Id,
                    DeviceId = connectMessage.Device.Id,
                };

                @switch.SendMessage(connectMessage.Device, agentStateMessage);
            }
        }

        public SwitchMessageType MessageType {
            get {
                return SwitchMessageType.Connect;
            }
        }
    }
}
