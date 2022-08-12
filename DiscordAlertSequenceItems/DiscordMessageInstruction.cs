using NINA.DiscordAlert.Util;
using Newtonsoft.Json;
using NINA.Core.Model;
using NINA.Sequencer.SequenceItem;
using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using NINA.DiscordAlert.DiscordWebhook;

namespace NINA.DiscordAlert.DiscordAlertSequenceItems {
    /// <summary>
    /// Sends a discord message when this sequence item is reached
    /// </summary>
    [ExportMetadata("Name", "Discord message")]
    [ExportMetadata("Description", "Sends a message to discord")]
    [ExportMetadata("Icon", "Discord_logo_SVG")]
    [ExportMetadata("Category", "Discord Alert")]
    [Export(typeof(ISequenceItem))]
    [JsonObject(MemberSerialization.OptIn)]
    public class DiscordMessageInstruction : SequenceItem {

        [ImportingConstructor]
        public DiscordMessageInstruction() {
        }
        public DiscordMessageInstruction(DiscordMessageInstruction copyMe) : this() {
            CopyMetaData(copyMe);
        }

        [JsonProperty]
        public string Text { get; set; }

        public override async Task Execute(IProgress<ApplicationStatus> progress, CancellationToken token) {
            await DiscordHelper.SendMessage(MessageType.Information, Text, this, token);
        }

        public override object Clone() {
            return new DiscordMessageInstruction(this) {
                Icon = Icon,
                Name = Name,
                Category = Category,
                Description = Description,
                Text = Text
            };
        }

        public override string ToString() {
            return $"Category: {Category}, Item: {nameof(DiscordMessageInstruction)}, Text: {Text}";
        }
    }
}