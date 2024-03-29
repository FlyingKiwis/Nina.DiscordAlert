﻿using NINA.Core.Utility;
using NINA.Plugin;
using NINA.Plugin.Interfaces;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
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

        [ExcludeFromCodeCoverage]
        public override Task Teardown() {
            return base.Teardown();
        }

        public string DiscordWebhookURL {
            get {
                return Settings.Default.DiscordWebhookURL;
            }
            set {
                Logger.Debug($"Set discord webhook URL={value}");
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
