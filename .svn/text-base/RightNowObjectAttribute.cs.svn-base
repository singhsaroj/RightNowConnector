using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using inContact.Integration.RightNow.ConnectService;

namespace inContact.Integration.RightNow
{
    public class RightNowObjectAttribute : Attribute
    {
        public RightNowObjectAttribute(string packageName, string objectName)
        {
            ObjectName = objectName;
            PackageName = packageName;
        }

        public string PackageName { get; set; }

        public string ObjectName { get; set; }
    }

    public class RightNowFieldAttribute : Attribute
    {
        public RightNowFieldAttribute(string name)
            : this(name, true)
        { }

        public RightNowFieldAttribute(string name, bool canUpdate)
        {
            CanUpdate = canUpdate;
            Name = name;
        }

        public RightNowFieldAttribute(string name, ItemsChoiceType type)
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
