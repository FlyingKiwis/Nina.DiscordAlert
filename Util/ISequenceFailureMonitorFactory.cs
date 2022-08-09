using NINA.Sequencer.Container;
using NINA.Sequencer.SequenceItem;

namespace NINA.DiscordAlert.Util {
    public interface ISequenceFailureMonitorFactory 
    {
        ISequenceFailureMonitor CreateSequenceFailureMonitor(ISequenceRootContainer container);

        ISequenceFailureMonitor CreateSequenceFailureMonitor(ISequenceItem item);
    }
}
