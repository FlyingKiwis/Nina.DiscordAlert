using NINA.Sequencer;
using NINA.Sequencer.Container;
using NINA.Sequencer.SequenceItem;

namespace NINA.DiscordAlert.SequenceFailureMonitor {
    public class SequenceFailureMonitorFactory : ISequenceFailureMonitorFactory {
        public ISequenceFailureMonitor CreateSequenceFailureMonitor(ISequenceRootContainer container) {
            return new SequenceFailureMonitor(container);
        }

        public ISequenceFailureMonitor CreateSequenceFailureMonitor(ISequenceItem item) {
            return new SequenceFailureMonitor(item);
        }

        public ISequenceFailureMonitor CreateSequenceFailureMonitor(ISequenceEntity item) {
            return new SequenceFailureMonitor(item);
        }
    }
}
