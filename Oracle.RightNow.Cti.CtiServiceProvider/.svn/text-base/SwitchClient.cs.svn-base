using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using Oracle.RightNow.Cti.CtiServiceProvider.Messaging;

namespace Oracle.RightNow.Cti.CtiServiceProvider {
    public class SwitchClient : System.ServiceModel.DuplexClientBase<IRemoteMessagingProvider>, IRemoteMessagingProvider {
        private Timer _keepAliveTimer;
        public static bool _isFaulted;
        public SwitchClient(InstanceContext callbackInstance) : base(callbackInstance, GetBinding(), new EndpointAddress("net.pipe://localhost/oraclerightnow/CtiServiceswitch")) {
            Id = Guid.NewGuid();
        }
               
        public SwitchClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : base(callbackInstance, endpointConfigurationName) {
        }

        public SwitchClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }

        public SwitchClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }

        public SwitchClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : base(callbackInstance, binding, remoteAddress) {
        }

        private static Binding GetBinding() {
            var binding = new CustomBinding(new NetNamedPipeBinding() { ReceiveTimeout = TimeSpan.FromDays(1) });
            return binding;
        }

        public void Connect(Oracle.RightNow.Cti.Providers.CtiServiceProvider.Device device, bool enableGlobalSubscription = false) {
            Channel.Connect(device, enableGlobalSubscription);
            (this as ICommunicationObject).Faulted -= SwitchClient_Faulted;
            (this as ICommunicationObject).Faulted += SwitchClient_Faulted;
            _keepAliveTimer = new Timer(ping, null, 30000, 30000);
        }

        void SwitchClient_Faulted(object sender, EventArgs e)
        {            
            if (!_isFaulted)
            {
                _isFaulted = true;
                System.Windows.Forms.MessageBox.Show("Connection with CTI lost, Please restart the application.");
                Logger.Logger.Log.Error("Application Faulted");
                System.Windows.Forms.Application.Exit();
            }
        }
   
        private void ping(object state) {
            Request(new Oracle.RightNow.Cti.CtiServiceProvider.Messaging.Messages.SwitchMessage(SwitchMessageType.KeepAlive));
        }
     
        public void Disconnect(Oracle.RightNow.Cti.Providers.CtiServiceProvider.Device device) {
            if (_keepAliveTimer != null)
            {
                _keepAliveTimer.Change(Timeout.Infinite, Timeout.Infinite);
                _keepAliveTimer.Dispose();
                _keepAliveTimer = null;
            }
            Channel.Disconnect(device);
        }

        public void Request(Oracle.RightNow.Cti.CtiServiceProvider.Messaging.Messages.Message message) {
            if ((this as ICommunicationObject).State == CommunicationState.Opened)
                Channel.Request(message);
            
        }

        public Guid Id { get; set; }
    }
}