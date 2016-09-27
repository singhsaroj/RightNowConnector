using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oracle.RightNow.Cti.Common
{
    internal static class OracleCtiObjectStrings
    {
        public const string PackageName = "Oracle_Cti";
        public const string AgentState = "AgentState";
        public const string ScreenPopPackageName = "Avaya_Cti";
        public const string ScreenPopConfig = "ScreenPopConfig";
        public const string ScreenPopOptions = "ScreenPopValues";
    }
    public class RightNowCustomObjectAttribute : Attribute
    {
      
        public RightNowCustomObjectAttribute(string packageName, string objectName)
        {
            ObjectName = objectName;
            PackageName = packageName;
        }

        public string PackageName { get; set; }
        public string ObjectName { get; set; }
    }
}
