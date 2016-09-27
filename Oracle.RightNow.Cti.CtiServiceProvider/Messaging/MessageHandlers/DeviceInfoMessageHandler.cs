using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Oracle.RightNow.Cti.Providers.CtiServiceProvider.Messaging;
using Oracle.RightNow.Cti.CtiServiceProvider.Messaging.Messages;

namespace Oracle.RightNow.Cti.CtiServiceProvider.Messaging.MessageHandlers {
    [Export(typeof(IMessageHandler))]
    public class DeviceInfoMessageHandler : IMessageHandler {
        public void HandleMessage(CtiServiceSwitch @switch, Message message) {
            var deviceInfoMessage = message as GetDeviceInfoMessage;
            if (deviceInfoMessage != null) {
                var device = @switch.Devices.FirstOrDefault(d => d.Id == deviceInfoMessage.DeviceId);
                var targetDevice = @switch.Devices.FirstOrDefault(d => d.Id == deviceInfoMessage.TargetDeviceId);
                var response = new DeviceInfoMessage { TargetDevice = targetDevice };

                @switch.SendMessage(device, response);
            }
        }

        public SwitchMessageType MessageType {
            get {
                return SwitchMessageType.GetDeviceInfo;
            }
        }
    }
}
