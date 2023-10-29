using System.Threading;
using System.Threading.Tasks;
using Discord;
using System.Collections.Generic;
using System;
using NINA.Sequencer;
using NINA.DiscordAlert.Util;
using NINA.Core.Utility;
using NINA.DiscordAlert.Images;
using System.Windows.Media.Imaging;
using System.IO;
using NINA.Core.Model;

namespace NINA.DiscordAlert.DiscordWebhook {
    public class DiscordHelper : IDiscordHelper {
        public async Task SendMessage(MessageType type, string message, ISequenceEntity sequenceItem, CancellationToken cancelToken, ImagePatterns templateValues = null, BitmapSource attachedImage = null, Exception exception = null) {

            Logger.Debug($"Type={type} Message={message} Entity={sequenceItem}");

            var client = Factories.DiscordClientFactory.Create();

            if (client == null) {
                throw new ArgumentNullException("Discord client error");
            }

            var embed = new EmbedBuilder();

            if (templateValues != null) {
                message = templateValues.GetImageFileString(message);
                Logger.Debug($"Message converted to={message}");

            } else {
                Logger.Debug("No image");
            }

            var target = sequenceItem.GetDSOContainer();
            if (target?.Target?.TargetName != null) {
                embed.AddField("Target", target.Target.TargetName);
            }

            var sequence = sequenceItem.GetRootContainer();
            if (sequence?.Name != null) {
                embed.AddField("Sequence Name", sequence.Name);
            }

            if (type == MessageType.Information) {
                embed.Color = Color.Blue;
            } else if (type == MessageType.Error) {
                embed.Color = Color.Red;

                if (sequenceItem?.Name != null) {
                    embed.AddField("Failing Step", sequenceItem.Name);
                }

                var issues = FailureHelper.GetReasons(sequenceItem);
                if (exception != null) {
                    issues.Add(exception.ToString());
                }
                embed.AddField("Issues", string.Join("\n\n", issues));
            }

            embed.Timestamp = DateTime.UtcNow;

            if (cancelToken.IsCancellationRequested)
                return;

            if (attachedImage != null) {
                using (var tempFile = new TemporaryImageFileWriter(attachedImage)) {
                    embed.ImageUrl = $"attachment://{Path.GetFileName(tempFile.Filename)}";
                    var embeds = new List<Embed> { embed.Build() };
                    await client.SendFileAsync(tempFile.Filename, text: message, embeds: embeds);
                }
            } else {
                var embeds = new List<Embed> { embed.Build() };
                await client.SendMessageAsync(text: message, embeds: embeds);
            }
            client.Dispose();

            Logger.Debug($"Sent message: {message}");
        }
    }
}
