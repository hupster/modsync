using System;
using System.IO;
using System.Diagnostics;
using EnterpriseDT.Net.Ftp;

namespace modsync
{
    static class Forge
    {
        public static void Check(ref FTPConnection ftpcon)
        {
            // return if disabled
            if ((Config.settings.ForgeVersion == "") || (Config.settings.ForgeDownloadFile == ""))
            {
                Console.WriteLine(Strings.Get("ForgeError") + Strings.Get("ConfigMissing"));
                return;
            }

            // check forge json file
            if (File.Exists(Locations.LocalFolderName_Versions + "\\" + Config.settings.ForgeVersion + "\\" + Config.settings.ForgeVersion + ".json"))
            {
                Console.WriteLine(Strings.Get("ForgeOK"));
            }
            else
            {
                Install(ref ftpcon);
            }

            // create forge profile if missing
            Profiles.Update("Forge", Config.settings.ForgeVersion, Config.settings.JavaArguments);
        }

        static void Install(ref FTPConnection ftpcon)
        {
            // create default profile if missing
            if (!File.Exists(Locations.LauncherProfiles))
            {
                Profiles.Update("(Default)", Config.settings.MinecraftVersion, "");
            }

            string LocalFile = Locations.LocalFolderName_TempDir + "\\" + Config.settings.ForgeDownloadFile;
            string RemoteFile = Config.ftpsettings.FtpServerFolder + "/" + Config.settings.ForgeDownloadFile;
            try
            {
                // download and install forge
                Console.WriteLine(Strings.Get("ForgeDownload"));
                File.Delete(LocalFile);
                ftpcon.DownloadFile(LocalFile, RemoteFile);
                Console.WriteLine(Strings.Get("ForgeInstall"));

                // parse forge version (e.g. 14.23.4.2747)
                string version = Config.settings.ForgeVersion;
                string wrapper = "ForgeInstallWrapper";
                try
                {
                    version = version.Substring(version.LastIndexOf('-') + 1);
                    version = version.Replace(".", "");
                    int version_i = int.Parse(version);
                    if (version_i > 142342747)
                    {
                        wrapper = "ForgeInstallWrapper2";
                    }
                }
                catch (Exception)
                {
                }

                if (File.Exists(Locations.Java))
                {
                    // extract wrapper class to invoke client install
                    // java -cp "C:\path\to\ForgeInstallWrapper.jar;forge-x.y.z-installer.jar" ForgeInstallWrapper "forge-x.y.z-installer.jar" "C:\Users\[user]\AppData\Roaming\.minecraft"
                    Resources.ExtractFile(Locations.LocalFolderName_TempDir, wrapper + ".jar");
                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.FileName = Locations.Java;
                    psi.WorkingDirectory = Path.GetDirectoryName(LocalFile);
                    psi.Arguments = "-cp \"" + Locations.LocalFolderName_TempDir + "\\" + wrapper + ".jar;" + Config.settings.ForgeDownloadFile + "\" ";
                    psi.Arguments += wrapper + " ";
                    psi.Arguments += "\"" + Config.settings.ForgeDownloadFile + "\" ";
                    psi.Arguments += "\"" + Locations.LocalFolderName_Minecraft + "\"";
                    psi.UseShellExecute = false;
                    var process = Process.Start(psi);
                    process.WaitForExit();
                    File.Delete(LocalFile);
                    File.Delete(Locations.LocalFolderName_TempDir + "\\" + wrapper + ".jar");
                }
                else
                {
                    Process.Start(LocalFile);
                    // exit without starting game
                    Program.Exit(false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(Strings.Get("ForgeError") + ex.Message);
                Console.WriteLine(Strings.Get("PressKey"));
                Console.ReadKey();
                Program.Exit(true);
            }
        }
    }
}
