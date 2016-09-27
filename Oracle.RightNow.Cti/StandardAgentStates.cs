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
    public static class StandardAgentStates {
        public static AgentState Available = new AgentState(-1, "Available to receive interactions", AgentSwitchMode.Ready, "3", true, "Available");        
        public static AgentState Default = new AgentState(-2, "Default", AgentSwitchMode.NotReady, "0", true, "Not Ready");
        public static AgentState WrapUp = new AgentState(-3, "Wrap up work mode", AgentSwitchMode.WrapUp, "4", true, "Wrap up");
        public static AgentState LoggedOut = new AgentState(-4, "Logged out of the phone", AgentSwitchMode.LoggedOut, "1", false, "Logged out");
        public static AgentState LoggedIn = new AgentState(-5, "Logged into the phone", AgentSwitchMode.LoggedIn, "0", false, "Logged in");
        public static AgentState InCall = new AgentState(-6, "Currently handling an interaction", AgentSwitchMode.HandlingInteraction, "-1", false, "Handling interaction");
        public static AgentState Calling = new AgentState(-8, "Making an outbound call", AgentSwitchMode.NotReady, "-1", false, "Calling");
        public static AgentState Unknown = new AgentState(-9, "Unknown agent state", AgentSwitchMode.NewReason, string.Empty, false, "Unknown");
        public static AgentState NotReady = new AgentState(-10, "Not Ready", AgentSwitchMode.NotReady, "2", true, "Not Ready");
        public static AgentState NewReason = new AgentState(-11, "New Reason for Not Ready", AgentSwitchMode.NewReason, "2", true, "New Reason");
    }
}