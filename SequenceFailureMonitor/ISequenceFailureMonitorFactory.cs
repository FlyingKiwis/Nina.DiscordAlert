using NINA.Sequencer.Container;
using NINA.Sequencer.SequenceItem;

namespace NINA.DiscordAlert.SequenceFailureMonitor {
    public interface ISequenceFailureMonitorFactory 
    {
        ISequenceFailureMonitor CreateSequenceFailureMonitor(ISequenceRootContainer container);

        ISequenceFailureMonitor CreateSequenceFailureMonitor(ISequenceItem item);
    }
}
