using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Oracle.RightNow.Cti.Providers.CtiServiceProvider.Messaging;
using Oracle.RightNow.Cti.CtiServiceProvider.Messaging.Messages;

namespace Oracle.RightNow.Cti.CtiServiceProvider.Messaging.MessageHandlers {
    [Export(typeof(IMessageHandler))]
    public class CreateInteractionMessageHandler : IMessageHandler {
        public void HandleMessage(CtiServiceSwitch @switch, Message message) {
            var createInteractionMessage = message as CreateInteractionMessage;
            if (createInteractionMessage != null) {
                @switch.QueueInteraction(createInteractionMessage.Interaction);
            }
        }

        public SwitchMessageType MessageType {
            get {
                return SwitchMessageType.CreateInteraction;
            }
        }
    }
}
