using NINA.DiscordAlert.Properties;
using NINA.Core.Model;
using NINA.Core.Utility;
using NINA.Image.ImageData;
using NINA.Plugin;
using NINA.Plugin.Interfaces;
using NINA.Profile;
using NINA.Profile.Interfaces;
using NINA.WPF.Base.Interfaces.Mediator;
using NINA.WPF.Base.Interfaces.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Settings = NINA.DiscordAlert.Properties.Settings;

namespace NINA.DiscordAlert {
    /// <summary>
    /// Plugin Base
    /// </summary>
    [Export(typeof(IPluginManifest))]
    public class DiscordAlert : PluginBase, INotifyPropertyChanged {


        [ImportingConstructor]
        public DiscordAlert() {
            if (Settings.Default.UpdateSettings) {
                Settings.Default.Upgrade();
                Settings.Default.UpdateSettings = false;
                CoreUtil.SaveSettings(Settings.Default);
            }
        }

        public override Task Teardown() {
            return base.Teardown();
        }

        public string DiscordWebhookURL {
            get {
                return Settings.Default.DiscordWebhookURL;
            }
            set {
                Settings.Default.DiscordWebhookURL = value;
                CoreUtil.SaveSettings(Settings.Default);
                RaisePropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
