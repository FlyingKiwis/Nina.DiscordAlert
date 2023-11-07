using NUnit.Framework;

namespace DiscordAlert.Tests {

    [TestFixture]
    public class DiscordAlertTests {
        [Test]
        public void DiscordWebhookURL_SetThenGet_ReturnsSetValue() {
            var expected = "expected URL";

            var discordAlert = new NINA.DiscordAlert.DiscordAlert();
            discordAlert.DiscordWebhookURL = expected;

            Assert.AreEqual(expected, discordAlert.DiscordWebhookURL);
        }
    }
}
