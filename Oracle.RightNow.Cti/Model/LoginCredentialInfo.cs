using Oracle.RightNow.Cti.Common.ConnectService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oracle.RightNow.Cti.Model
{
    [RightNowCustomObjectAttribute(OracleCtiObjectStrings.ScreenPopPackageName, OracleCtiObjectStrings.LoginCredential)]
    public class LoginCredentialInfo
    {
        #region Property(ies)
        [RightNowCustomObjectField("ID", false)]
        public long ID { get; set; }

        [RightNowCustomObjectField("AgentID", ItemsChoiceType.IntegerValue)]
        public int AgentID { get; set; }

        [RightNowCustomObjectField("Password", ItemsChoiceType.StringValue)]
        public string Password { get; set; }

        [RightNowCustomObjectField("AccountID", ItemsChoiceType.IntegerValue)]
        public int AccountID { get; set; }

        [RightNowCustomObjectField("Extension", ItemsChoiceType.IntegerValue)]
        public int Extension { get; set; }

        [RightNowCustomObjectField("Queue", ItemsChoiceType.IntegerValue)]
        public int Queue { get; set; }

        #endregion Property(ies)
    }
}
