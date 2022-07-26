using NINA.Sequencer.Container;
using NINA.Sequencer.SequenceItem;
using NINA.Core.Utility.Notification;
using System.Threading;
using System.Threading.Tasks;
using DrewMcdermott.NINA.DiscordAlert.Properties;

namespace DrewMcdermott.NINA.DiscordAlert.Util {
    public class DiscordHelper 
    {
        public static void SendMessage(string message, ISequenceItem sequenceItem, CancellationToken cancelToken, string mention = null) 
        {
            var url = Properties.Settings.Default.DiscordWebhookURL;

            Notification.ShowInformation(message);
            Notification.ShowInformation(sequenceItem.Name ?? "");
            Notification.ShowInformation(sequenceItem.RootContainer().Name ?? "");
            Notification.ShowInformation(sequenceItem.TargetContainer()?.Target?.TargetName ?? "");
            Notification.ShowInformation(url);
        }
    }
}
