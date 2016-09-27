using System.Collections.Generic;
using Oracle.RightNow.Cti.Model;

namespace Oracle.RightNow.Cti.Configuration {
    public interface IConfigurationProvider {
        IEnumerable<AgentState> GetAgentStates();
        ScreenPopConfig GetAgentScreenByProfile();
        bool HideorDisabled { get; }
    }
}