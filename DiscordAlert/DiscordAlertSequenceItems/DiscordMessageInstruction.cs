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
using NINA.Equipment.Interfaces.Mediator;

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

        [ImportingConstructor]
        public DiscordMessageInstruction(ICameraMediator cameraMediator, ITelescopeMediator telescopeMediator, IFilterWheelMediator filterWheelMediator, IFocuserMediator focuserMediator, IRotatorMediator rotatorMediator) {
            _cameraMediator = cameraMediator;
            _telescopeMediator = telescopeMediator;
            _filterWheelMediator = filterWheelMediator;
            _focuserMediator = focuserMediator;
            _rotatorMediator = rotatorMediator;
        }

        [ExcludeFromCodeCoverage]
        public DiscordMessageInstruction(DiscordMessageInstruction copyMe) : this(copyMe._cameraMediator, copyMe._telescopeMediator, copyMe._filterWheelMediator, copyMe._focuserMediator, copyMe._rotatorMediator) {
            CopyMetaData(copyMe);
        }

        [JsonProperty]
        public string Text { get; set; }

        private readonly ICameraMediator _cameraMediator;
        private readonly ITelescopeMediator _telescopeMediator;
        private readonly IFilterWheelMediator _filterWheelMediator;
        private readonly IFocuserMediator _focuserMediator;
        private readonly IRotatorMediator _rotatorMediator;

        public override async Task Execute(IProgress<ApplicationStatus> progress, CancellationToken token) {
            try {
                
                Logger.Debug($"Sending Message - Text={Text}  Entity={this}");
                var templateValues = Helpers.Template.GetSequenceTemplateValues(this, _telescopeMediator, _cameraMediator, _filterWheelMediator, _focuserMediator, _rotatorMediator);
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