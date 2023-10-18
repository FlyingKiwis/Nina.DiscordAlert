using NINA.DiscordAlert.Images;
using NINA.Sequencer;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Media.Imaging;
using System;

namespace NINA.DiscordAlert.DiscordWebhook {
    public interface IDiscordHelper {
        Task SendMessage(MessageType type, string message, ISequenceEntity sequenceItem, CancellationToken cancelToken, DiscordImageDetails image = null, Exception exception = null);
    }
}
