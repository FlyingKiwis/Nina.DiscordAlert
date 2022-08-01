using System.ComponentModel.Composition;
using System.Windows;

namespace NINA.DiscordAlert {

    [Export(typeof(ResourceDictionary))]
    partial class Options : ResourceDictionary {

        public Options() {
            InitializeComponent();
        }
    }
}