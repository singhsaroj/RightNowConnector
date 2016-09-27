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
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Threading;

namespace Oracle.RightNow.Cti.Providers.CtiServiceProvider {
    [DataContract(IsReference=true)]
    public class NotifyingObject : INotifyPropertyChanged {
      

        public event PropertyChangedEventHandler PropertyChanged;
        
        protected void OnPropertyChanged(string propertyName) {
            var args = new PropertyChangedEventArgs(propertyName);
            var handler = PropertyChanged;
            if (handler != null) {
                handler(this, args);
            }
        }
    }
}