using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace modsync
{
    class Mods
    {
        public static void Pull(ref FtpLib ftp)
        {
            // return if disabled
            if (Config.settings.SyncFolders == "")
            {
                return;
            }

            // download changes on server to local
            foreach (string folder in Config.settings.SyncFolders.Split(','))
            {
                Console.WriteLine(Strings.Get("SyncDown") + " " + folder + " ... ");
                FTPSyncSettings ftpsyncsettings = new FTPSyncSettings();
                if (folder == "config")
                {
                    // special case for config folder
                    ftpsyncsettings.Recurse = false;
                    ftpsyncsettings.WhenMissingLocal = FTPAction.Copy;
                    ftpsyncsettings.WhenMissingRemote = FTPAction.Noop;
                    ftpsyncsettings.WhenNewerLocal = FTPAction.Noop;
                    ftpsyncsettings.WhenNewerRemote = FTPAction.Copy;
                    ftpsyncsettings.WhenLargerLocal = FTPAction.Noop;
                    ftpsyncsettings.WhenLargerRemote = FTPAction.Noop;
                }
                else
                {
                    // other folders
                    ftpsyncsettings.Recurse = false;
                    ftpsyncsettings.WhenMissingLocal = FTPAction.Copy;
                    ftpsyncsettings.WhenMissingRemote = FTPAction.Delete;
                    ftpsyncsettings.WhenNewerLocal = FTPAction.Noop;
                    ftpsyncsettings.WhenNewerRemote = FTPAction.Noop;
                    ftpsyncsettings.WhenLargerLocal = FTPAction.Noop;
                    ftpsyncsettings.WhenLargerRemote = FTPAction.Noop;
                }
                Sync(ref ftp, folder, ftpsyncsettings);
            }
        }

        public static void Push(ref FtpLib ftp)
        {
            // return if disabled
            if (Config.settings.SyncFolders == "")
            {
                return;
            }
            if (Config.settings.SyncAllowUpload != "true")
            {
                Console.WriteLine(Strings.Get("SyncUpNotAllowed"));
                Console.ReadKey();
                return;
            }

            // upload changes in local folders to server
            foreach (string folder in Config.settings.SyncFolders.Split(','))
            {
                Console.WriteLine(Strings.Get("SyncUp") + " " + folder + " ... ");
                FTPSyncSettings ftpsyncsettings = new FTPSyncSettings();
                if (folder == "config")
                {
                    // skip push for config folder
                    continue;
                }
                else
                {
                    // other folders
                    ftpsyncsettings.Recurse = false;
                    ftpsyncsettings.WhenMissingLocal = FTPAction.Delete;
                    ftpsyncsettings.WhenMissingRemote = FTPAction.Copy;
                    ftpsyncsettings.WhenNewerLocal = FTPAction.Noop;
                    ftpsyncsettings.WhenNewerRemote = FTPAction.Noop;
                    ftpsyncsettings.WhenLargerLocal = FTPAction.Noop;
                    ftpsyncsettings.WhenLargerRemote = FTPAction.Noop;
                }
                Sync(ref ftp, folder, ftpsyncsettings);
            }
        }

        static bool Sync(ref FtpLib ftp, string folder, FTPSyncSettings ftpsyncsettings)
        {
            try
            {
                // make list of changes
                string localFolder = Locations.LocalFolderName_Minecraft + "\\" + folder;
                string remoteFolder = Config.ftpsettings.FtpServerFolder + "/" + folder;
                List<FTPSyncModification> changes = ftp.CompareDirectory(localFolder, remoteFolder, ftpsyncsettings);
                if (changes.Count > 0)
                {
                    Console.WriteLine(Strings.Get("Syncing") + " " + folder + " ... ");
                    ftp.SynchronizeFiles(changes, false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(Strings.Get("SyncError") + ex.Message);
                Console.WriteLine(Strings.Get("PressKey"));
                Console.ReadKey();
                return false;
            }
            return true;
        }
    }
}
