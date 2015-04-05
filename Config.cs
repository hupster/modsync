using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using EnterpriseDT.Net.Ftp;

namespace modsync
{
    static class Config
    {
        public static FtpSettings ftpsettings = new FtpSettings();
        public static Settings settings = new Settings();
        
        // decodes FTP link in format ftp://user:pass@server:port/folder
        public static void FtpLinkParse(string link)
        {
            try
            {
                // remove ftp://
                link = link.Substring(6);

                // split the string by separator
                int idx;
                List<String> str = new List<String>();
                foreach (char sep in new char[] { ':', '@', ':', '/' })
                {
                    idx = link.IndexOf(sep, 0);
                    str.Add(link.Substring(0, idx));
                    link = link.Substring(idx + 1);
                }
                str.Add(link);

                // all ok, update the settings
                ftpsettings.FtpUserName = str[0];
                ftpsettings.FtpPassword = str[1];
                ftpsettings.FtpServerAddress = str[2];
                ftpsettings.FtpServerPort = str[3];
                ftpsettings.FtpServerFolder = str[4];
            }
            catch
            {
                Console.WriteLine(Strings.Get("FtpBadCommandLine"));
                Console.ReadKey();
                Program.Exit(false);
                return;
            }
        }

        // reads modsyncftp.xml
        public static void FtpLinkRead()
        {
            string FtpFile = Locations.LocalFolderName_Minecraft + "\\" + "modsyncftp.xml";
            if (File.Exists(FtpFile))
            {
                try
                {
                    ftpsettings = (FtpSettings)Read(typeof(FtpSettings), FtpFile);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(Strings.Get("ConfigError") + ex.Message);
                    Console.ReadKey();
                }
            }
        }

        // writes modsyncftp.xml
        public static void FtpLinkInput()
        {
            // have user enter the ftp parameters
            string str;
            Console.WriteLine(Strings.Get("FtpInputLink"));
            foreach (FieldInfo field in ftpsettings.GetType().GetFields())
            {
                do
                {
                    Console.Write(field.Name + ": ");
                    str = Console.ReadLine();
                }
                while (str == "");
                field.SetValue(ftpsettings, str);
            }

            // write file
            string FtpFile = Locations.LocalFolderName_Minecraft + "\\" + "modsyncftp.xml";
            try
            {
                Write(ftpsettings, FtpFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine(Strings.Get("ConfigError") + ex.Message);
                Console.ReadKey();
            }
        }

        // downloads modsync.xml
        public static void FtpUpdate(ref FTPConnection ftpcon)
        {
            string ConfigFile = "modsync.xml";
            string LocalFile = Locations.LocalFolderName_Minecraft + "\\" + ConfigFile;
            string RemoteFile = ftpsettings.FtpServerFolder + "/" + ConfigFile;
            try
            {
                if (ftpcon.Exists(RemoteFile))
                {
                    ftpcon.DownloadFile(LocalFile, RemoteFile);
                    settings = (Settings)Read(typeof(Settings), LocalFile);
                }
                else
                {
                    Console.WriteLine(Strings.Get("ConfigMissing"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(Strings.Get("ConfigError") + ex.Message);
                Console.ReadKey();
            }
        }

        /// Write Settings to XML File
        public static void Write(object settings, string path)
        {
            XmlSerializer x = new XmlSerializer(settings.GetType());
            StreamWriter writer = new StreamWriter(path);
            x.Serialize(writer, settings);
        }

        /// Read Settings from XML File
        public static object Read(Type typ, string path)
        {
            XmlSerializer x = new XmlSerializer(typ);
            StreamReader reader = new StreamReader(path);
            return x.Deserialize(reader);
        }
    }
}
