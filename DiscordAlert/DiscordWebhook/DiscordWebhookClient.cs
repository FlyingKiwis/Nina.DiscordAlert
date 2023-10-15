using Discord;
using System.Collections.Generic;
using System.Threading.Tasks;
using NINA.Core.Utility;
using System;

namespace NINA.DiscordAlert.DiscordWebhook {
    public class DiscordWebhookClient : IDiscordWebhookClient {
        public DiscordWebhookClient(string url) {
            Logger.Debug($"URL={url}");
            try {
                _discordWebhookClient = new Discord.Webhook.DiscordWebhookClient(url);
            }
            catch (Exception ex) {
                Logger.Error("Error attempting to create discord client", ex);
            }
        }

        private Discord.Webhook.DiscordWebhookClient _discordWebhookClient;

        public async Task SendMessageAsync(string text = null, bool isTTS = false, IEnumerable<Embed> embeds = null, string username = null, string avatarUrl = null, RequestOptions options = null, AllowedMentions allowedMentions = null, MessageComponent components = null, MessageFlags flags = MessageFlags.None, ulong? threadId = null) {
            Logger.Debug(string.Empty);
            await _discordWebhookClient.SendMessageAsync(text, isTTS, embeds, username, avatarUrl, options, allowedMentions, components, flags, threadId);
        }

        public async Task SendSimpleMessageAsync(string text = null, IEnumerable<Embed> embeds = null) {
            Logger.Debug(string.Empty);
            await SendMessageAsync(text: text, embeds: embeds);
        }

        public void Dispose() {
            try {
                _discordWebhookClient?.Dispose();
            } catch { 
                _discordWebhookClient = null;
            }
        }
    }
}
