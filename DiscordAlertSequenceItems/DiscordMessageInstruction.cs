using DrewMcdermott.NINA.DiscordAlert.Util;
using Newtonsoft.Json;
using NINA.Core.Model;
using NINA.Sequencer.SequenceItem;
using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;

namespace DrewMcdermott.NINA.DiscordAlert.DiscordAlertSequenceItems {
    /// <summary>
    /// Sends a discord message when this sequence item is reached
    /// </summary>
    [ExportMetadata("Name", "Discord message")]
    [ExportMetadata("Description", "This item will just show a notification and is just there to show how the plugin system works")]
    [ExportMetadata("Icon", "Discord_logo_SVG")]
    [ExportMetadata("Category", "Discord Alert")]
    [Export(typeof(ISequenceItem))]
    [JsonObject(MemberSerialization.OptIn)]
    public class DiscordMessageInstruction : SequenceItem {

        [ImportingConstructor]
        public DiscordMessageInstruction() {
            Text = String.Empty;
        }
        public DiscordMessageInstruction(DiscordMessageInstruction copyMe) : this() {
            CopyMetaData(copyMe);
        }

        [JsonProperty]
        public string Text { get; set; }

        public override Task Execute(IProgress<ApplicationStatus> progress, CancellationToken token) {
            DiscordHelper.SendMessage(Text, this, token);

            return Task.CompletedTask;
        }

        public override object Clone() {
            return new DiscordMessageInstruction(this);
        }

        public override string ToString() {
            return $"Category: {Category}, Item: {nameof(DiscordMessageInstruction)}, Text: {Text}";
        }
    }
}