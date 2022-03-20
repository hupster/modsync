using System;
using System.IO;
using EnterpriseDT.Net.Ftp;

namespace modsync
{
    static class Vanilla
    {
        public static void Check(ref FTPConnection ftpcon)
        {
            // create default profile if missing
            Profiles.Update("(Default)", Config.settings.MinecraftVersion, "");
        }
    }
}
