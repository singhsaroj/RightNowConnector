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

using Oracle.RightNow.Cti.Model;

namespace Oracle.RightNow.Cti {
    public interface ICredentialsProvider {
        /// <summary>
        /// Retrieves the credentials associated with the current user. 
        /// The credentials retrieved will be used to authenticate the user against the switch. 
        /// </summary>
        /// <returns>An <see cref="Oracle.RightNow.Cti.Model.InteractionCredentials"/>InteractionCredentials instance with information related to the current user.</returns>
        InteractionCredentials GetCredentials();
        InteractionCredentials ReSetCredentials(string AgentID, string password, string locaiton, string queue,string AgentName="Guest");
    }
}