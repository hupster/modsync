# modsync
Minecraft Mod Sync tool

Greetings Minecrafters!

I was maintaining a Minecraft server for a bunch of kids, and wanted to expand it with a few mods. Now I didn't like the idea of me going around with a USB stick installing the mods on all of the computers, every time I would add a mod. And I was surprised to find that there seems to be no easy tool available to do this automatically.

So I've written a little program that can be installed on every computer, that will keep the mod directory in sync with those on the server. When mods are added or removed on the server, the tool does the same for the clients. Over time I've added many checks, which eventually have created a tool that can install a modded Minecraft client on any Windows computer with just a few clicks.

Now there is the security risk of having you Minecraft directory accessed by the tool. That's why I provide the source code on Github, and encourage everyone to compile your own version, and share it with users that trust you.

In essence, the tool requires a link to a FTP server, containing a config file, the directory with the mods to keep in sync, and installers to use when stuff is missing. The tool only approaches the configured FTP server, and no other sites or locations. Local file access is limited to the Minecraft directory, an optional launcher in program files, and an optional desktop shortcut.

**Features**

Note that any if the listed actions can be disabled by leaving the corresponding configuration setting empty.

- Check Java
Checks Java installation for a specific version set by <JavaVersion>. If present, use this version for Minecraft. This resolves issues with mods not working with the latest version of Java. If not present, download installer <JavaDownloadFile> from the FTP server and execute it.

- Check Minecraft
Checks presence of a Minecraft launcher in either Program Files (x86)\Minecraft or the Minecraft folder. If not present, download launcher <MinecraftDownloadFile> from the FTP server and put it in the Minecraft folder. The <MinecraftDownloadFile> should be the version without installer that you can get [here](https://minecraft.net/download).

- Check Forge
Checks if <ForgeVersion> is installed. If not, download and execute <ForgeDownloadFile>. Updates launcher profile to use <MinecraftVersion> before Forge is installed, so it will be downloaded by the installer. Updates launcher profile to use <ForgeVersion> after Forge is installed. This mechanism allows for pushing Forge updates by updating the settings. Make sure that <ForgeDownloadFile> does install <ForgeVersion>. It can be the .exe or .jar you can get at [Forge](http://files.minecraftforge.net/minecraftforge/).

- Check Mods
Sync the local Minecraft mods folder with <ModsSyncFolder> on the FTP server. Download any missing file locally, and remove any file missing on the FTP server. Show progress and actions during sync.
If command line option “updateserver” is given and <ModsSyncAllowPush> is set to “true”, perform the sync the other way round: Upload any file missing on the FTP server, and remove any file missing locally. Starting the game is skipped in this case.

- Check server list
Checks if there is a server with the address <ServerAddress> in the multiplayer server list. If not, adds an entry with name <ServerName>.

- Check desktop shortcut
Skipped unless application is deployed through Clickonce (see below).
Place shortcut on desktop named <DesktopShortcutToCreate>.
Remove any shortcut named <DesktopShortcutToRemove>.

- Start Minecraft
After all checks, start the game by executing the launcher. This allows for the tool to be installed as a Minecraft launcher, checking for new mods at every start.

**Configuration**

The tool requires a link to a FTP server, and some additional settings. The settings can be retrieved from the FTP server, as soon as the FTP link is known.

1. FTP link
At first start, the tool will ask for the required FTP connection info: user, password, server address, server port and root folder. It will write the entered values to a file called “modsyncftp.xml” in the minecraft directory, and use this file further on.

It is also possible to hard-code the FTP info in Settings.cs, or to supply an FTP link as a command line argument. Hard-coded values are overwritten if a stored file is present, but a command line FTP link is used over the file when provided in the correct syntax: ftp://user:pass@server:port/folder

2. Settings
The settings are supplied by a file “modsync.xml” on the FTP server, for example:

```
<?xml version="1.0" encoding="utf-8"?>
<Settings xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <JavaVersion>1.7</JavaVersion>
  <JavaDownloadFile>jre-7u75-windows-i586-iftw.exe</JavaDownloadFile>
  <MinecraftVersion>1.7.10</MinecraftVersion>
  <MinecraftDownloadFile>MinecraftLauncher.exe</MinecraftDownloadFile>
  <ForgeVersion>1.7.10-Forge10.13.0.1207</ForgeVersion>
  <ForgeDownloadFile>forge-1.7.10-10.13.0.1207-installer.jar</ForgeDownloadFile>
  <ModsSyncFolder>mods</ModsSyncFolder>
  <ModsSyncAllowPush>true</ModsSyncAllowPush>
  <DesktopShortcutToRemove>Minecraft.lnk</DesktopShortcutToRemove>
  <DesktopShortcutToCreate>Minecraft.appref-ms</DesktopShortcutToCreate>
  <ServerName>My Minecraft Server</ServerName>
  <ServerAddress>192.168.0.1</ServerAddress>
</Settings>
```

All settings are referenced to in the features list above.
If any settings are missing or left empty, the corresponding checks are skipped.

It is also possible to hard-code the values in Settings.cs. These are overwritten by any values present in the file on the FTP server.

**FTP server**

The FTP server should contain the files referenced to in modsync.xml, the file itself, and the mod directory to keep in sync. For the settings listed above the file structure would be:

```
mods/
modsync.xml
jre-7u75-windows-i586-iftw.exe
MinecraftLauncher.exe
forge-1.7.10-10.13.0.1207-installer.jar
```

Write access is only needed for the command line option “updateserver” to work, and can be omitted otherwise.

**Compiling**

Although you could use the binary attached, I encourage everyone to compile your own.
You could do so by installing Visual Studio, I used version [2008 sp1](http://download.microsoft.com/download/E/8/E/E8EEB394-7F42-4963-A2D8-29559B738298/VS2008ExpressWithSP1ENUX1504728.iso), newer should work too.
- Open the project by clicking modsync.csproj
- The reference to edtFTPnet.dll will be missing, get it at EnterpriseDT: download the zip and copy the edtFTPnet.dll from the bin folder to the modsync project folder
- Click Build, Build solution

**Deployment**

The application has a single dependency outside of .NET 3.5 SP1: [edtFTPnet](https://enterprisedt.com/products/edtftpnet/) from EnterpriseDT. The library is included in the program as an embedded resource, allowing the executable to be run by itself. There are therefore two options for deployment:

1. Distribute the executable
Use the attached file or, preferably, build the application yourself and just copy modsync.exe from the folder bin\Release.

2. Deployment through clickonce
By publishing the application and putting it on a web server, it can perform auto-updates. Also the generated installer can install the .NET framework dependency.

**Disclaimer**

No warranty, no responsibility, code provided as is, etc.

**License**

Because of the dependency on edtFTPnet, the code falls under the LGPL license, refer to LICENSE.
