using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using EnterpriseDT.Net.Ftp;

namespace modsync
{
    class Minecraft
    {
        public static void Check(ref FTPConnection ftpcon)
        {
            try
            {
                // return if disabled
                if ((Config.settings.MinecraftVersion == "") || (Config.settings.MinecraftDownloadFile == ""))
                {
                    return;
                }

                // ok when launcher is there
                if (File.Exists(Locations.Launcher_Install) || File.Exists(Locations.Launcher_Download))
                {
                    Console.WriteLine(Strings.Get("MinecraftOK"));
                    return;
                }

                // download launcher
                if (!Directory.Exists(Locations.LocalFolderName_Launcher))
                {
                    Directory.CreateDirectory(Locations.LocalFolderName_Launcher);
                }
                Console.WriteLine(Strings.Get("MinecraftDownload"));
                ftpcon.DownloadFile(Locations.Launcher_Download, Config.ftpsettings.FtpServerFolder + "/" + Config.settings.MinecraftDownloadFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine(Strings.Get("MinecraftError") + ex.Message);
                Console.WriteLine(Strings.Get("PressKey"));
                Console.ReadKey();
                Program.Exit(false);
            }
        }
    }
}
