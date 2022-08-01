using System.ComponentModel.Composition;
using System.Windows;

namespace NINA.DiscordAlert.DiscordAlertSequenceItems {
    [Export(typeof(ResourceDictionary))]
    public partial class PluginItemTemplate : ResourceDictionary {
        public PluginItemTemplate() {
            InitializeComponent();
        }
    }
}