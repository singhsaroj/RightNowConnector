using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Oracle.RightNow.Cti;
using RightNow.AddIns.AddInViews;

namespace Oracle.RightNow.Cti.Administration.ViewModels {
    [Export(typeof(IAgentStatesAdministrationViewModel))]
    public class AgentStatesAdministrationViewModel : IAgentStatesAdministrationViewModel {
        public void Load() {
            var objectProvider = new RightNowObjectProvider(Context);
            objectProvider.GetObjects<RightNow.Cti.Model.AgentState>();
        }

        [Import]
        public IGlobalContext Context { get; set; }
    }
}
