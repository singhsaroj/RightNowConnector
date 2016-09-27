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

namespace Oracle.RightNow.Cti.Model {
    [Flags]
    public enum TransferTypes {
        None = 0,
        /// <summary>
        /// Single step transfer.
        /// </summary>
        SingleStep = 1,
        /// <summary>
        /// Cold transfers.
        /// </summary>
        Cold = 2,
        /// <summary>
        /// Warm transfers.
        /// </summary>
        Warm = 4,
        /// <summary>
        /// DTMF transfers.
        /// </summary>  
        Dtmf = 8,
        /// <summary>
        /// Conference.
        /// </summary>
        Conference = 16,
        /// <summary>
        /// Single step conference.
        /// </summary>
        SingleStepConference = 32,

        OutboundDialing = 64,

        ColdTransfers = Cold | SingleStep,

        Conferences = Conference | SingleStepConference,

        AllTransfers = SingleStep | Cold | Warm | Dtmf | Conference | SingleStepConference
    }
}