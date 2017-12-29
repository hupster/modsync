using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using EnterpriseDT.Net.Ftp;

namespace modsync
{
    static class Java
    {
        public static void Check(ref FTPConnection ftpcon)
        {
            // return if disabled
            if ((Config.settings.JavaVersion == "") || (Config.settings.JavaDownloadFile == ""))
            {
                return;
            }

            // check registery key
            if (CheckRegistery())
            {
                Console.WriteLine(Strings.Get("JavaOK"));
                return;
            }
            Console.WriteLine(Strings.Get("JavaNotFound") + Config.settings.JavaVersion);

            string LocalFile = Locations.LocalFolderName_TempDir + "\\" + Config.settings.JavaDownloadFile;
            string RemoteFile = Config.ftpsettings.FtpServerFolder + "/" + Config.settings.JavaDownloadFile;
            try
            {
                // download and install java
                Console.WriteLine(Strings.Get("JavaDownload"));
                File.Delete(LocalFile);
                ftpcon.DownloadFile(LocalFile, RemoteFile);
                Console.WriteLine(Strings.Get("JavaInstall"));
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = LocalFile;
                psi.WorkingDirectory = Path.GetDirectoryName(LocalFile);
                psi.Verb = "runas";
                psi.Arguments = "/s";
                psi.UseShellExecute = true;
                var process = Process.Start(psi);
                process.WaitForExit();
                File.Delete(LocalFile);

                // check registery
                CheckRegistery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(Strings.Get("JavaError") + ex.Message);
                Console.WriteLine(Strings.Get("PressKey"));
                Console.ReadKey();
            }
        }

        static bool CheckRegistery()
        {
            RegistryKey subKey;
            if (Config.settings.JavaArchitecture == "32")
            {
                subKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node\\JavaSoft\\Java Runtime Environment\\" + Config.settings.JavaVersion);
            }
            else
            {
                subKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\JavaSoft\\Java Runtime Environment\\" + Config.settings.JavaVersion);
            }

            if (subKey != null)
            {
                Locations.Javaw = subKey.GetValue("JavaHome").ToString() + "\\bin\\javaw.exe";
                Locations.Java = subKey.GetValue("JavaHome").ToString() + "\\bin\\java.exe";
                return true;
            }
            return false;
        }
    }
}
