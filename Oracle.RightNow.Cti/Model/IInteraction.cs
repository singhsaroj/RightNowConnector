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

namespace Oracle.RightNow.Cti.Model {
    public interface IInteraction : INotifyPropertyChanged {
        event EventHandler Connected;
        event EventHandler Disconnected;
        event EventHandler DataChanged;
        event EventHandler<InteractionStateChangedEventArgs> StateChanged;

        void Accept();

        string Id { get; }
        string Address { get; }
        string Queue { get; }
        DateTime StartTime { get; set;  }
        DateTime EndTime { get; }
        TimeSpan Duration { get; }
        MediaType Type { get; set; }
        //MediaType Type { get; }
        IDictionary<string, string> AdditionalIdentifiers { get; }
        IDictionary<string, string> InteractionData { get; }
        InteractionState State { get; set; }
        bool IsRealTime { get; }
        bool CanCompleteTransfer { get; set; }
        bool ConferencedCall { get; set; }
        ScreenPopConfig ScreenPopConfiguration { get; set; }
		string CallId { get; set; }
        bool EnableDropLastParty { get; set; }
        string CurrentExtension { get; set; }
        InteractionBlindType BlindCall { get; set; }
        bool isCallFailed { get; set; }
    }
}