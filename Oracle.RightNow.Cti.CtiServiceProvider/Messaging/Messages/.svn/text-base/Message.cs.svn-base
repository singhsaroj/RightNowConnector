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
using System.Linq;
using System.Runtime.Serialization;

namespace Oracle.RightNow.Cti.CtiServiceProvider.Messaging.Messages {
    [DataContract]
    [KnownType("GetTypes")]
    public abstract class Message {
        [DataMember]
        public virtual SwitchMessageType Type { get; set; }

        public static Type[] GetTypes() {
            var messageType = typeof(Oracle.RightNow.Cti.CtiServiceProvider.Messaging.Messages.Message);
            return messageType.Assembly.GetTypes().Where(t => messageType.IsAssignableFrom(t)).ToArray();
        }
    }
}