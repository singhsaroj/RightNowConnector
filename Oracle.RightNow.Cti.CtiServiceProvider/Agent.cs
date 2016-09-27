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
using Oracle.RightNow.Cti.CtiServiceProvider;

namespace Oracle.RightNow.Cti.Providers.CtiServiceProvider {
    public class Agent : NotifyingObject {
        private Device _device;
        private Guid _id;
        private DateTime _lastStateChange;
        private SwitchAgentState _state;
        private List<SwitchInteraction> _interactions;
        private string _displayName;
        private bool _isSystemProcess;

        public Agent() {
            Interactions = new List<SwitchInteraction>();
        }

        public Device Device {
            get {
                return _device;
            }
            set {
                if (_device != value) {
                    _device = value;
                    OnPropertyChanged("Device");
                }
            }
        }

        public string DisplayName {
            get {
                return _displayName;
            }
            set {
                if (string.Compare(_displayName, value) != 0) {
                    _displayName = value;
                    OnPropertyChanged("DisplayName");
                }
            }
        }

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

        public List<SwitchInteraction> Interactions {
            get {
                return _interactions;
            }
            set {
                if (_interactions != value) {
                    _interactions = value;
                    OnPropertyChanged("Interactions");
                }
            }
        }

        public DateTime LastStateChange {
            get {
                return _lastStateChange;
            }
            set {
                if (_lastStateChange != value) {
                    _lastStateChange = value;
                    OnPropertyChanged("LastStateChange");
                }
            }
        }

        public SwitchAgentState State {
            get {
                return _state;
            }
            set {
                if (_state != value) {
                    _state = value;
                    OnPropertyChanged("State");
                }
            }
        }

        public bool IsSystemProcess {
            get {
                return _isSystemProcess;
            }
            set {
                if (_isSystemProcess != value) {
                    _isSystemProcess = value;
                    OnPropertyChanged("IsSystemProcess");
                }
            }
        }
    }
}