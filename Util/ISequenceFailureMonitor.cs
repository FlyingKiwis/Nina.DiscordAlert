using System;

namespace NINA.DiscordAlert.Util {
    public interface ISequenceFailureMonitor : IDisposable
    {
        event EventHandler<SequenceFailureEventArgs> OnFailure;
    }
}
