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
    public interface IDevice {
        event EventHandler Dialing;
        event EventHandler DialComplete;
        event EventHandler TransferStarted;
        event EventHandler<TransferCompletedEventArgs> TransferCompleted;
        event EventHandler<ExtensionChangedEventArgs> AddressChanged;

        string Address { get; set; }

        void Dial(string address);

        void InitiateTransfer(ICall call, TransferTypes transferType, string address);

        void CompleteTransfer(ICall originalCall, ICall targetCall, TransferTypes transferType);

        void CancelTransfer(ICall originalCall, ICall targetCall, TransferTypes transferType);

        void InitiateConference(ICall call, TransferTypes transferType, string address);

        void CompleteConference(ICall originalCall, ICall targetCall, TransferTypes transferType);

        void SendDtmf(string dtmfDigit);
    }
}