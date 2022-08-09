using NINA.Sequencer.SequenceItem;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using System.Collections.Generic;
using System;
using NINA.Sequencer;

namespace NINA.DiscordAlert.Util {
    public class DiscordHelper 
    {
        public static async Task SendMessage(MessageType type, string message, ISequenceEntity sequenceItem, CancellationToken cancelToken, Exception exception = null) {

            var client = Resources.Client;

            if(client == null) {
                throw new ArgumentNullException("Discord client error");
            }

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

                embed.AddField("Failing Step", sequenceItem.Name);

                var issues = FailureHelper.GetReasons(sequenceItem);
                if (exception != null) {
                    issues.Add(exception.ToString());
                }
                embed.AddField("Issues", string.Join("\n\n", issues));
            }

            var embeds = new List<Embed> { embed.Build() };

            if (cancelToken.IsCancellationRequested)
                return;

            await client.SendSimpleMessageAsync(text: message, embeds: embeds);
        }
    }
}
