$version = "1.1.1"

$currentDir = $PWD | select -Expand Path

Set-Location -Path $env:USERPROFILE\code\nina.plugin.manifests\tools

.\CreateManifest.ps1 -createArchive -includeAll -file "$env:USERPROFILE\AppData\Local\NINA\Plugins\Discord Alert\Discord Alert.dll" -installerUrl https://github.com/FlyingKiwis/Nina.DiscordAlert/releases/download/v$version/Discord.Alert.zip -Tags "Discord,Alert,Message,Broadcast,Webhook"

Set-Location -Path $currentDir