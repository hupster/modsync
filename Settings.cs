using System;
using System.Linq;
using System.Text;

namespace modsync
{
    /* FTP link
     * 
     * These are overwritten from the file modsyncftp.xml that can be put in the Minecraft folder.
     * The file is not required if all settings are set here, or supplied as command line argument
     * in the format:
     * ftp://user:pass@server:port/folder
     */
    public class FtpSettings
    {
        public string FtpUserName = "";
        public string FtpPassword = "";
        public string FtpServerAddress = "";
        public string FtpServerPort = "";
        public string FtpServerFolder = "";
    }

    /* Settings
     * 
     * These are overwritten from the file modsync.xml that can be put in the FtpServerFolder.
     * The file is not required if all settings are set here.
     * Leaving any of the settings empty disables the corresponding check.
     */
    public class Settings
    {
        public string ToolVersion = "";
        public string ToolDownloadFile = "";
        public string JavaVersion = "";
        public string JavaDownloadFile = "";
        public string JavaArguments = "";
        public string MinecraftVersion = "";
        public string MinecraftDownloadFile = "";
        public string ForgeVersion = "";
        public string ForgeDownloadFile = "";
        public string SyncFolders = "";
        public string SyncAllowUpload = "";
        public string ServerName = "";
        public string ServerAddress = "";
    }
}
