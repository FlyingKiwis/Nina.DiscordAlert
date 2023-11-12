using Newtonsoft.Json;
using NINA.Core.Model;
using NINA.DiscordAlert.DiscordWebhook;
using NINA.DiscordAlert.Images;
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
using NINA.Equipment.Interfaces.Mediator;
using NINA.Profile.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Diagnostics;
using System.Windows.Media.Imaging;

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
        private readonly IImageSaveMediator _imageSaveMediator;
        private readonly IImageDataFactory _imageDataFactory;
        private readonly IImagingMediator _imagingMediator;
        private readonly IProfileService _profileService;
        private readonly ConcurrentQueue<ExecuteDetails> _triggerQueue = new ConcurrentQueue<ExecuteDetails>();
        private bool _enabled = false;

        [ImportingConstructor]
        public DiscordMessageAfterImageTrigger(IImageSaveMediator imageSaveMediator, IImagingMediator imagingMediator, IImageDataFactory imageDataFactory, IProfileService profileService) {
            _imageSaveMediator = imageSaveMediator;
            _imageDataFactory = imageDataFactory;
            _imagingMediator = imagingMediator;
            _profileService = profileService;
        }

        [JsonProperty]
        public string Text { get; set; } = "";

        public override void SequenceBlockInitialize() {
            Logger.Debug(string.Empty);

            _imageSaveMediator.ImageSaved -= ImageMonitor_ImageSaved;        
            _imageSaveMediator.ImageSaved += ImageMonitor_ImageSaved;
            base.SequenceBlockInitialize();
        }

        [ExcludeFromCodeCoverage]
        public override void SequenceBlockTeardown() {
            Logger.Debug(string.Empty);
            _enabled = false;
            base.SequenceBlockTeardown();
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
            Logger.Debug(string.Empty);
            if (_enabled) {
                _triggerQueue.Enqueue(new ExecuteDetails(context, progress, token));
            }
        }

        private void ImageMonitor_ImageSaved(object sender, ImageSavedEventArgs e) 
        {
            if(_triggerQueue.TryDequeue(out var executeDetails)) 
            {
                if (executeDetails.Token.IsCancellationRequested)
                    return;
                try {
                    var image = e.Image.Clone();
                    image.Freeze();
                    var saveImageContainer = new SavedImageContainer(e.MetaData, image, e.Statistics, e.StarDetectionAnalysis, e.PathToImage, e.IsBayered);
                    _ = Task.Run(() => SendMessageAsync(executeDetails, saveImageContainer));      
                } catch (Exception ex) {
                    Logger.Error(ex);
                }
            }
            else 
            {
                if(!_enabled) 
                {
                    _imageSaveMediator.ImageSaved -= ImageMonitor_ImageSaved;
                }
            }
        }

        private async void SendMessageAsync(ExecuteDetails executeDetails, ISavedImageContainer imageArgs)  {
            var renderStopwatch = new Stopwatch();
            var sendStopwatch = new Stopwatch();
            renderStopwatch.Start();
            var resizedImage = imageArgs.Image.Resize(2560);
            var templateValues = Helpers.Template.GetImageTemplateValues(imageArgs.MetaData, imageArgs.StarDetectionAnalysis);
            renderStopwatch.Stop();
            sendStopwatch.Start();
            using (var tempFileWriter = Factories.TemporaryImageFileWriter.Create(_profileService.ActiveProfile, resizedImage)) {
                await Helpers.Discord.SendMessage(MessageType.Information, Text, executeDetails.Context, executeDetails.Token, templateValues: templateValues, attachedFilename: tempFileWriter.Filename);
            }
            sendStopwatch.Stop();
            Logger.Info($"Render time={renderStopwatch.ElapsedMilliseconds}ms send time={sendStopwatch.ElapsedMilliseconds}ms");
        }

        [ExcludeFromCodeCoverage]
        public override object Clone() {
            return new DiscordMessageAfterImageTrigger(_imageSaveMediator, _imagingMediator, _imageDataFactory, _profileService) {
                Icon = Icon,
                Name = Name,
                Category = Category,
                Description = Description,
                Text = Text
            };
        }

        [ExcludeFromCodeCoverage]
        public override string ToString() {
            return $"Category: {Category}, Item: {nameof(DiscordMessageAfterImageTrigger)}";
        }
    }
}
