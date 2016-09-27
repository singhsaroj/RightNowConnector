using Oracle.RightNow.Cti.CtiServiceProvider.Messaging.Messages;

namespace Oracle.RightNow.Cti.Providers.CtiServiceProvider {
    internal interface ISwitchMessageConsumer {
        void HandleMessage(Message message);
    }
}