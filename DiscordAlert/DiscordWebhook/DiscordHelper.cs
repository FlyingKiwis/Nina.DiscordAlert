using System.Threading;
using System.Threading.Tasks;
using Discord;
using System.Collections.Generic;
using System;
using NINA.Sequencer;
using NINA.DiscordAlert.Util;
using NINA.Core.Utility;
using System.Windows.Media.Imaging;
using NINA.Core.Model;

namespace NINA.DiscordAlert.DiscordWebhook {
    public class DiscordHelper : IDiscordHelper {
        public async Task SendMessage(MessageType type, string message, ISequenceEntity sequenceItem, CancellationToken cancelToken, ImagePatterns templateValues = null, string attachedFilename = null, Exception exception = null) {

            Logger.Debug($"Type={type} Message={message} Entity={sequenceItem}");

            using (var client = Factories.DiscordClient.Create()) {

                if (client == null) {
                    throw new ArgumentNullException("Discord client error");
                }

                var embed = new EmbedBuilder();

                if (templateValues != null) {
                    message = templateValues.GetImageFileString(message);
                    Logger.Debug($"Message converted to={message}");

                } else {
                    Logger.Debug("No template replacement set");
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

                if (attachedFilename != null) {
                    embed.ImageUrl = $"attachment://{Helpers.File.GetFileName(attachedFilename)}";
                    var embeds = new List<Embed> { embed.Build() };
                    await client.SendFileAsync(attachedFilename, text: message, embeds: embeds);
                } else {
                    var embeds = new List<Embed> { embed.Build() };
                    await client.SendMessageAsync(text: message, embeds: embeds);
                }
            }

            Logger.Debug($"Sent message: {message}");
        }
    }
}
