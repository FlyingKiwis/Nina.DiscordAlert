using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace NINA.DiscordAlert.DiscordAlertSequenceItems {
    [Export(typeof(ResourceDictionary))]
    public partial class PluginItemTemplate : ResourceDictionary {

        [ExcludeFromCodeCoverage]
        public PluginItemTemplate() {
            InitializeComponent();
        }
    }
}