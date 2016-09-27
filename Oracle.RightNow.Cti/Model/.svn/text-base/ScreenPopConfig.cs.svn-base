using Oracle.RightNow.Cti.Common;
using Oracle.RightNow.Cti.Common.ConnectService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oracle.RightNow.Cti.Model
{
    [RightNowCustomObjectAttribute(OracleCtiObjectStrings.ScreenPopPackageName, OracleCtiObjectStrings.ScreenPopConfig)]
    public class ScreenPopConfig
    {
        public ScreenPopConfig()
        {
        }

        #region Property(ies)
        [RightNowCustomObjectField("ID",false)]
        public long ID { get; set; }

        [RightNowCustomObjectField("ProfileID", ItemsChoiceType.IntegerValue)]
        public int ProfileId { get; set; }

        [RightNowCustomObjectField("CanScreenPop", ItemsChoiceType.BooleanValue)]
        public bool CanScreenPop { get; set; }

        [RightNowCustomObjectField("DefaultWorkspace", ItemsChoiceType.StringValue)]
        public string DefaultWorkSpace { get; set; }

        [RightNowCustomObjectField("ChatDefaultWorkspace", ItemsChoiceType.StringValue)]
        public string ChatDefaultWorkspace { get; set; }

        [RightNowCustomObjectField("IncidentDefaultWorkspace", ItemsChoiceType.StringValue)]
        public string IncidentDefaultWorkspace { get; set; }

        [RightNowCustomObjectField("WrapupReasonEnabled", ItemsChoiceType.BooleanValue)]
        public bool WrapupReasonEnabled { get; set; }

        [RightNowCustomObjectField("LogoutReasonEnabled", ItemsChoiceType.BooleanValue)]
        public bool LogoutReasonEnabled { get; set; }

        [RightNowCustomObjectField("AUXReasonEnabled", ItemsChoiceType.BooleanValue)]
        public bool AUXReasonEnabled { get; set; }

        [RightNowCustomObjectField("IsDefault", ItemsChoiceType.BooleanValue)]
        public bool IsDefault { get; set; }

        [RightNowCustomObjectField("CanOpen", ItemsChoiceType.BooleanValue)]
        public bool CanOpen { get; set; }

        [RightNowCustomObjectField("CanOpenChat", ItemsChoiceType.BooleanValue)]
        public bool CanOpenChat { get; set; }

        [RightNowCustomObjectField("CanOpenIncident", ItemsChoiceType.BooleanValue)]
        public bool CanOpenIncident { get; set; }

        [RightNowCustomObjectField("PrimaryCTIEngine", ItemsChoiceType.StringValue)]
        public string PrimaryCTIEngine { get; set; }

        [RightNowCustomObjectField("SecondaryCTIEngine", ItemsChoiceType.StringValue)]
        public string SecondaryCTIEngine { get; set; }

        [RightNowCustomObjectField("IsQueueEnabled", ItemsChoiceType.BooleanValue)]
        public bool IsQueueEnabled { get; set; }

        [RightNowCustomObjectField("AutoRecieve", ItemsChoiceType.BooleanValue)]
        public bool AutoRecieve { get; set; }

        [RightNowCustomObjectField("VoiceScreenPop", ItemsChoiceType.BooleanValue)]
        public bool VoiceScreenPop { get; set; }
        [RightNowCustomObjectField("ChatScreenPop", ItemsChoiceType.BooleanValue)]
        public bool ChatScreenPop { get; set; }
        [RightNowCustomObjectField("IncidentScreenPop", ItemsChoiceType.BooleanValue)]
        public bool IncidentScreenPop { get; set; }

        [RightNowCustomObjectField("IsEnhanced", ItemsChoiceType.BooleanValue)]
        public bool IsEnhanced { get; set; }

        public List<ScreenPopOptions> ScreenPopOptionsList { get; set; }

        #endregion Property(ies)
    }

    [RightNowCustomObjectAttribute(OracleCtiObjectStrings.ScreenPopPackageName, OracleCtiObjectStrings.ScreenPopOptions)]
    public class ScreenPopOptions
    {
        public ScreenPopOptions()
        {
        }

        #region Property(ies)
        [RightNowCustomObjectField("ID",false)]
        public long ID { get; set; }

        [RightNowCustomObjectField("ScreenPopConfigID", ItemsChoiceType.NamedIDValue)]
        public long ScreenPopConfigID { get; set; }

        [RightNowCustomObjectField("Pop_Label", ItemsChoiceType.StringValue)]
        public string Pop_Label { get; set; }

        [RightNowCustomObjectField("Name", ItemsChoiceType.StringValue)]
        public string Name { get; set; }
        
        [RightNowCustomObjectField("Description", ItemsChoiceType.StringValue)]
        public string Description { get; set; }

        [RightNowCustomObjectField("Type", ItemsChoiceType.IntegerValue)]
        public int Type { get; set; }

        //[RightNowCustomObjectField("Expression", ItemsChoiceType.StringValue)]
        //public string Expression { get; set; }



        #endregion Property(ies)
    }
}
