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

namespace Oracle.RightNow.Cti {
    public class LoginErrorEventArgs : EventArgs {
        public LoginErrorEventArgs(string friendlyMessage, int code, Exception error) {
            this.FriendlyMessage = friendlyMessage;
            this.Code = code;
            this.Error = error;
        }

        public int Code { get; set; }
        public string FriendlyMessage { get; set; }
        public Exception Error { get; set; }
    }
}