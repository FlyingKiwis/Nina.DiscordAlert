using NINA.Sequencer.Container;
using NINA.Sequencer.SequenceItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrewMcdermott.NINA.DiscordAlert.Util {
    public static class SequenceHelper {

        public static ISequenceContainer RootContainer(this ISequenceContainer container)
        {
            return container.Parent == null ? container : container.Parent.RootContainer();
        }

        public static ISequenceContainer RootContainer(this ISequenceItem item) 
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

        public static IDeepSkyObjectContainer TargetContainer(this ISequenceItem item) {
            return item.Parent == null ? null : item.Parent.TargetContainer();
        }
    }
}
