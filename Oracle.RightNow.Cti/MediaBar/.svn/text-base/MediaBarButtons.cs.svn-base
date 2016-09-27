using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oracle.RightNow.Cti.MediaBar {
    public enum MediaBarButtons {
        None = 0,
        AnswerHangup = 1,
        HoldRetrieve = 2,
        Transfer = 4,
        Conference = 8,
        //Namachi
        //Chat = 16,
        //Incident = 32,
        CustomVoice1 = 16,
        CustomVoice2 = 32,
        CustomVoice3 = 64,
        CustomVoice4 = 128,
        CustomVoice5 = 256,
        BlindTransfer = 512,
        BlindConference = 1024,
        StandardVoice = AnswerHangup | HoldRetrieve | Transfer | Conference | BlindTransfer | BlindConference,
        CustomVoice = CustomVoice1 | CustomVoice2 | CustomVoice3 | CustomVoice4 | CustomVoice5,
        Voice = StandardVoice | CustomVoice,
        //CustomVoice = CustomVoice1 | CustomVoice2 | CustomVoice3,
        //Voice = StandardVoice | CustomVoice,
        All = Voice,
        ConsultConferenced = HoldRetrieve | Transfer | Conference,
        ConferencedCall = AnswerHangup | HoldRetrieve
    }
}
