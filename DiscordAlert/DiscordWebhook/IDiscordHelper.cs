using NINA.Sequencer;
using System.Threading.Tasks;
using System.Threading;
using System;
using NINA.Core.Model;

namespace NINA.DiscordAlert.DiscordWebhook {
    public interface IDiscordHelper {
        Task SendMessage(MessageType type, string message, ISequenceEntity sequenceItem, CancellationToken cancelToken, ImagePatterns templateValues = null, string attachedFilename = null, Exception exception = null);
    }
}
