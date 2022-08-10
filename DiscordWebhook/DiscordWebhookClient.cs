using Discord;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NINA.DiscordAlert.DiscordWebhook {
    public class DiscordWebhookClient : IDiscordWebhookClient {
        public DiscordWebhookClient(string url) {
            _discordWebhookClient = new Discord.Webhook.DiscordWebhookClient(url);
        }

        private Discord.Webhook.DiscordWebhookClient _discordWebhookClient;

        public async Task SendMessageAsync(string text = null, bool isTTS = false, IEnumerable<Embed> embeds = null, string username = null, string avatarUrl = null, RequestOptions options = null, AllowedMentions allowedMentions = null, MessageComponent components = null, MessageFlags flags = MessageFlags.None, ulong? threadId = null) {
            await _discordWebhookClient.SendMessageAsync(text, isTTS, embeds, username, avatarUrl, options, allowedMentions, components, flags, threadId);
        }

        public async Task SendSimpleMessageAsync(string text = null, IEnumerable<Embed> embeds = null) {
            await SendMessageAsync(text: text, embeds: embeds);
        }
    }
}
