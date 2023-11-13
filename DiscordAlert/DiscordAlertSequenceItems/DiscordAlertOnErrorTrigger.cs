using Newtonsoft.Json;
using NINA.Core.Model;
using NINA.Sequencer.Container;
using NINA.Sequencer.SequenceItem;
using NINA.Sequencer.Trigger;
using NINA.Core.Utility;
using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using NINA.DiscordAlert.Util;
using NINA.DiscordAlert.SequenceFailureMonitor;
using NINA.DiscordAlert.DiscordWebhook;
using System.Diagnostics.CodeAnalysis;
using NINA.Equipment.Interfaces.Mediator;

namespace NINA.DiscordAlert.DiscordAlertSequenceItems {
    /// <summary>
    /// This is a trigger action that can be used in a sequence to send a discord message after an error occurs during the sequence
    /// </summary>
    [ExportMetadata("Name", "Discord: Send after failure")]
    [ExportMetadata("Description", "Sends a message to discord after a failure occurs")]
    [ExportMetadata("Icon", "Discord_logo_SVG")]
    [ExportMetadata("Category", "Discord Alert")]
    [Export(typeof(ISequenceTrigger))]
    [JsonObject(MemberSerialization.OptIn)]
    public class DiscordAlertOnErrorTrigger : SequenceTrigger {

        [ImportingConstructor]
        public DiscordAlertOnErrorTrigger(ICameraMediator cameraMediator, ITelescopeMediator telescopeMediator, IFilterWheelMediator filterWheelMediator, IFocuserMediator focuserMediator, IRotatorMediator rotatorMediator) {
            _cameraMediator = cameraMediator;
            _telescopeMediator = telescopeMediator;
            _filterWheelMediator = filterWheelMediator;
            _focuserMediator = focuserMediator;
            _rotatorMediator = rotatorMediator;
        }

        [JsonProperty]
        public string Text { get; set; } = "@everyone";

        private readonly ICameraMediator _cameraMediator;
        private readonly ITelescopeMediator _telescopeMediator;
        private readonly IFilterWheelMediator _filterWheelMediator;
        private readonly IFocuserMediator _focuserMediator;
        private readonly IRotatorMediator _rotatorMediator;

        private ISequenceFailureMonitor _failureMonitor;
        

        private async void FailureMonitor_OnFailure(object sender, SequenceFailureEventArgs e) {
            try {
                Logger.Debug($"Entity={e.Entity} Exception={e.Exception}");

                var templateValues = Helpers.Template.GetSequenceTemplateValues(e.Entity, _telescopeMediator, _cameraMediator, _filterWheelMediator, _focuserMediator, _rotatorMediator);
                await Helpers.Discord.SendMessage(MessageType.Error, Text, e.Entity, CancellationToken.None, exception: e.Exception, templateValues: templateValues);
            } catch (Exception ex) {
                Logger.Error(ex);
            }
        }

        public override void SequenceBlockInitialize() {
            
            _failureMonitor = Factories.SequenceFailureMonitor.Create(this);
            if (_failureMonitor != null) {
                _failureMonitor.OnFailure += FailureMonitor_OnFailure;
                Logger.Info("Attached Failure Monitor");
            }

            base.SequenceBlockInitialize();
        }

        public override void SequenceBlockTeardown() 
        {
            Logger.Debug(string.Empty);
            if (_failureMonitor != null) {
                try {
                    _failureMonitor.Dispose();
                } catch { }
                _failureMonitor.OnFailure -= FailureMonitor_OnFailure;
                Logger.Info("Removed & disposed Failure Monitor");
            }
            
            base.SequenceBlockTeardown();
        }

        [ExcludeFromCodeCoverage]
        public override object Clone() {
            return new DiscordAlertOnErrorTrigger(_cameraMediator, _telescopeMediator, _filterWheelMediator, _focuserMediator, _rotatorMediator) {
                Icon = Icon,
                Name = Name,
                Category = Category,
                Description = Description,
                Text = Text
            };
        }

        [ExcludeFromCodeCoverage]
        public override bool ShouldTrigger(ISequenceItem previousItem, ISequenceItem nextItem) {
            return false;
        }

        [ExcludeFromCodeCoverage]
        public override Task Execute(ISequenceContainer context, IProgress<ApplicationStatus> progress, CancellationToken token) {
            return Task.CompletedTask;
        }

        [ExcludeFromCodeCoverage]
        public override string ToString() {
            return $"Category: {Category}, Item: {nameof(DiscordAlertOnErrorTrigger)}";
        }
    }
}