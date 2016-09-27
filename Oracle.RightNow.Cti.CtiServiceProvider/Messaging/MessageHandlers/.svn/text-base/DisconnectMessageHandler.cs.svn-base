using System.ComponentModel.Composition;
using System.Linq;
using Oracle.RightNow.Cti.Providers.CtiServiceProvider.Messaging;
using Oracle.RightNow.Cti.CtiServiceProvider.Messaging.Messages;

namespace Oracle.RightNow.Cti.CtiServiceProvider.Messaging.MessageHandlers {
    [Export(typeof(IMessageHandler))]
    class DisconnectMessageHandler : IMessageHandler {
        public void HandleMessage(CtiServiceSwitch @switch, Message message) {
            var disconnectMessage = message as SwitchMessage;
            if (disconnectMessage != null) {
                @switch.SendMessage(null, disconnectMessage);

                var device = @switch.Devices.FirstOrDefault(d=>d.Id == disconnectMessage.DeviceId);
                if (device != null)
                    @switch.Devices.Remove(device);
            }
        }

        public SwitchMessageType MessageType {
            get {
                return SwitchMessageType.Disconnected;
            }
        }
    }
}