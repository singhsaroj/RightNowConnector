using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Oracle.RightNow.Cti.Providers.CtiServiceProvider;

namespace Oracle.RightNow.Cti.CtiServiceProvider.Messaging.Messages {
    [DataContract]
    public class DeviceInfoMessage : Message {
        [DataMember]
        public Device TargetDevice { get; set; }

        public override SwitchMessageType Type {
            get {
                return SwitchMessageType.DeviceInfo;
            }
            set {
                //
            }
        }
    }
}
