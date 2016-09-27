using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.RightNow.Cti.Model;

namespace Oracle.RightNow.Cti {
    public interface IContactProvider {
        IList<Contact> GetContacts();
    }
}
