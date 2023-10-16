using NINA.Sequencer;
using NINA.Sequencer.Container;
using NINA.Sequencer.SequenceItem;

namespace NINA.DiscordAlert.SequenceFailureMonitor {
    public interface ISequenceFailureMonitorFactory 
    {
        ISequenceFailureMonitor Create(ISequenceRootContainer container);

        ISequenceFailureMonitor Create(ISequenceItem item);

        ISequenceFailureMonitor Create(ISequenceEntity item);
    }
}
