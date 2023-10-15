using NINA.Sequencer;
using NINA.Sequencer.Container;
using NINA.Sequencer.SequenceItem;

namespace NINA.DiscordAlert.SequenceFailureMonitor {
    public class SequenceFailureMonitorFactory : ISequenceFailureMonitorFactory {
        public ISequenceFailureMonitor Create(ISequenceRootContainer container) {
            return new SequenceFailureMonitor(container);
        }

        public ISequenceFailureMonitor Create(ISequenceItem item) {
            return new SequenceFailureMonitor(item);
        }

        public ISequenceFailureMonitor Create(ISequenceEntity item) {
            return new SequenceFailureMonitor(item);
        }
    }
}
