﻿using Newtonsoft.Json;
using NINA.Core.Model;
using NINA.Core.Enum;
using NINA.Sequencer.Container;
using NINA.Sequencer.SequenceItem;
using NINA.Sequencer.Trigger;
using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using NINA.DiscordAlert.Util;
using NINA.Sequencer.Utility;

namespace NINA.DiscordAlert.DiscordAlertSequenceItems {
    /// <summary>
    /// This is a trigger action that can be used in a sequence to send a discord message after an error occurs during the sequence
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
            _failureMonitor = Resources.SequenceFailureMonitorFactory.CreateSequenceFailureMonitor(this.Parent);
            _failureMonitor.OnFailure += FailureMonitor_OnFailure;
        }

        [JsonProperty]
        public string Text { get; set; } = "@everyone";

        private ISequenceItem _failedItem;
        private ISequenceFailureMonitor _failureMonitor;

        public override object Clone() {
            return new DiscordAlertOnErrorTrigger() {
                Icon = Icon,
                Name = Name,
                Category = Category,
                Description = Description
            };
        }

        private async void FailureMonitor_OnFailure(object sender, SequenceEntityFailureEventArgs e) {

            await DiscordHelper.SendMessage(MessageType.Error, Text, e.Entity, CancellationToken.None);

        }

        public override void Teardown() {
            base.Teardown();

            _failureMonitor?.Dispose();
        }

        public override string ToString() {
            return $"Category: {Category}, Item: {nameof(DiscordAlertOnErrorTrigger)}";
        }

        public override bool ShouldTrigger(ISequenceItem previousItem, ISequenceItem nextItem) {
            return false;
        }

        public override Task Execute(ISequenceContainer context, IProgress<ApplicationStatus> progress, CancellationToken token) {
            return Task.CompletedTask;
        }
    }
}