using Oracle.RightNow.Cti.Common.ConnectService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oracle.RightNow.Cti.Common
{
    public class RightNowCustomObjectFieldAttribute : Attribute
    {
        public RightNowCustomObjectFieldAttribute(string name)
        {
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
