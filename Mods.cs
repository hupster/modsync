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
            if (Config.settings.ModsSyncFolder == "")
            {
                return;
            }
            // update local with server mods
            Console.WriteLine(Strings.Get("ModsPulling"));
            FTPSyncSettings ftpsyncsettings = new FTPSyncSettings();
            ftpsyncsettings.Recurse = false;
            ftpsyncsettings.WhenMissingLocal = FTPAction.Copy;
            ftpsyncsettings.WhenMissingRemote = FTPAction.Delete;
            ftpsyncsettings.WhenNewerLocal = FTPAction.Noop;
            ftpsyncsettings.WhenNewerRemote = FTPAction.Noop;
            Sync(ref ftp, ftpsyncsettings);
        }

        public static void Push(ref FtpLib ftp)
        {
            // return if disabled
            if (Config.settings.ModsSyncFolder == "")
            {
                return;
            }
            if (Config.settings.ModsSyncAllowPush != "true")
            {
                Console.WriteLine(Strings.Get("ModsPushNotAllowed"));
                Console.ReadKey();
                return;
            }

            // update server with local mods
            Console.WriteLine(Strings.Get("ModsPushing"));
            FTPSyncSettings ftpsyncsettings = new FTPSyncSettings();
            ftpsyncsettings.Recurse = false;
            ftpsyncsettings.WhenMissingLocal = FTPAction.Delete;
            ftpsyncsettings.WhenMissingRemote = FTPAction.Copy;
            ftpsyncsettings.WhenNewerLocal = FTPAction.Noop;
            ftpsyncsettings.WhenNewerRemote = FTPAction.Noop;
            Sync(ref ftp, ftpsyncsettings);
        }

        static bool Sync(ref FtpLib ftp, FTPSyncSettings ftpsyncsettings)
        {
            try
            {
                // make list of changes
                List<FTPSyncModification> changes = ftp.CompareDirectory(Locations.LocalFolderName_Mods, Config.ftpsettings.FtpServerFolder + "/" + Config.settings.ModsSyncFolder, ftpsyncsettings);
                if (changes.Count > 0)
                {
                    Console.WriteLine(Strings.Get("ModsSyncing"));
                    ftp.SynchronizeFiles(changes, false);
                }
                Console.WriteLine(Strings.Get("ModsOK"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(Strings.Get("ModsError") + ex.Message);
                Console.ReadKey();
                return false;
            }
            return true;
        }
    }
}
