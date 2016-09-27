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
using Oracle.RightNow.Cti.Model;

namespace Oracle.RightNow.Cti {
    public class TransferCompletedEventArgs : EventArgs {
        public TransferCompletedEventArgs(TransferResult result)
            : this(result, string.Empty, null) {
        }


        public TransferCompletedEventArgs(TransferResult result, string description)
            : this(result, description, null) {
        }


        public TransferCompletedEventArgs(TransferResult result, string description, Exception error) {
            this.Result = result;
            this.Description = description;
            this.Error = error;
        }

        public TransferResult Result { get; set; }
        public string Description { get; set; }
        public Exception Error { get; set; }
        public TransferTypes TransferType { get; set; }
        public Contact TransferContact { get; set; }
    }
}