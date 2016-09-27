using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using Oracle.RightNow.Cti.Providers.CtiServiceProvider;
using Oracle.RightNow.Cti.CtiServiceProvider.Messaging.Messages;


namespace Oracle.RightNow.Cti.CtiServiceProvider.Messaging {
    [ServiceContract(CallbackContract = typeof(IRemoteMessagingClient), SessionMode = SessionMode.Required)]
    public interface IRemoteMessagingProvider {
        [OperationContract(IsInitiating = true)]
        void Connect(Device device, bool enableGlobalSubscription);

        [OperationContract(IsTerminating = true)]
        void Disconnect(Device device);

        [OperationContract(IsOneWay = true)]
        void Request(Message message);
    }

    public interface IRemoteMessagingClient {
        [OperationContract(IsOneWay = true)]
        void HandleMessage(Message message);
    }
}

namespace Oracle.RightNow.Cti.CtiServiceProvider.Messaging {
    [Export(typeof(IMessagingProvider))]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class RemoteMessagingProvider : IRemoteMessagingProvider, IMessagingProvider {
        private Dictionary<Guid, IRemoteMessagingClient> _clients = new Dictionary<Guid, IRemoteMessagingClient>();
        private ServiceHost _serviceHost;
         
        public event EventHandler<SwitchMessageEventArgs> NewMessage;
        public RemoteMessagingProvider() {  
        }

        [Import]
        public CtiServiceSwitch Switch { get; set; }

        public void Initialize() {
            _serviceHost = new ServiceHost(this);
            _serviceHost.AddServiceEndpoint(typeof(IRemoteMessagingProvider), new NetNamedPipeBinding(), "net.pipe://localhost/oraclerightnow/CtiServiceswitch");
            _serviceHost.Open();
        }

        public void Connect(Device device, bool enableGlobalSubscription) {
            var client = OperationContext.Current.GetCallbackChannel<IRemoteMessagingClient>();
            if (_clients.ContainsKey(device.Id))
                _clients.Remove(device.Id);

            _clients.Add(device.Id, client);

            Switch.HandleMessage(new ConnectMessage { Device = device, EnableGlobalSubscription = enableGlobalSubscription });
        }

        public void Disconnect(Device device) {
            if (_clients.ContainsKey(device.Id)) {
                var client = _clients[device.Id];

                var message = new SwitchMessage(SwitchMessageType.Disconnected){ DeviceId = device.Id};
                
                client.HandleMessage(message);
                _clients.Remove(device.Id);
                
                Switch.HandleMessage(message);
            }
        }

        public void SendMessage(Device device, Message message) {
            var client = getClient(device.Id);

            if (client != null) {
                try {
                    client.HandleMessage(message);
                }
                catch (CommunicationException) {
                    _clients.Remove(device.Id);
                }
            }
        }

        private IRemoteMessagingClient getClient(Guid id) {
            if (_clients.ContainsKey(id)) {
                return _clients[id];
            }

            return null;
        }

        public void BroadcastMessage(Message message) {
            foreach (var client in _clients.Values) {
                client.HandleMessage(message);
            }
        }

        public void Request(Message message) {
            Switch.HandleMessage(message);
        }
    }
}