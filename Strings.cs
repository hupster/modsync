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
            {"ConfigError", "Config error: "},
            {"ConfigMissing", "Config mist"},
            {"JavaOK", "Java ok"},
            {"JavaNotFound", "Java versie niet gevonden: "},
            {"JavaDownload", "Java downloaden ..."},
            {"JavaInstall", "Druk op een toets om Java te installeren, en start opnieuw!"},
            {"JavaError", "Java download error: "},
            {"MinecraftOK", "Minecraft ok"},
            {"MinecraftDownload", "Minecraft downloaden ..."},
            {"MinecraftError", "Minecraft error: "},
            {"MinecraftStart", "Druk op een toets, laat Minecraft starten, en start opnieuw!"},
            {"Profile", "Profiel: "},
            {"ProfileAdded", " toegevoegd"},
            {"ProfileSelected", " geselecteerd"},
            {"ProfileGameVersion", "Minecraft versie "},
            {"ProfileJavaVersion", "Java versie "},
            {"ProfileError", "Kan profiel niet bijwerken, selecteer spel versie "},
            {"ForgeOK", "Forge ok"},
            {"ForgeDownload", "Forge downloaden ..."},
            {"ForgeInstall", "Druk op een toets om Forge te installeren (klik OK), en start opnieuw!"},
            {"ForgeError", "Forge error: "},
            {"ModsPushNotAllowed", "Mods uploaden is uitgeschakeld"},
            {"ModsPushing", "Mods uploaden ..."},
            {"ModsPulling", "Mods controleren ..."},
            {"ModsSyncing", "Mods syncen ..."},
            {"ModsOK", "Mods ok"},
            {"ModsError", "Mods error: "},
            {"ServerAdded", "Server toegevoegd: "},
            {"ServerError", "Server error: "},
            {"DllError", "Kan bestand niet laden: "}
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
            {"ConfigError", "Config error: "},
            {"ConfigMissing", "Config missing"},
            {"JavaOK", "Java ok"},
            {"JavaNotFound", "Java version not found: "},
            {"JavaDownload", "Downloading Java ..."},
            {"JavaInstall", "Hit any key to install Java, then restart!"},
            {"JavaError", "Java download error: "},
            {"MinecraftOK", "Minecraft ok"},
            {"MinecraftDownload", "Downloading Minecraft ..."},
            {"MinecraftError", "Minecraft error: "},
            {"MinecraftStart", "Hit any key, let Minecraft load once, then restart!"},
            {"Profile", "Profile: "},
            {"ProfileAdded", " added"},
            {"ProfileSelected", " selected"},
            {"ProfileGameVersion", "Minecraft version "},
            {"ProfileJavaVersion", "Java version "},
            {"ProfileError", "Unable to update profile, please select game version "},
            {"ForgeOK", "Forge ok"},
            {"ForgeDownload", "Downloading Forge ..."},
            {"ForgeInstall", "Hit any key to install Forge (click OK), then restart!"},
            {"ForgeError", "Forge error: "},
            {"ModsPushNotAllowed", "Uploading mods is disabled"},
            {"ModsPushing", "Uploading mods ..."},
            {"ModsPulling", "Checking mods ..."},
            {"ModsSyncing", "Syncing Mods ..."},
            {"ModsOK", "Mods ok"},
            {"ModsError", "Error syncing Mods: "},
            {"ServerAdded", "Added server: "},
            {"ServerError", "Server error: "},
            {"DllError", "Unable to load library: "}
        };
    }
}
