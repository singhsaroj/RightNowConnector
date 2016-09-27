using Oracle.RightNow.Cti.CtiServiceProvider;
using Oracle.RightNow.Cti.CtiServiceProvider.Messaging;
using Oracle.RightNow.Cti.CtiServiceProvider.Messaging.Messages;

namespace Oracle.RightNow.Cti.Providers.CtiServiceProvider.Messaging {
    public interface IMessageHandler {
        void HandleMessage(CtiServiceSwitch @switch, Message message);

        SwitchMessageType MessageType { get; }
    }
}