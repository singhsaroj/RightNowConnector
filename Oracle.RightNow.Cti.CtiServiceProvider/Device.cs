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
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Oracle.RightNow.Cti.Providers.CtiServiceProvider {
    [DataContract(IsReference=true)]
    public class Device : NotifyingObject {
        private string _address;
        private Agent _agent;
        private Guid _id;

        [DataMember]
        public string Address {
            get {
                return _address;
            }
            set {
                if (string.Compare(_address, value) != 0) {
                    _address = value;
                    OnPropertyChanged("Address");
                }
            }
        }

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
        public Guid Id {
            get {
                return _id;
            }
            set {
                if (_id != value) {
                    _id = value;
                    OnPropertyChanged("Id");
                }
            }
        }
    }
}