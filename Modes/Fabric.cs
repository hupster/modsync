using System;
using System.IO;
using System.Diagnostics;
using EnterpriseDT.Net.Ftp;

namespace modsync
{
    static class Fabric
    {
        public static void Check(ref FTPConnection ftpcon)
        {
            // return if disabled
            if ((Config.settings.FabricVersion == "") || (Config.settings.FabricDownloadFile == ""))
            {
                Console.WriteLine(Strings.Get("FabricError") + Strings.Get("ConfigMissing"));
                return;
            }

            // check fabric json file
            string jsonfile = Locations.LocalFolderName_Versions + "\\" + Config.settings.FabricVersion + "\\" + Config.settings.FabricVersion + ".json";
            if (File.Exists(jsonfile))
            {
                Console.WriteLine(Strings.Get("FabricOK"));
            }
            else
            {
                Install(ref ftpcon, jsonfile);
            }

            // create fabric profile if missing
            Profiles.Update("Fabric", Config.settings.FabricVersion, Config.settings.JavaArguments);
        }

        static void Install(ref FTPConnection ftpcon, string jsonfile)
        {
            // create default profile if missing
            if (!File.Exists(Locations.LauncherProfiles))
            {
                Profiles.Update("(Default)", Config.settings.MinecraftVersion, "");
            }

            // create directory if missing
            if (!Directory.Exists(Locations.LocalFolderName_Versions + "\\" + Config.settings.FabricVersion))
            {
                Directory.CreateDirectory(Locations.LocalFolderName_Versions + "\\" + Config.settings.FabricVersion);
            }

            // download fabric json file
            string RemoteFile = Config.ftpsettings.FtpServerFolder + "/" + Config.settings.FabricDownloadFile;
            try
            {
                Console.WriteLine(Strings.Get("FabricDownload"));
                ftpcon.DownloadFile(jsonfile, RemoteFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine(Strings.Get("FabricError") + ex.Message);
                Console.WriteLine(Strings.Get("PressKey"));
                Console.ReadKey();
                Program.Exit(true);
            }
        }
    }
}
