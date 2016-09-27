using Oracle.RightNow.Cti.CtiServiceProvider.Messaging.Messages;

namespace Oracle.RightNow.Cti.CtiServiceProvider {
    internal interface INotificationClient {
        void HandleMessage(Message message);
    }
}