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

namespace Oracle.RightNow.Cti {
    public class CustomEventArgs : EventArgs {
        public CustomEventArgs() {
            this.Name = string.Empty;
            this.Type = string.Empty;
            this.Description = string.Empty;
            this.State = string.Empty;
            this.Data = new Dictionary<string, string>();
        }

        #region Property(ies)
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public object State { get; set; }
        public IDictionary<string, string> Data { get; set; }
        #endregion
    }
}