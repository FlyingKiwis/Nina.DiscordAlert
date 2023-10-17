using Discord;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NINA.DiscordAlert.DiscordWebhook {
    public interface IDiscordWebhookClient : IDisposable
    {
        Task SendMessageAsync(string text = null, bool isTTS = false, IEnumerable<Embed> embeds = null, string username = null, string avatarUrl = null, RequestOptions options = null, AllowedMentions allowedMentions = null, MessageComponent components = null, MessageFlags flags = MessageFlags.None, ulong? threadId = null);

        Task SendFileAsync(string filename, string text = null, IEnumerable<Embed> embeds = null);
    }
}
