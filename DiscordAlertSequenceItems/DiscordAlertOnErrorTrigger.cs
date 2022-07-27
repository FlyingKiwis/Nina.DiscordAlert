using Newtonsoft.Json;
using NINA.Core.Model;
using NINA.Core.Enum;
using NINA.Sequencer.Container;
using NINA.Sequencer.SequenceItem;
using NINA.Sequencer.Trigger;
using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using NINA.Core.Utility.Notification;
using System.Windows.Controls;
using System.Collections.Generic;

namespace DrewMcdermott.NINA.DiscordAlert.DiscordAlertSequenceItems {
    /// <summary>
    /// This is a action that can be used in a sequence to send a discord message after an error occurs during the sequence
    /// </summary>
    [ExportMetadata("Name", "Send after failure")]
    [ExportMetadata("Description", "Sends a message to discord after a failure occurs")]
    [ExportMetadata("Icon", "Discord_logo_SVG")]
    [ExportMetadata("Category", "Discord Alert")]
    [Export(typeof(ISequenceTrigger))]
    [JsonObject(MemberSerialization.OptIn)]
    public class DiscordAlertOnErrorTrigger : SequenceTrigger {

        [ImportingConstructor]
        public DiscordAlertOnErrorTrigger() {
        }

        [JsonProperty]
        public string Text { get; set; } = "@everyone";

        private ISequenceItem _failedItem;

        public override object Clone() {
            return new DiscordAlertOnErrorTrigger() {
                Icon = Icon,
                Name = Name,
                Category = Category,
                Description = Description
            };
        }

        public override async Task Execute(ISequenceContainer context, IProgress<ApplicationStatus> progress, CancellationToken token) {
            await Util.DiscordHelper.SendMessage(Util.MessageType.Error, String.Empty, _failedItem, token, Mention);
        }

        public override bool ShouldTrigger(ISequenceItem previousItem, ISequenceItem nextItem) 
        {
            return false;
        }

        public override bool ShouldTriggerAfter(ISequenceItem previousItem, ISequenceItem nextItem) 
        {
            return ShouldTriggerInternal(previousItem, nextItem);
        }

        private bool ShouldTriggerInternal(ISequenceItem previousItem, ISequenceItem nextItem) 
        {
            if (previousItem == null)
                return false;

            if (previousItem.Status != SequenceEntityStatus.FAILED)
                return false;

            _failedItem = previousItem;
            return true;
        }

        public override string ToString() {
            return $"Category: {Category}, Item: {nameof(DiscordAlertOnErrorTrigger)}";
        }
    }
}