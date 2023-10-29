using Newtonsoft.Json;
using NINA.Core.Model;
using NINA.Core.Utility;
using NINA.Sequencer.SequenceItem;
using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using NINA.DiscordAlert.DiscordWebhook;
using NINA.DiscordAlert.Util;
using NINA.Profile.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace NINA.DiscordAlert.DiscordAlertSequenceItems {
    /// <summary>
    /// Sends a discord message when this sequence item is reached
    /// </summary>
    [ExportMetadata("Name", "Discord: Send message")]
    [ExportMetadata("Description", "Sends a message to discord")]
    [ExportMetadata("Icon", "Discord_logo_SVG")]
    [ExportMetadata("Category", "Discord Alert")]
    [Export(typeof(ISequenceItem))]
    [JsonObject(MemberSerialization.OptIn)]
    public class DiscordMessageInstruction : SequenceItem {

        private readonly IProfileService _profileService;

        [ImportingConstructor]
        public DiscordMessageInstruction(IProfileService profileService) 
        {
            _profileService = profileService;
        }

        [ExcludeFromCodeCoverage]
        public DiscordMessageInstruction(DiscordMessageInstruction copyMe) : this(copyMe._profileService) {
            CopyMetaData(copyMe);
        }

        [JsonProperty]
        public string Text { get; set; }

        public override async Task Execute(IProgress<ApplicationStatus> progress, CancellationToken token) {
            try {
                
                Logger.Debug($"Sending Message - Text={Text}  Entity={this}");
                var templateValues = Helpers.Template.GetSequenceTemplateValues(this, _profileService);
                await Helpers.Discord.SendMessage(MessageType.Information, Text, this, token, templateValues: templateValues);
            } catch (Exception ex) {
                Logger.Error(ex);
            }
        }

        [ExcludeFromCodeCoverage]
        public override object Clone() {
            return new DiscordMessageInstruction(this) {
                Icon = Icon,
                Name = Name,
                Category = Category,
                Description = Description,
                Text = Text
            };
        }

        [ExcludeFromCodeCoverage]
        public override string ToString() {
            return $"Category: {Category}, Item: {nameof(DiscordMessageInstruction)}, Text: {Text}";
        }
    }
}