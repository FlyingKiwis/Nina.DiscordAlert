using NINA.Sequencer;
using System;

namespace NINA.DiscordAlert.Util {
    public class SequenceFailureEventArgs : EventArgs
    {
        public ISequenceEntity Entity { get; }
        public Exception Exception { get; }

        public SequenceFailureEventArgs(ISequenceEntity entity, Exception ex) {
            Entity = entity;
            Exception = ex;
        }
    }
}
