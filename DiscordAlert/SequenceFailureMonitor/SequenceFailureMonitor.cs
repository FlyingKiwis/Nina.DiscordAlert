using NINA.Core.Utility;
using NINA.Sequencer;
using NINA.Sequencer.Container;
using NINA.Sequencer.SequenceItem;
using NINA.Sequencer.Utility;
using System;
using System.Threading.Tasks;

namespace NINA.DiscordAlert.SequenceFailureMonitor {
    public class SequenceFailureMonitor : ISequenceFailureMonitor {

        public event EventHandler<SequenceFailureEventArgs> OnFailure;

        private ISequenceRootContainer _sequenceRootContainer;

        public SequenceFailureMonitor(ISequenceRootContainer container) {
            SetRootContainer(container);
        }

        public SequenceFailureMonitor(ISequenceItem item) {
            if(item.Parent == null) 
            {
                throw new ArgumentException($"{nameof(item)} does not have a parent");
            }

            var root = ItemUtility.GetRootContainer(item.Parent);
            SetRootContainer(root);
        }

        public SequenceFailureMonitor(ISequenceEntity entity) {
            if (entity.Parent == null) {
                throw new ArgumentException($"{nameof(entity)} does not have a parent");
            }

            var root = ItemUtility.GetRootContainer(entity.Parent);
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
            Logger.Error($"Failure event occured: Entity={args.Entity}", args.Exception);

            OnFailure?.Invoke(sender, new SequenceFailureEventArgs(args.Entity, args.Exception));
            return Task.CompletedTask;
        }
    }
}
