using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oracle.RightNow.Cti.ScreenPopConfiguration
{
    public class NameValueViewModel
    {
        public NameValueViewModel(long id, string name)
        {
            this.Value = id;
            this.Name = name;
        }

        public NameValueViewModel(long id, string name, object tag)
        {
            this.Value = id;
            this.Name = name;
            this.Tag = tag;
        }

        public NameValueViewModel(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }

        public long Value { get; set; }

        public object Tag { get; set; }

        public override string ToString()
        {
            return string.Format("Name:{0}, Value:{1}, Tag:{2}", Name, Value, Tag);
        }
    }
}
