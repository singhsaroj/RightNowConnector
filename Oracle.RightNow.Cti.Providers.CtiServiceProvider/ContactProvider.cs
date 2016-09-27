// ===========================================================================================
//  Oracle RightNow Connect
    

//  CTI Sample Code
// ===========================================================================================
//  Copyright © Oracle Corporation.  All rights reserved.
// 
//  Sample code for training only. This sample code is provided "as is" with no warranties 
//  of any kind express or implied. Use of this sample code is pursuant to the applicable
//  non-disclosure agreement and or end user agreement and or partner agreement between
//  you and Oracle Corporation. You acknowledge Oracle Corporation is the exclusive
//  owner of the object code, source code, results, findings, ideas and any works developed
//  in using this sample code.
// ===========================================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Oracle.RightNow.Cti.Model;
using Oracle.RightNow.Cti.CtiServiceProvider;
using Oracle.RightNow.Cti.CtiServiceProvider.Messaging;
using Oracle.RightNow.Cti.CtiServiceProvider.Messaging.Messages;
using RightNow.AddIns.AddInViews;

namespace Oracle.RightNow.Cti.Providers.CtiServiceProvider {
    [Export(typeof(IContactProvider))]
    public class ContactProvider : IContactProvider, IRemoteMessagingClient {
        private readonly Device _device;
        private readonly IGlobalContext _rightNowContext;        
        private SwitchClient _client;
        private object _clientRootSyncContact = new object();
        private IList<Contact> _currentResponse;
        private AutoResetEvent _waitHandle = new AutoResetEvent(false);
        private string _currentExtension;

        [ImportingConstructor]
        public ContactProvider(IGlobalContext rightNowContext) {
            _rightNowContext = rightNowContext;
            _device = new Device {
                Address = "Contact Provider",
                Id = Guid.NewGuid(),
                Agent = new Agent {
                    Id = Guid.NewGuid(),
                    DisplayName = string.Empty,
                    IsSystemProcess = true
                }
            };

            _currentExtension = string.Format("{0}{1}{2}", _rightNowContext.InterfaceId, _rightNowContext.ProfileId, _rightNowContext.AccountId);
        }

        public SwitchClient Client {
            get {
                if (_client == null) {
                    lock (_clientRootSyncContact) {
                        if (_client == null) {
                            var client = initializeClient();
                            client.Connect(_device, true);
                            _client = client;
                        }
                    }
                }
                return _client;
            }
        }

        public IList<Contact> GetContacts() {
            var task = Task.Factory.StartNew<IList<Contact>>(() => {
                Client.Request(new SwitchMessage(SwitchMessageType.GetSwitchState) { DeviceId = _device.Id });
                _waitHandle.WaitOne();
                return _currentResponse;
            });

            return task.Result;
        }

        public void HandleMessage(Message message) {
            if (message.Type == SwitchMessageType.SwitchState) {
                _currentResponse = ((SwitchStateMessage)message).Devices
                                                                .Where(d => d.Agent != null && !d.Agent.IsSystemProcess && string.Compare(d.Address, _currentExtension) != 0)
                                                                .Select(d => new Contact {
                                                                           Name = d.Agent.DisplayName,
                                                                           Description = string.Format("{0} ({1})", d.Agent.DisplayName, d.Address),
                                                                           Number = d.Address,
                                                                           TransferType = TransferTypes.AllTransfers,
                                                                       }).ToList();
                _waitHandle.Set();
            }
        }

        private SwitchClient initializeClient() {
            var client = new SwitchClient(new InstanceContext(this));
            try {
                client.Open();
                (client as ICommunicationObject).Faulted += CtiServiceController_Faulted;
            }
            catch (EndpointNotFoundException) {
                CtiServiceSwitch.Run(_rightNowContext);
                client = initializeClient();
            }

            return client;
        }

        private void CtiServiceController_Faulted(object sender, EventArgs e)
        {            
        
        }
    }
}