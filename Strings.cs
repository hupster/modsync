using System;
using System.Collections.Generic;
using System.Globalization;

namespace modsync
{
    static class Strings
    {
        // very simple string localization
        public static string Get(string name)
        {
            try
            {
                switch (CultureInfo.CurrentCulture.TwoLetterISOLanguageName)
                {
                    case "nl":
                        return NL[name];
                    default:
                        return EN[name];
                }
            }
            catch (Exception)
            {
                return "[?]";
            }
        }
        
        // dutch
        static Dictionary<string, string> NL = new Dictionary<string, string>()
        {
            {"AutoUpdate", "Updating ..."},
            {"AutoUpdateDone", "Start nog een keer!"},
            {"AutoUpdateError", "Update error: "},
            {"AutoUpdateDotnet", "Installeer .NET 3.5 sp1"},
            {"FtpInputLink", "Geef de FTP server gegevens in"},
            {"FtpBadCommandLine", "Onjuiste FTP link"},
            {"FtpBadConfig", "Verkeerde FTP instellingen"},
            {"FtpBadFolder", "FTP folder mist: "},
            {"FtpNoConnection", "Geen verbinding!"},
            {"RepairAsk", "Toets 'r' om minecraft te repareren ..."},
            {"RepairSuccess", "Minecraft wordt opnieuw geinstalleerd"},
            {"RepairFail", "Minecraft kan niet worden verwijderd, controleer of het spel niet nog open staat!"},
            {"ConfigError", "Config error: "},
            {"ConfigMissing", "Config mist"},
            {"JavaOK", "Java ok"},
            {"JavaNotFound", "Java versie niet gevonden: "},
            {"JavaDownload", "Java downloaden ..."},
            {"JavaInstall", "Java installeren ..."},
            {"JavaError", "Java download error: "},
            {"MinecraftOK", "Minecraft ok"},
            {"MinecraftDownload", "Minecraft downloaden ..."},
            {"MinecraftError", "Minecraft error: "},
            {"ProfileError", "Profiel gerepareerd"},
            {"FabricOK", "Fabric ok"},
            {"FabricDownload", "Fabric downloaden ..."},
            {"ForgeOK", "Forge ok"},
            {"ForgeDownload", "Forge downloaden ..."},
            {"ForgeInstall", "Forge installeren ..."},
            {"ForgeError", "Forge error: "},
            {"SyncUpNotAllowed", "Uploaden is uitgeschakeld"},
            {"SyncUp", "Uploaden"},
            {"SyncDown", "Controleren"},
            {"Syncing", "Syncen"},
            {"SyncError", "Sync error: "},
            {"ServerAdded", "Server toegevoegd: "},
            {"ServerError", "Server error: "},
            {"DllError", "Kan bestand niet laden: "},
            {"PressKey", "[druk op een knop om verder te gaan]"}
        };

        // english
        static Dictionary<string, string> EN = new Dictionary<string, string>()
        {
            {"AutoUpdate", "Updating ..."},
            {"AutoUpdateDone", "Please start again!"},
            {"AutoUpdateError", "Update error: "},
            {"AutoUpdateDotnet", "Please install .NET 3.5 sp1"},
            {"FtpInputLink", "Please input the FTP server details"},
            {"FtpBadCommandLine", "Bad FTP link"},
            {"FtpBadConfig", "Bad connection settings"},
            {"FtpBadFolder", "FTP folder missing: "},
            {"FtpNoConnection", "No connection!"},
            {"RepairAsk", "Press 'r' to repair minecraft ..."},
            {"RepairSuccess", "Minecraft will be reinstalled"},
            {"RepairFail", "Failed to remove minecraft, check if its not still running!"},
            {"ConfigError", "Config error: "},
            {"ConfigMissing", "Config missing"},
            {"JavaOK", "Java ok"},
            {"JavaNotFound", "Java version not found: "},
            {"JavaDownload", "Downloading Java ..."},
            {"JavaInstall", "Installing Java ..."},
            {"JavaError", "Java download error: "},
            {"MinecraftOK", "Minecraft ok"},
            {"MinecraftDownload", "Downloading Minecraft ..."},
            {"MinecraftError", "Minecraft error: "},
            {"ProfileError", "Profile defaulted"},
            {"FabricOK", "Fabric ok"},
            {"FabricDownload", "Downloading Fabric ..."},
            {"ForgeOK", "Forge ok"},
            {"ForgeDownload", "Downloading Forge ..."},
            {"ForgeInstall", "Installing Forge ..."},
            {"ForgeError", "Forge error: "},
            {"SyncUpNotAllowed", "Uploading is disabled"},
            {"SyncUp", "Uploading"},
            {"SyncDown", "Checking"},
            {"Syncing", "Syncing"},
            {"SyncError", "Error syncing: "},
            {"ServerAdded", "Added server: "},
            {"ServerError", "Server error: "},
            {"DllError", "Unable to load library: "},
            {"PressKey", "[press any key to continue]"}
        };
    }
}
