using NINA.Sequencer.SequenceItem;
using System.Threading;
using System.Threading.Tasks;
using Discord.Webhook;
using Discord;
using System.Collections.Generic;
using System;

namespace NINA.DiscordAlert.Util {
    public class DiscordHelper 
    {
        static DiscordHelper() 
        {
            var url = Properties.Settings.Default.DiscordWebhookURL;
            try {
                _webhookClient = new DiscordWebhookClient(url);
            }
            catch (Exception ex) { 
                _webhookClient = null;
                _webhookException = ex;
            }
        }

        private static DiscordWebhookClient _webhookClient;
        private static Exception _webhookException;

        public static async Task SendMessage(MessageType type, string message, ISequenceItem sequenceItem, CancellationToken cancelToken) {

            if (_webhookClient == null)
                throw new ArgumentException("Issue occured while initializing webhook client", _webhookException);

            var embed = new EmbedBuilder();

            var target = sequenceItem.TargetContainer();
            if (target != null) {
                embed.AddField("Target", target.Target.TargetName);
            }

            var sequence = sequenceItem.RootContainer();
            if (sequence != null) {
                embed.AddField("Sequence Name", sequence.Name);
            }

            if (type == MessageType.Information) {
                embed.Color = Color.Blue;
            } else if (type == MessageType.Error) {
                embed.Color = Color.Red;

                embed.AddField("Failing Step", sequenceItem.ToString());

                var issues = FailureHelper.GetReasons(sequenceItem);
                embed.AddField("Issues", string.Join("\n\n", issues));
            }

            var embeds = new List<Embed> { embed.Build() };

            if (cancelToken.IsCancellationRequested)
                return;

            await _webhookClient.SendMessageAsync(text: message, embeds: embeds);
        }
    }
}
