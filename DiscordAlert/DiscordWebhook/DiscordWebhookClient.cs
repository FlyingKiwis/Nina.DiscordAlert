using Discord;
using System.Collections.Generic;
using System.Threading.Tasks;
using NINA.Core.Utility;
using System;
using System.Diagnostics.CodeAnalysis;

namespace NINA.DiscordAlert.DiscordWebhook {
    [ExcludeFromCodeCoverage(Justification = "This class is a passthrough to the actual discord client")]
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
            try {
                await _discordWebhookClient.SendMessageAsync(text, isTTS, embeds, username, avatarUrl, options, allowedMentions, components, flags, threadId);
            }
            catch (Exception ex) {
                Logger.Error($"Failed to send message", ex);
            }
        }

        public async Task SendFileAsync(string filename, string text = null, IEnumerable<Embed> embeds = null) {
            Logger.Debug(string.Empty);
            try {
                await _discordWebhookClient.SendFileAsync(filePath: filename, text: text, embeds: embeds);
            }
            catch (Exception ex) {
                Logger.Error($"Failed to send message", ex);
            }
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
