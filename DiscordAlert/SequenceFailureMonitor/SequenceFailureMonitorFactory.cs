using NINA.Sequencer;
using NINA.Sequencer.Container;
using NINA.Sequencer.SequenceItem;
using System.Diagnostics.CodeAnalysis;

namespace NINA.DiscordAlert.SequenceFailureMonitor {
    [ExcludeFromCodeCoverage]
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
