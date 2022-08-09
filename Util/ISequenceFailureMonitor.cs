using NINA.Sequencer.Container;
using NINA.Sequencer.SequenceItem;
using NINA.Sequencer.Utility;
using System;

namespace NINA.DiscordAlert.Util {
    public interface ISequenceFailureMonitor : IDisposable
    {
        event EventHandler<SequenceEntityFailureEventArgs> OnFailure;
    }
}
