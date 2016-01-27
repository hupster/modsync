# modsync
Minecraft Mod Sync tool

Modsync is a small tool that can:
- fully install Minecraft using configurable settings and installers, without user intervention and with very few dependencies
- keep the installation in sync with the configured server, hosting the mods, resourcepacks and version settings

It can be used as a Minecraft pre-launcher. It will install everything required, and then start the Minecraft launcher. The installers to use and directories to sync are configurable from the server. When all checks are enabled, the tool can install Java, Minecraft with Forge complete with mods, resourcepacks and shaderpacks, without user intervention. Once installed, changing any of the versions in the config file of the server will result in an automated re-install at clients when they run the tool.

Now there is the security risk of having you Minecraft directory accessed by the tool. That's why I provide the source code on Github, and encourage everyone to compile your own version, and share it with users that trust you.

In essence, the tool requires a link to a FTP server, containing a config file, the directory with the mods to keep in sync, and installers to use when stuff is missing. The tool only approaches the configured FTP server, and no other sites or locations. Local file access is limited to the Minecraft directory, an optional launcher in program files. It will only remove or overwrite files in the directories listed in config file. So the person in control of the FTP server, has full control of the Minecraft installation at the clients. The tool does not require Administator rights, however the Java installer does.

**Features**

This is what the tool does, in approximately the same order.
Note that any of the listed actions can be disabled by leaving the corresponding configuration setting empty.

- Check for updates

Checks for updates of the tool itself. If the version set by config key 'ToolVersion' is higher than the version running, then download the file set by 'ToolDownloadFile' from the FTP server and replace the running executable with it. Make sure that the AssemblyFileVersion (set in Properties\AssemblyInfo.cs) matches 'ToolVersion'.

- Check Java

Checks Java installation for a specific version set by 'JavaVersion'. If present, use this version for Minecraft. This resolves issues with mods not working with the latest version of Java. If not present, download installer 'JavaDownloadFile' from the FTP server and execute it.

- Check Minecraft

Checks presence of a Minecraft launcher in either Program Files (x86)\Minecraft or the Minecraft folder. If not present, download launcher 'MinecraftDownloadFile' from the FTP server and put it in the Minecraft folder. The 'MinecraftDownloadFile' should be the version without installer that you can get [here](https://minecraft.net/download).

- Check Forge

Checks if 'ForgeVersion' is installed (use the version shown below at Settings or newer). If not, create default profile for 'MinecraftVersion' if required, download and install 'ForgeDownloadFile', and update launcher profile to use 'ForgeVersion'. Make sure that 'ForgeDownloadFile' does install 'ForgeVersion'. Use the .jar installer from [Forge](http://files.minecraftforge.net/minecraftforge/).

- Sync Folders

All folders listed in the comma-separated list set by 'SyncFolders' are synced with the FTP server.
Typical folders to sync are:

config: containing mod config files

mods: containing Forge mods

resourcepacks: containing resource packs, selectable in the game by Options\Resource Packs

shaderpacks: containing shaders, for use with the GLSL Shaders Mod, selectable in the game by Options\Shaders

The tool downloads any missing file, and removes any local file that is missing on the FTP server, showing progress and actions during sync.
If the “config" folder is listed, it is synced with different settings: don't remove any local file, just download any file that is newer on the server, or missing locally. This allows updating specific mod setting files, to fix incompatibilities.

If command line option “updateserver” is given and 'SyncAllowUpload' is set to “true”, perform the sync the other way round: Upload any file missing on the FTP server, and remove any file missing locally. The “config" folder is skipped in this case when listed, also the game is not started when done.

- Check server list

Checks if there is a server with the address 'ServerAddress' in the multiplayer server list. If not, adds an entry with name 'ServerName'.

- Start Minecraft

After all checks, start the game by executing the launcher. This allows for the tool to be installed as a Minecraft launcher, checking for new mods at every start.

**Configuration**

The tool requires a link to a FTP server, and some additional settings. The settings can be retrieved from the FTP server, as soon as the FTP link is known.

- FTP link

At first start, the tool will ask for the required FTP connection info: user, password, server address, server port and root folder. It will write the entered values to a file called “modsyncftp.xml” in the minecraft directory, and use this file further on.

It is also possible to hard-code the FTP info in Settings.cs, or to supply an FTP link as a command line argument. Hard-coded values are overwritten if a stored file is present, but a command line FTP link is used over the file when provided in the correct syntax: ftp://user:pass@server:port/folder

- Settings

The settings are supplied by a file “modsync.xml” on the FTP server, for example:

```
<?xml version="1.0" encoding="utf-8"?>
<Settings xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <ToolVersion>1.0.1.1</ToolVersion>
  <ToolDownloadFile>modsync.exe</ToolDownloadFile>
  <JavaVersion>1.7</JavaVersion>
  <JavaDownloadFile>jre-7u75-windows-i586-iftw.exe</JavaDownloadFile>
  <MinecraftVersion>1.7.10</MinecraftVersion>
  <MinecraftDownloadFile>Minecraft Launcher.exe</MinecraftDownloadFile>
  <ForgeVersion>1.7.10-Forge10.13.4.1614-1.7.10</ForgeVersion>
  <ForgeDownloadFile>forge-1.7.10-10.13.4.1614-1.7.10-installer.jar</ForgeDownloadFile>
  <SyncFolders>mods,resourcepacks,shaderpacks,config</SyncFolders>
  <SyncAllowUpload>true</SyncAllowUpload>
  <ServerName>My Minecraft Server</ServerName>
  <ServerAddress>192.168.0.1</ServerAddress>
</Settings>
```

All settings are referenced to in the features list above.
If any settings are missing or left empty, the corresponding checks are skipped.

It is also possible to hard-code the values in Settings.cs. These are overwritten by any values present in the file on the FTP server.

**FTP server**

The FTP server should contain the files referenced to in modsync.xml, the config file itself, and the directories to keep in sync. For the settings listed above the file structure would be:

```
config/
mods/
resourcepacks/
shaderpacks/
modsync.exe
modsync.xml
jre-7u75-windows-i586-iftw.exe
MinecraftLauncher.exe
forge-1.7.10-10.13.4.1614-1.7.10-installer.jar
```

Write access is only needed for the command line option “updateserver” to work, and can be omitted otherwise.

**Rebuilding**

To rebuild modsync:
- Install Microsoft Visual Studio, version [2008 sp1](http://download.microsoft.com/download/E/8/E/E8EEB394-7F42-4963-A2D8-29559B738298/VS2008ExpressWithSP1ENUX1504728.iso) or newer.
- Open the project by clicking modsync.csproj
- Rebuilding requires [edtFTPnet](https://enterprisedt.com/products/edtftpnet/) from EnterpriseDT. To fix the missing reference to edtFTPnet.dll, download the zip and copy the edtFTPnet.dll from the bin folder to the modsync project folder
- Click Build, Build solution
- Use modsync.exe from the folder bin\Release

**Download**

Download the binary from [here](https://github.com/hupster/modsync/blob/master/bin/Release/modsync.exe?raw=true).
The application requires [.NET 3.5 SP1](https://www.microsoft.com/en-us/download/details.aspx?id=22) to run.
Modsync will ask for FTP connection details on the first run.

**Disclaimer**

No warranty, no responsibility, code provided as is, etc.

**License**

Because of the dependency on edtFTPnet, the code falls under the LGPL license, refer to LICENSE.
