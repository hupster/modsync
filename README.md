# modsync
Minecraft Mod Sync tool

Modsync is a small tool for automated modded Minecraft install and maintenance on remote Windows computers. Users require a single binary to install and play Minecraft. Controlled by a configution file on the server, they can receive automatic updates to Java, Fabric or Forge, mods and more. It supports Vanilla, Forge and Fabric.

It was created to easily get kids playing Minecraft on a custom multiplayer server, being able to add and update mods without having to update all the clients.

**Description**

The tool requires details of a FTP server, from where it downloads a configuration file. All required files are downloaded from the configured FTP server, no other sites or locations are used. Local file access is limited to the Minecraft directory. No Administator rights are required except when prompted during Java install.

**Features**

This is what the tool does, in approximately the same order.
Note that any of the listed actions can be disabled by leaving the corresponding configuration setting empty.

- Check for updates

Checks for updates of the tool itself. If the version set by config key 'ToolVersion' is higher than the version running, then download the file set by 'ToolDownloadFile' from the FTP server and replace the running executable with it. Make sure that the AssemblyFileVersion (set in Properties\AssemblyInfo.cs) matches 'ToolVersion'.

- Check Java

Checks Java installation for a specific version set by 'JavaVersion', having architecture 'JavaArchitecture' (32 or 64). If present, use this version for Minecraft. This resolves issues with mods not working with the latest version of Java. If not present, download installer 'JavaDownloadFile' from the FTP server and execute it.

- Check Minecraft

Checks presence of a Minecraft launcher in either Program Files (x86)\Minecraft or the Minecraft folder. If not present, download launcher 'MinecraftDownloadFile' from the FTP server and put it in the Minecraft folder. The 'MinecraftDownloadFile' should be the version without installer that you can get [here](https://minecraft.net/download).

- Check Fabric or Forge

Depending on setting 'MinecraftMode':

Vanilla: Create default profile for 'MinecraftVersion' if required, runs vanilla Minecraft (no mods).

Fabric: Checks if 'FabricVersion' is installed. If not, download and install 'FabricDownloadFile', and update launcher profile to use 'FabricVersion'. Make sure that 'FabricDownloadFile' does install 'FabricVersion'. Use the .json file that is placed in the 'installdir\versions\fabric-loader-xyz\' directory by the Client install from [Fabric](https://fabricmc.net/use/installer/).

Forge: Checks if 'ForgeVersion' is installed (use the version shown below at Settings or newer). If not, download and install 'ForgeDownloadFile', and update launcher profile to use 'ForgeVersion'. Make sure that 'ForgeDownloadFile' does install 'ForgeVersion'. Use the .jar installer from [Forge](http://files.minecraftforge.net/minecraftforge/).

- Sync Folders

All folders listed in the comma-separated list set by 'SyncFolders' are synced with the FTP server.
Typical folders to sync are:

__config__: containing mod config files

__mods__: containing Forge mods

__resourcepacks__: containing resource packs, selectable in the game by Options\Resource Packs

__shaderpacks__: containing shaders, for use with the GLSL Shaders Mod, selectable in the game by Options\Shaders

__cachedImages/skins__: containing skins, for use with the OfflineSkins mod, see skin section below

The tool normally downloads any missing file, and removes any local file that is missing on the FTP server, showing progress and actions during sync. If the “config" folder is listed, it is synced with different settings: don't remove any local file, just download any file that is newer on the server, or missing locally. This allows updating specific mod setting files, to fix incompatibilities. Also the "cachedImages/skins" folder has different sync settings: files from this folder are also uploaded to the server. This requires 'SyncAllowUpload' to be set to “true”.

If command line option “updateserver” is given and 'SyncAllowUpload' is set to “true”, perform the sync the other way round: Upload any file missing on the FTP server, and remove any file missing locally. The “config" folder is skipped in this case when listed, also the game is not started when done.

- Offline skin support

The mod OfflineSkins allows using your skin on an offline server. The mod sync tool allows users to change their skin, by checking for modifications in the local skin files (folder "cachedImages/skins") and uploading these files to the server. Any uploaded skin will be available to the other users after they run the sync tool again, which will download the new skin.

The skin files are generic minecraft skins that can be created by for instance skincraft. To facilitate changing your skin, the sync tool moves all skin files (very small png files) from the desktop to the minecraft directory before syncing. So the user just has to put the file on the desktop. The user is responsible for giving the skin the correct name: <username>.png

- Check server list

Checks if there is a server with the address 'ServerAddress' in the multiplayer server list. If not, adds an entry with name 'ServerName'.

- Start Minecraft

After all checks, start the game by executing the launcher. This allows for the tool to be installed as a Minecraft launcher, syncing folders at every start.

**Configuration**

The tool requires a link to a FTP server, and some additional settings. The settings can be retrieved from the FTP server, as soon as the FTP link is known.

- FTP link

At first start, the tool will ask for the required FTP connection info: user, password, server address, server port and root folder. It will write the entered values to a file called “modsyncftp.xml” in the minecraft directory, and use this file further on.

It is also possible to hard-code the FTP info in Settings.cs, or to supply an FTP link as a command line argument. Hard-coded values are overwritten if a stored file is present, but a command line FTP link is used over the file when provided in the correct syntax: ftp://user:pass@server:port/folder

- Settings

The settings are supplied by a file “modsync.xml” on the FTP server.

Example for Fabric:

```
<?xml version="1.0" encoding="utf-8"?>
<Settings xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <ToolVersion>1.0.1.9</ToolVersion>
  <ToolDownloadFile>modsync.exe</ToolDownloadFile>
  <JavaVersion>17.0.1.12</JavaVersion>
  <JavaArchitecture>x64</JavaArchitecture>
  <JavaDownloadFile>OpenJDK17U-jre_x64_windows_hotspot_17.0.1_12.msi</JavaDownloadFile>
  <JavaArguments>-Xms1G -Xmx4G</JavaArguments>
  <MinecraftVersion>1.18.1</MinecraftVersion>
  <MinecraftDownloadFile>Minecraft Launcher.exe</MinecraftDownloadFile>
  <MinecraftMode>Fabric</MinecraftMode>
  <FabricVersion>fabric-loader-0.12.12-1.18.1</FabricVersion>
  <FabricDownloadFile>fabric-loader-0.12.12-1.18.1.json</FabricDownloadFile>
  <SyncFolders>mods</SyncFolders>
  <SyncAllowUpload>true</SyncAllowUpload>
  <ServerName>My Minecraft Server</ServerName>
  <ServerAddress>192.168.0.1</ServerAddress>
</Settings>
```

Example for Forge:

```
<?xml version="1.0" encoding="utf-8"?>
<Settings xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <ToolVersion>1.0.1.9</ToolVersion>
  <ToolDownloadFile>modsync.exe</ToolDownloadFile>
  <JavaVersion>1.8</JavaVersion>
  <JavaArchitecture>x64</JavaArchitecture>
  <JavaDownloadFile>jre-8u251-windows-x64.exe</JavaDownloadFile>
  <JavaArguments>-Xmx1536M -XX:+UseConcMarkSweepGC -XX:+CMSIncrementalMode -XX:-UseAdaptiveSizePolicy -Xmn128M</JavaArguments>
  <MinecraftVersion>1.12.2</MinecraftVersion>
  <MinecraftDownloadFile>Minecraft Launcher.exe</MinecraftDownloadFile>
  <MinecraftMode>Forge</MinecraftMode>
  <ForgeVersion>1.12.2-forge-14.23.5.2854</ForgeVersion>
  <ForgeDownloadFile>forge-1.12.2-14.23.5.2854-installer.jar</ForgeDownloadFile>
  <SyncFolders>mods,resourcepacks,shaderpacks,config,cachedImages/skins</SyncFolders>
  <SyncAllowUpload>true</SyncAllowUpload>
  <ServerName>My Minecraft Server</ServerName>
  <ServerAddress>192.168.0.1</ServerAddress>
</Settings>
```

All settings are referenced to in the features list above.
If any settings are missing or left empty, the corresponding checks are skipped.

It is also possible to hard-code the values in Settings.cs. These are overwritten by any values present in the file on the FTP server.

**FTP server**

Modsync will ask for FTP connection details on the first run. It is also possible to hard-code the values in Settings.cs.
The FTP server should contain the files referenced to in modsync.xml, the config file itself, and the directories to keep in sync.

Example files for Fabric:

```
mods/
modsync.exe
modsync.xml
OpenJDK17U-jre_x64_windows_hotspot_17.0.1_12.msi
MinecraftLauncher.exe
fabric-loader-0.12.12-1.18.1.json
```

Example files for Forge:

```
config/
mods/
resourcepacks/
shaderpacks/
cachedImages/skins/
modsync.exe
modsync.xml
jre-8u144-windows-x64.exe
MinecraftLauncher.exe
forge-1.12.1-14.22.1.2478-installer.jar
```

Write access is only needed for the command line option “updateserver” to work, and can be omitted otherwise.

**Rebuilding**

To rebuild modsync:
- Install Microsoft Visual Studio Express 2015 for Windows Desktop or newer.
- Open the project by clicking modsync.csproj
- Rebuilding requires [edtFTPnet](https://enterprisedt.com/products/edtftpnet/) from EnterpriseDT. To fix the missing reference to edtFTPnet.dll, download the zip and copy the edtFTPnet.dll from the bin folder to the modsync project folder
- Click Build, Build solution
- Use modsync.exe from the folder bin\Release

**Download**

Download the binary from [here](https://github.com/hupster/modsync/blob/master/bin/Release/modsync.exe?raw=true).
The application requires the .NET framework 4 to run and has no further requirements.

**Disclaimer**

No warranty, no responsibility, code provided as is, etc.

**License**

Because of the dependency on edtFTPnet, the code falls under the LGPL license, refer to LICENSE.
