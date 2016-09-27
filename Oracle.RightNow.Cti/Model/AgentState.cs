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
  

using Oracle.RightNow.Cti.Common.ConnectService;
using System;

namespace Oracle.RightNow.Cti.Model {
    [RightNowCustomObject(OracleCtiObjectStrings.ScreenPopPackageName, OracleCtiObjectStrings.AgentState)]
    public class AgentState:NotifyingObject {
        public AgentState() {

        }

        public AgentState NewAgentState(AgentState obj)
        {
            return new AgentState(obj.Id, obj.Description, obj.SwitchMode, obj.Code, obj.AgentSelectable, obj.Name, obj.IsOutboundEnabled);
        }

        public AgentState(int id, string description, AgentSwitchMode ctiMode, string code, bool agentSelectable, string name = null, bool outboundEnabled = false) {
            this.Id = id;
            this.Description = description;
            this.SwitchMode = ctiMode;
            this.Code = code;
            this.AgentSelectable = agentSelectable;
            this.IsOutboundEnabled = outboundEnabled;
            this.Name = name ?? description;
        }

        #region Property(ies)
        [RightNowCustomObjectField("ID")]
        public int Id { get; set; }
        
        [RightNowCustomObjectField("Name")]
        public string Name { get; set; }

        [RightNowCustomObjectField("DefaultName")]
        public string DefaultName { get; set; }

        private string _description;

        [RightNowCustomObjectField("Description")]
        public string Description
        {
            get
            {
                return this._description;
            }
            set
            {
                this._description = value; OnPropertyChanged("Description");
            }
        }
        

        [RightNowCustomObjectField("IsOutboundEnabled")]
        public bool IsOutboundEnabled { get; set; }

        [RightNowCustomObjectField("Code")]
        public string Code { get; set; }

        [RightNowCustomObjectField("SwitchMode")]
        public AgentSwitchMode SwitchMode { get; set; }

        [RightNowCustomObjectField("AgentSelectable")]
        public bool AgentSelectable { get; set; }
        #endregion Property(ies)

        public override bool Equals(object obj) {
            if (ReferenceEquals(this, obj))
                return true;

            if (obj == null || obj.GetType() != typeof(AgentState))
                return false;

            var other = (AgentState)obj;
            return String.Compare(Code, other.Code) == 0 &&
                SwitchMode == other.SwitchMode && string.Compare(Description,other.Description)==0;
        }

        public override int GetHashCode() {
            return (Code + SwitchMode.ToString("G")).GetHashCode();
        }
    }

    internal static class OracleCtiObjectStrings {
        public const string PackageName = "Oracle_Cti";
        public const string AgentState = "AgentState";
        public const string ScreenPopPackageName = "Avaya_Cti";
        public const string LoginCredential = "LoginCredential";
        public const string ScreenPopConfig = "ScreenPopConfig";
        public const string ScreenPopOptions = "ScreenPopValues";
    }

    public class RightNowCustomObjectAttribute : Attribute {
        public RightNowCustomObjectAttribute(string packageName, string objectName) {
            ObjectName = objectName;
            PackageName = packageName;
        }

        public string PackageName { get; set; }
        public string ObjectName { get; set; }
    }

    public class RightNowCustomObjectFieldAttribute : Attribute {
        public RightNowCustomObjectFieldAttribute(string name) {
            Name = name;
        }

        public RightNowCustomObjectFieldAttribute(string name, bool canUpdate)
        {
            CanUpdate = canUpdate;
            Name = name;
        }

        public RightNowCustomObjectFieldAttribute(string name, ItemsChoiceType type)
        {
            CanUpdate = true;
            FieldType = type;
            Name = name;
        }
        public string Name { get; set; }

        public ItemsChoiceType FieldType { get; set; }

        public bool CanUpdate { get; set; }
    }
}