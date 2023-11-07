# Discord Alert

This is a plugin for [NINA](https://nighttime-imaging.eu) to allow for in-sequence messages sent to a discord server via webhook

See more about [Discord's Webhooks here](https://support.discord.com/hc/en-us/articles/228383668-Intro-to-Webhooks)

# Sequence items

- Discord: Send message - Will send a message when that point in the sequence is reached
- Discord: Send after failure - Triggered action, send a discord message after a detected failure.
- Discord: Send after image - Sends a message after each image, attaches an auto-streched image preview

# Mentioning

Using mentions in webhooks are a little different than regular typing into a discord client so this is a bit of a tutorial.

## @everyone / @here

"@everyone" and "@here" are the same, you can just type them in as normal and the expected result will happen

## Users and Roles

Users and Roles require the ID in order to mention them, are a couple of ways to get it

### Method 1: Do a backslah mention

In the server you want to mention the user or role type \@mention (where mention would be the user/role name) for example if I have a group called NINA I would type \@NINA.
This will output a line of text that will look like <@0000000000> or <@&0000000000>, copy that whole line and put it in the message text box and that will mention the user / group

### Method 2: Dev mode

- Step 1: Click on the gear in discord and go to the "Advanced" item, turn "Developer Mode" on
- Step 2: Right click on a user or a role and select Copy User ID / Copy Role ID.
- Step 3: You will need to surround the ID you copied based on the type (replace the [ID] below with what you copied in step 2)
    - For users: `<@[ID]>`
    - For roles: `<@&[ID]>`

# Patterns

You can use the NINA patterns in your messages, simply use the dolar sign notation in your message (For example: "`Using telescope $$TELESCOPE$$`").  You can find examples of these in NINA by going to Options > Imaging and look at the patterns on the left column.
See the below table for support:

| Pattern | Send after image | Send message / Send after failure |
|-------------:|------------------|-----------------------------------|
|Time Category|✅|✅|
|Camera Category|✅|✅|
|`$$SEQUENCETITLE$$`|✅|✅|
|`$$TARGETNAME$$`|✅|✅|
|Others in Image Category|✅|❌|
|`$$FILTER$$`|✅|✅|
|Focuser Category|✅|✅|
|Guider Category|✅|❌|
|`$$ROTATORANGLE$$`|✅|✅|
|`$$SQM$$`|✅|❌|
|`$$TELESCOPE$$`|✅|✅|
|Placeholders added by other plugins|✅|❌|

# Suggestion Welcome

I'm open to suggestions, please submit one via [GitHub's issue tracker](https://github.com/FlyingKiwis/Nina.DiscordAlert/issues).

# Contact

I'm in the NINA discord server as Kiwi🥝

# Legal

- This plugin is licensed under the [Mozilla Public License](https://www.mozilla.org/en-US/MPL/2.0/) (MPL v2)

## Licenses of libraries used by this software

- [NINA](https://bitbucket.org/Isbeorn/nina/src/master/LICENSE.txt) - MPL v2
- [Discord.Net](https://github.com/discord-net/Discord.Net/blob/dev/LICENSE) - [MIT license](https://choosealicense.com/licenses/mit/)