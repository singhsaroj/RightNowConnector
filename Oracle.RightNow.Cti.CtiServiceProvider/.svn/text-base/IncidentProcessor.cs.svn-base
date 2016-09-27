using RightNow.AddIns.AddInViews;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;

namespace Oracle.RightNow.Cti.CtiServiceProvider {
    [Export]
    public class IncidentProcessor {
        private Timer _pollingTimer;
        private RightNowObjectProvider _objectProvider;
        private DateTime _lastPollingTime;
        

        [Import]
        public CtiServiceSwitch Switch { get; set; }

        [Import]
        public IGlobalContext RightNowGlobalContext { get; set; }

        public void Initialize() {
            _objectProvider = new RightNowObjectProvider(RightNowGlobalContext);
           // _pollingTimer = new Timer(retrieveIncidents, null, 0, 15000);
            _lastPollingTime = DateTime.UtcNow.AddSeconds(-RightNowGlobalContext.TimeOffset);
        }

        private void retrieveIncidents(object state) {
            IList<IncidentInfo> incidents = _objectProvider.GetPendingIncidents(_lastPollingTime);

            foreach (var incident in incidents) {
                var interaction = new SwitchInteraction{
                    Id = Guid.NewGuid(),
                    LastStateChange = DateTime.Now,
                    Queue = incident.QueueName,
                    ReferenceId = incident.Id.ToString(),
                    SourceAddress =  incident.ContactEmail,
                    Type = getInteractionType(incident.Channel)
                };
                Switch.QueueInteraction(interaction);
            }   

            if (incidents.Count > 0)
                _lastPollingTime = incidents.Max(i => i.LastUpdate).ToUniversalTime();
        }

        private Oracle.RightNow.Cti.CtiServiceProvider.InteractionType getInteractionType(RightNowChannel rightNowChannel) {
            switch (rightNowChannel)
            {
                case RightNowChannel.Email:
                    return Oracle.RightNow.Cti.CtiServiceProvider.InteractionType.Email;
                case RightNowChannel.Web:
                    return Oracle.RightNow.Cti.CtiServiceProvider.InteractionType.Web;
            }

            return Oracle.RightNow.Cti.CtiServiceProvider.InteractionType.Email;
        }
    }
}
