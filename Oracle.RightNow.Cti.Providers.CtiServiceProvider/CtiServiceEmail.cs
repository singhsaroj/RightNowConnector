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
using System.ComponentModel;
using Oracle.RightNow.Cti.Model;

namespace Oracle.RightNow.Cti.Providers.CtiServiceProvider {
    public class CtiServiceEmail : CtiServiceInteraction, IEmail {
        public CtiServiceEmail(CtiServiceProvider provider) : base(provider) {
        }

        public override MediaType Type {
            get {
                return MediaType.Email;
            }
            set { }
        }

        public override bool IsRealTime { get { return false; } }

        public bool CanCompleteTransfer { get; set; }

        public bool ConferencedCall { get; set; }

        public string CallId { get; set; }

        public bool EnableDropLastParty { get; set; }
        
        public string CurrentExtension { get; set; }

        public InteractionBlindType BlindCall { get; set; }

        bool isCallFailed { get; set; }
    }
}