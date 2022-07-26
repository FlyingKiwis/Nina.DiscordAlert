﻿using NINA.Sequencer;
using NINA.Sequencer.Container;
using NINA.Sequencer.SequenceItem;

namespace NINA.DiscordAlert.Util {
    public static class SequenceExtensions {

        public static ISequenceContainer RootContainer(this ISequenceContainer container)
        {
            return container.Parent == null ? container : container.Parent.RootContainer();
        }

        public static ISequenceContainer RootContainer(this ISequenceEntity item) 
        {
            return item.Parent == null ? null : item.Parent.RootContainer();
        }

        public static IDeepSkyObjectContainer TargetContainer(this ISequenceContainer container) 
        {
            if(container is IDeepSkyObjectContainer dsoContainer) {
                return dsoContainer;
            }

            if (container.Parent == null)
                return null;

            return container.Parent.TargetContainer();
        }

        public static IDeepSkyObjectContainer TargetContainer(this ISequenceEntity item) {
            return item.Parent == null ? null : item.Parent.TargetContainer();
        }
    }
}
