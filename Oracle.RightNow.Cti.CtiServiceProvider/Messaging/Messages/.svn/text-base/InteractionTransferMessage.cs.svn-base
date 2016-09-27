using System;
using System.Runtime.Serialization;

namespace Oracle.RightNow.Cti.CtiServiceProvider.Messaging.Messages {
    [DataContract]
    public class InteractionTransferMessage : Message {
        public override SwitchMessageType Type {
            get {
                return SwitchMessageType.InteractionTransfer;
            }
            set {
            }
        }

        [DataMember]
        public Guid InteractionId { get; set; }

        [DataMember]
        public string TargetAddress { get; set; }
    }
}