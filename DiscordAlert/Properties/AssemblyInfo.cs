using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// [MANDATORY] The following GUID is used as a unique identifier of the plugin. Generate a fresh one for your plugin!
[assembly: Guid("bbca7e0a-b89f-4e7c-92f1-0d5b24db6911")]

// [MANDATORY] The assembly versioning
//Should be incremented for each new release build of a plugin
[assembly: AssemblyVersion("2.1.2.0")]
[assembly: AssemblyFileVersion("2.1.2.0")]

// [MANDATORY] The name of your plugin
[assembly: AssemblyTitle("Discord Alert")]
// [MANDATORY] A short description of your plugin
[assembly: AssemblyDescription("Sends alerts to discord")]

// The following attributes are not required for the plugin per se, but are required by the official manifest meta data

// Your name
[assembly: AssemblyCompany("Drew McDermott")]
// The product name that this plugin is part of
[assembly: AssemblyProduct("Discord Alert")]
[assembly: AssemblyCopyright("Copyright © 2023 Drew McDermott")]

// The minimum Version of N.I.N.A. that this plugin is compatible with
[assembly: AssemblyMetadata("MinimumApplicationVersion", "3.0.0.1062")]

// The license your plugin code is using
[assembly: AssemblyMetadata("License", "MPL-2.0")]
// The url to the license
[assembly: AssemblyMetadata("LicenseURL", "https://www.mozilla.org/en-US/MPL/2.0/")]
// The repository where your pluggin is hosted
[assembly: AssemblyMetadata("Repository", "https://github.com/FlyingKiwis/Nina.DiscordAlert")]

// The following attributes are optional for the official manifest meta data

//[Optional] Your plugin homepage URL - omit if not applicaple
[assembly: AssemblyMetadata("Homepage", "https://github.com/FlyingKiwis/Nina.DiscordAlert")]

//[Optional] Common tags that quickly describe your plugin
[assembly: AssemblyMetadata("Tags", "Discord,Alert,Message,Broadcast,Webhook")]

//[Optional] A link that will show a log of all changes in between your plugin's versions
[assembly: AssemblyMetadata("ChangelogURL", "https://github.com/FlyingKiwis/Nina.DiscordAlert/blob/main/CHANGELOG.md")]

//[Optional] The url to a featured logo that will be displayed in the plugin list next to the name
[assembly: AssemblyMetadata("FeaturedImageURL", "")]
//[Optional] A url to an example screenshot of your plugin in action
[assembly: AssemblyMetadata("ScreenshotURL", "")]
//[Optional] An additional url to an example example screenshot of your plugin in action
[assembly: AssemblyMetadata("AltScreenshotURL", "")]
//[Optional] An in-depth description of your plugin
[assembly: AssemblyMetadata("LongDescription", @"# Discord Alert

This is a to allow for in-sequence messages sent to a discord server via webhook.

See more about [Discord's Webhooks here](https://support.discord.com/hc/en-us/articles/228383668-Intro-to-Webhooks)

For more information please see [the readme](https://github.com/FlyingKiwis/Nina.DiscordAlert/blob/main/README.md)

# Suggestion Welcome

I'm open to suggestions, please submit one via [GitHub's issue tracker](https://github.com/FlyingKiwis/Nina.DiscordAlert/issues).

# Contact

I'm in the NINA discord server as Kiwi🥝

# 3rd Party Licences

- [Discord.Net](https://github.com/discord-net/Discord.Net/blob/dev/LICENSE) is licensed under the MIT license")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]
// [Unused]
[assembly: AssemblyConfiguration("")]
// [Unused]
[assembly: AssemblyTrademark("")]
// [Unused]
[assembly: AssemblyCulture("")]