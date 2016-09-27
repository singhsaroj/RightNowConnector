using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Oracle.RightNow.Cti.Providers.CtiServiceProvider;
using Oracle.RightNow.Cti.Providers.CtiServiceProvider.Messaging;
using Oracle.RightNow.Cti.CtiServiceProvider.Messaging.Messages;

namespace Oracle.RightNow.Cti.CtiServiceProvider.Messaging.MessageHandlers {
    [Export(typeof(IMessageHandler))]
    public class GetSwitchStateMessageHandler : IMessageHandler {
        public void HandleMessage(CtiServiceSwitch @switch, Message message) {
            var switchMessage = message as SwitchMessage;
            if (switchMessage != null) {
                var device = @switch.Devices.FirstOrDefault(d => d.Id == switchMessage.DeviceId);

                var response = new SwitchStateMessage { Devices = new List<Device>(@switch.Devices) };

                @switch.SendMessage(device, response);
            }
        }

        public SwitchMessageType MessageType {
            get {
                return SwitchMessageType.GetSwitchState;
            }
        }
    }
}