using Newtonsoft.Json;
using NINA.Core.Model;
using NINA.DiscordAlert.DiscordWebhook;
using NINA.DiscordAlert.Images;
using NINA.DiscordAlert.SequenceFailureMonitor;
using NINA.DiscordAlert.Util;
using NINA.Sequencer.Container;
using NINA.Sequencer.SequenceItem;
using NINA.Sequencer.Trigger;
using NINA.WPF.Base.Interfaces.Mediator;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Threading;
using System;
using NINA.Image.Interfaces;
using NINA.Core.Utility;
using NINA.Sequencer.Interfaces;
using System.Collections.Concurrent;

namespace NINA.DiscordAlert.DiscordAlertSequenceItems {
    /// <summary>
    /// This is a trigger action that can be used in a sequence to send a discord message after an error occurs during the sequence
    /// </summary>
    [ExportMetadata("Name", "Discord: Send after image")]
    [ExportMetadata("Description", "Sends a message to discord after an image is taken")]
    [ExportMetadata("Icon", "Discord_logo_SVG")]
    [ExportMetadata("Category", "Discord Alert")]
    [Export(typeof(ISequenceTrigger))]
    [JsonObject(MemberSerialization.OptIn)]
    public class DiscordMessageAfterImageTrigger : SequenceTrigger {
        private readonly IImageSaveMediator _imgageMediator;
        private readonly IImageDataFactory _imageDataFactory;
        private readonly IImageSaveMonitor _imageMonitor;
        private readonly ConcurrentQueue<CancellationToken> _triggerQueue = new ConcurrentQueue<CancellationToken>();
        private bool _enabled = false;

        [ImportingConstructor]
        public DiscordMessageAfterImageTrigger(IImageSaveMediator imageSaveMediator, IImageDataFactory imageDataFactory) {
            _imgageMediator = imageSaveMediator;
            _imageDataFactory = imageDataFactory;
            _imageMonitor = Factories.ImageSaveMonitorFactory.Create(_imgageMediator);
        }

        [JsonProperty]
        public string Text { get; set; } = "";

        public override object Clone() {
            return new DiscordMessageAfterImageTrigger(_imgageMediator, _imageDataFactory) {
                Icon = Icon,
                Name = Name,
                Category = Category,
                Description = Description,
                Text = Text
            };
        }

        public override void SequenceBlockInitialize() {
            Logger.Debug(string.Empty);

            _imageMonitor.ImageSaved -= ImageMonitor_ImageSaved;
            _imageMonitor.ImageSaved += ImageMonitor_ImageSaved;
            base.SequenceBlockInitialize();
        }

        public override void SequenceBlockTeardown() {
            Logger.Debug(string.Empty);
            _enabled = false;
            base.SequenceBlockTeardown();
        }

        public override string ToString() {
            return $"Category: {Category}, Item: {nameof(DiscordMessageAfterImageTrigger)}";
        }

        public override bool ShouldTrigger(ISequenceItem previousItem, ISequenceItem nextItem) 
        {
            Logger.Debug($"{nameof(previousItem)}={previousItem} {nameof(nextItem)}={nextItem}");
            if (nextItem is IExposureItem) 
            {
                _enabled = true;
                return true;
            }
            return false;
        }

        public override async Task Execute(ISequenceContainer context, IProgress<ApplicationStatus> progress, CancellationToken token) {
            try {

                Logger.Debug(string.Empty);
                if (_enabled) {
                    _triggerQueue.Enqueue(token);
                }
            }
            catch (Exception ex) {
                Logger.Error(ex);
            }
        }

        private async void ImageMonitor_ImageSaved(object sender, ISavedImageContainer e) 
        {
            if(_triggerQueue.TryDequeue(out var cancelToken)) 
            {
                if (cancelToken.IsCancellationRequested)
                    return;

                await DiscordHelper.SendMessage(MessageType.Information, Text, this, cancelToken, e);
            }
            else {

                if(!_enabled) 
                {
                    Logger.Debug("Disposed");
                    _imageMonitor.ImageSaved -= ImageMonitor_ImageSaved;
                }
            }
        }
    }
}
