using NINA.Sequencer.Container;
using NINA.Sequencer.SequenceItem;
using NINA.Sequencer.Utility;
using System;
using System.Threading.Tasks;

namespace NINA.DiscordAlert.Util {
    public class SequenceFailureMonitor : ISequenceFailureMonitor {

        public event EventHandler<SequenceEntityFailureEventArgs> OnFailure;

        private ISequenceRootContainer _sequenceRootContainer;

        public SequenceFailureMonitor(ISequenceRootContainer container) {

            SetRootContainer(container);

        }

        public SequenceFailureMonitor(ISequenceItem item) {

            var root = ItemUtility.GetRootContainer(item.Parent);
            SetRootContainer(root);
        }

        public void Dispose() {
            if (_sequenceRootContainer != null) {
                _sequenceRootContainer.FailureEvent -= SequenceRootContainer_FailureEvent;
            }
        }

        private void SetRootContainer(ISequenceRootContainer container) {

            if (container == null)
                throw new ArgumentNullException(nameof(container));

            _sequenceRootContainer = container;
            _sequenceRootContainer.FailureEvent += SequenceRootContainer_FailureEvent;
        }

        private Task SequenceRootContainer_FailureEvent(object sender, SequenceEntityFailureEventArgs args) {
            OnFailure?.Invoke(sender, args);
            return Task.CompletedTask;
        }
    }
}
