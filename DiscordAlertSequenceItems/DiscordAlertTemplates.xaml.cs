using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace DrewMcdermott.NINA.DiscordAlert.DiscordAlertSequenceItems {
    [Export(typeof(ResourceDictionary))]
    public partial class PluginItemTemplate : ResourceDictionary {
        public PluginItemTemplate() {
            InitializeComponent();
        }
    }
}