using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
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
            RegistryKey subKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\JavaSoft\\Java Runtime Environment\\" + Config.settings.JavaVersion);
            if (subKey != null)
            {
                Locations.Java = subKey.GetValue("JavaHome").ToString() + "\\bin\\javaw.exe";
                Console.WriteLine(Strings.Get("JavaOK"));
                return;
            }
            Console.WriteLine(Strings.Get("JavaNotFound") + Config.settings.JavaVersion);

            string LocalFile = Locations.LocalFolderName_Desktop + "\\" + Config.settings.JavaDownloadFile;
            string RemoteFile = Config.ftpsettings.FtpServerFolder + "/" + Config.settings.JavaDownloadFile;
            try
            {
                // download and install java
                Console.WriteLine(Strings.Get("JavaDownload"));
                ftpcon.DownloadFile(LocalFile, RemoteFile);
                Console.WriteLine(Strings.Get("JavaInstall"));
                Console.ReadKey();
                var process = Process.Start(LocalFile);
                Program.Exit(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(Strings.Get("JavaError") + ex.Message);
                Console.ReadKey();
            }
        }
    }
}
