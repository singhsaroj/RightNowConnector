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
using System.ComponentModel.Composition;
using System.Linq;
using Oracle.RightNow.Cti.Model;
using RightNow.AddIns.AddInViews;
using Oracle.RightNow.Cti.Common.ConnectService;
using Oracle.RightNow.Cti.Common;
using System.Collections.Generic;
namespace Oracle.RightNow.Cti {
    [Export(typeof(ICredentialsProvider))]
    public class CredentialsProvider : ICredentialsProvider {
        private readonly IGlobalContext _context;

        [ImportingConstructor]
        public CredentialsProvider(IGlobalContext context) {
            _context = context;
        }

        public InteractionCredentials GetCredentials() {
            // This is a simple implementation of the credentials provider.
            // Here, we're using the current user credentials but credentials could
            // come from any source, custom fields, objects or a UI prompt.
            var objectProvider = new RightNowObjectProvider(_context);
            StaffAccountInfo staffAccount = objectProvider.GetStaffAccountInformation(_context.AccountId);

            return new InteractionCredentials {
                AgentCredentials = new SwitchCredentials {
                    Id = _context.AccountId.ToString(),
                    Name = staffAccount.Name,
                    Password = staffAccount.Password,
                    AgentID=staffAccount.AgentID,
                    Extension=staffAccount.Extension,
                    Queue=staffAccount.Queue

                },
                LocationInfo = new LocationInfo {
                    ComputerName = staffAccount.Extension.ToString(),
                    DeviceAddress = staffAccount.Extension.ToString()
                }
                
                //LocationInfo = new LocationInfo {
                //    ComputerName = Environment.MachineName,
                //    DeviceAddress = string.Format("{0}{1}{2}", _context.InterfaceId, _context.ProfileId, _context.AccountId)
                //}
            };
        }

        public void SetCredentials(string AgentID, string password, string location, string AgentName = "Guest")
        {
            var objectProvider = new RightNowObjectProvider(_context);
            StaffAccountInfo staffAccount = new StaffAccountInfo();

        }

        public InteractionCredentials ReSetCredentials(string AgentID="", string password="", string location="", string queue="0",string AgentName="Guest")
        {
            // This is a simple implementation of the credentials provider.
            // Here, we're using the current user credentials but credentials could
            // come from any source, custom fields, objects or a UI prompt. 
            var objectProvider = new RightNowObjectProvider(_context);
            StaffAccountInfo staffAccount = new StaffAccountInfo();
            staffAccount.Name = AgentName;

            staffAccount.AgentID = IntTryPrase(AgentID);
            staffAccount.Password = password;
            staffAccount.Extension = IntTryPrase(location);
            staffAccount.Queue = IntTryPrase(queue);
            objectProvider.UpdateStaffAccountInformation(staffAccount);
            

            return new InteractionCredentials
            {
                AgentCredentials = new SwitchCredentials
                {
                    Id =  staffAccount.AgentID.ToString(),
                    Name = staffAccount.Name,
                    Password = staffAccount.Password,
                    AgentID = staffAccount.AgentID,
                    Extension = staffAccount.Extension,
                    Queue = staffAccount.Queue
                },
                LocationInfo = new LocationInfo
                {
                    ComputerName = location,
                    DeviceAddress = location
                }
            };
        }

        private int IntTryPrase(object value)
        {
            int val;
            int.TryParse((string)value, out val);

            return val;
        }

    }
}