using System;
using System.Linq;

namespace Oracle.RightNow.Cti.Providers.CtiServiceProvider {
    internal interface ISwitchMessageProducer {
        void Subscribe(ISwitchMessageConsumer consumer);
        void Unsubscribe(ISwitchMessageConsumer consumer);
    }
}
