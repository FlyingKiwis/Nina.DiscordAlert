using NINA.DiscordAlert.Util;
using NUnit.Framework;


[TestFixture]
public class BaseTextFixture {
    [SetUp]
    public void Setup() {
        Helpers.ClearHelpers();
        Factories.ClearFactories();
    }
}

