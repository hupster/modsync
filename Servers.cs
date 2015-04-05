using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace modsync
{
    static class Servers
    {
        // checks the servers file
        public static void Check()
        {
            // return if disabled
            if ((Config.settings.ServerName == "") || (Config.settings.ServerAddress == ""))
            {
                return;
            }

            try
            {
                // write empty file if it doesn't exists
                if (!File.Exists(Locations.ServersFile))
                {
                    File.WriteAllBytes(Locations.ServersFile, Default());
                }

                // add entry if server address not in file
                if (!File.ReadAllText(Locations.ServersFile).Contains(Config.settings.ServerAddress))
                {
                    AddEntry();
                    Console.WriteLine(Strings.Get("ServerAdded") + Config.settings.ServerAddress);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(Strings.Get("ServerError") + ex.Message);
                Console.ReadKey();
            }
        }

        // returns a default servers file
        static byte[] Default()
        {
            return new byte[] { 0x0a, 0x00, 0x00, 0x09, 0x00, 0x07, 0x73, 0x65, 0x72, 0x76, 0x65, 0x72, 0x73, 0x0a, 0x00, 0x00, 0x00, 0x00, 0x00 };
        }

        // writes a server list entry
        static void AddEntry()
        {
            FileStream stream = new FileStream(Locations.ServersFile, FileMode.Open, FileAccess.ReadWrite);

            // increment server list count
            stream.Seek(16, SeekOrigin.Begin);
            UInt16 count = 1;
            count += (UInt16)(stream.ReadByte() << 8);
            count += (UInt16)(stream.ReadByte());
            stream.Seek(16, SeekOrigin.Begin);
            stream.WriteByte((byte)((count >> 8) & 0xff));
            stream.WriteByte((byte)(count & 0xff));

            // add entry to the end of the list
            stream.Seek(-1, SeekOrigin.End);
            AddString(stream, "name", Config.settings.ServerName);
            AddString(stream, "ip", Config.settings.ServerAddress);
            stream.WriteByte(0x00);
            stream.WriteByte(0x00);
            stream.Close();
        }

        // writes a string tag
        static void AddString(FileStream stream, string name, string value)
        {
            stream.WriteByte(0x08);
            stream.WriteByte((byte)((name.Length >> 8) & 0xff));
            stream.WriteByte((byte)(name.Length & 0xff));
            stream.Write(Encoding.ASCII.GetBytes(name), 0, name.Length);
            stream.WriteByte((byte)((value.Length >> 8) & 0xff));
            stream.WriteByte((byte)(value.Length & 0xff));
            stream.Write(Encoding.ASCII.GetBytes(value), 0, value.Length);
        }
    }
}
