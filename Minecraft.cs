using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;
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

                // check registry for install location
                CheckRegistery();

                // ok when launcher is there
                if (File.Exists(Locations.Launcher))
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
                ftpcon.DownloadFile(Locations.Launcher, Config.ftpsettings.FtpServerFolder + "/" + Config.settings.MinecraftDownloadFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine(Strings.Get("MinecraftError") + ex.Message);
                Console.WriteLine(Strings.Get("PressKey"));
                Console.ReadKey();
                Program.Exit(false);
            }
        }

        static void CheckRegistery()
        {
            RegistryKey subKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Mojang\\Minecraft\\");
            if (subKey == null)
            {
                subKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node\\Mojang\\Minecraft\\");
            }
            if (subKey != null)
            {
                string launcher = subKey.GetValue("InstallDirNew").ToString() + "MinecraftLauncher.exe";
                if (File.Exists(launcher))
                {
                    Locations.Launcher = launcher;
                    Locations.Launcher_Jar = Path.GetDirectoryName(launcher) + "\\game\\launcher.jar";
                }
            }
        }

        public static void Start()
        {
            // start extracted launcher with desired java version if possible
            if (File.Exists(Locations.Javaw) && File.Exists(Locations.Launcher_Jar))
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = Locations.Javaw;
                psi.WorkingDirectory = Path.GetDirectoryName(Locations.Launcher_Jar);
                psi.Arguments = "-cp \"" + Locations.Launcher_Jar + "\" net.minecraft.launcher.Main";
                psi.UseShellExecute = false;
                Process.Start(psi);
            }
            // start launcher
            else if (File.Exists(Locations.Launcher))
            {
                Process.Start(Locations.Launcher);
            }
        }
    }
}
