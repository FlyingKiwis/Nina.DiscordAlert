using NINA.Sequencer;
using NINA.Sequencer.Container;

namespace NINA.DiscordAlert.Util {
    public static class SequenceExtensions {

        public static ISequenceContainer GetRootContainer(this ISequenceContainer container)
        {
            return container.Parent == null ? container : container.Parent.GetRootContainer();
        }

        public static ISequenceContainer GetRootContainer(this ISequenceEntity item) 
        {
            return item.Parent == null ? null : item.Parent.GetRootContainer();
        }

        public static IDeepSkyObjectContainer GetDSOContainer(this ISequenceContainer container) 
        {
            if(container is IDeepSkyObjectContainer dsoContainer) {
                return dsoContainer;
            }

            if (container.Parent == null)
                return null;

            return container.Parent.GetDSOContainer();
        }

        public static IDeepSkyObjectContainer GetDSOContainer(this ISequenceEntity item) {
            return item.Parent == null ? null : item.Parent.GetDSOContainer();
        }
    }
}
