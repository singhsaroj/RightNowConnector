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
using System.Runtime.Serialization;
using Oracle.RightNow.Cti.Providers.CtiServiceProvider;
using Oracle.RightNow.Cti.Model;

namespace Oracle.RightNow.Cti.CtiServiceProvider {
    [DataContract]
    public class SwitchInteraction : NotifyingObject {
        private SwitchInteractionState _state;
        private Agent _agent;

        [DataMember]
        public Agent Agent {
            get {
                return _agent;
            }
            set {
                if (_agent != value) {
                    _agent = value;
                    OnPropertyChanged("Agent");
                }
            }
        }

        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public DateTime LastStateChange { get; set; }

        [DataMember]
        public IDictionary<string, string> Properties { get; set; }

        [DataMember]
        public string Queue { get; set; }

        [DataMember]
        public string ReferenceId { get; set; }

        [DataMember]
        public string SourceAddress { get; set; }

        [DataMember]
        public SwitchInteractionState State {
            get {
                return _state;
            }
            set {
                if (_state != value) {
                    _state = value;
                    LastStateChange = DateTime.Now;
                    OnPropertyChanged("State");
                }
            }
        }

        [DataMember]
        public InteractionType Type { get; set; }
      
        [DataMember]
        public bool Direction { get; set; }

        [DataMember]
        public CallType CallType { get; set; }

        [DataMember]
        public int CallID { get; set; }

        [DataMember]
        public string CurrentExtension { get; set; }

        [DataMember]
        public bool ConferencedCall { get; set; }
        
        [DataMember]
        public InteractionBlindType BlindCall { get; set; }
        
        [DataMember]
        public bool isCallFailed { get; set; }
    }
}