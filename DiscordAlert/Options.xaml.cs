using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace NINA.DiscordAlert {

    [Export(typeof(ResourceDictionary))]
    [ExcludeFromCodeCoverage]
    partial class Options : ResourceDictionary {

        public Options() {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e) {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}