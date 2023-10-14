using System;

namespace NINA.DiscordAlert.SequenceFailureMonitor {
    public interface ISequenceFailureMonitor : IDisposable
    {
        event EventHandler<SequenceFailureEventArgs> OnFailure;
    }
}
