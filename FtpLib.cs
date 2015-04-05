using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using EnterpriseDT.Net.Ftp;

namespace modsync
{
    /// <summary>
    /// Wrapper class around the EnterpriseDT Ftp library
    /// </summary>
    public class FtpLib
    {
        private FTPConnection FtpServer;
        private long BytesTotal = 0;
        private long BytesDone = 0;
        private long BytesLast = 0;
        private string LastFile = "";

        public FtpLib(ref FTPConnection con)
        {
            this.FtpServer = con;
            this.FtpServer.ReplyReceived += HandleMessages;
            this.FtpServer.CommandSent += HandleMessages;
            this.FtpServer.Downloaded += new FTPFileTransferEventHandler(FtpServer_Downloaded);
            this.FtpServer.Uploaded += new FTPFileTransferEventHandler(FtpServer_Uploaded);
            this.FtpServer.BytesTransferred += HandleProgress;
            //this.FtpServer.TransferNotifyInterval = 1024;
        }

        // Handler for status indicator
        private void ShowStatus(int lvl, string msg)
        {
            if (lvl <= 1)
            {
                Console.WriteLine(msg);
            }
        }

        // Progress event handler
        private void ShowProgress(int progress)
        {
            Console.Write("\r{0}%   ", progress);
        }

        public void LogIn()
        {
            ShowStatus(2, "Connecting to " + this.FtpServer.ServerAddress);
            this.FtpServer.Connect();
        }

        public void DisConnect()
        {
            if (IsConnected())
            {
                try
                {
                    this.FtpServer.Close();
                }
                catch (Exception)
                {
                }
            }
        }

        public bool IsConnected()
        {
            return this.FtpServer.IsConnected;
        }

        public string CurrentWorkingFolder()
        {
            return this.FtpServer.ServerDirectory;
        }

        /// <summary>
        /// Compare local directory with remote directory
        /// </summary>
        /// <param name="localFolder">folder @ client</param>
        /// <param name="remoteFolder">folder @ server</param>
        /// <param name="recursive">Will we go into directories?</param>
        /// <returns>List of modifications to make</returns>
        public List<FTPSyncModification> CompareDirectory(string localFolder, string remoteFolder, FTPSyncSettings settings)
        {
            BytesTotal = 0;
            BytesDone = 0;
            BytesLast = 0;
            LastFile = "";

            // make list of local files
            List<string> locals = new List<string>();
            GetLocalFiles(ref locals, localFolder, settings.Recurse);

            // check remote files
            List<FTPSyncModification> mods = new List<FTPSyncModification>();
            CheckRemoteFiles(ref locals, ref mods, localFolder, remoteFolder, settings);

            if (settings.WhenMissingRemote != FTPAction.Noop)
            {
                // local files still in list have been added
                FileInfo fi;
                foreach (string LocalFile in locals)
                {
                    fi = new FileInfo(LocalFile);
                    FTPSyncModification m = new FTPSyncModification();
                    m.Exists = false;
                    m.Upload = true;
                    m.Action = settings.WhenMissingRemote;
                    m.LocalFile = LocalFile;
                    m.ModifiedTime = fi.LastWriteTime;
                    m.RemoteFile = LocalFile.Replace(localFolder, remoteFolder).Replace('\\', '/');
                    BytesTotal += fi.Length;
                    mods.Add(m);
                    ShowMod(m);
                }
            }
            return mods;
        }

        private void GetLocalFiles(ref List<string> files, string localFolder, bool recursive)
        {
            // create local dir if not present
            CreateDirectoryLocal(new DirectoryInfo(localFolder));

            // add files in current dir
            foreach (string f in Directory.GetFiles(localFolder))
            {
                files.Add(Path.Combine(localFolder, f));
            }

            // recurse into directories
            if (recursive)
            {
                foreach (string d in Directory.GetDirectories(localFolder))
                {
                    if (d != "." && d != "..")
                    {
                        GetLocalFiles(ref files, d, recursive);
                    }
                }
            }
        }
        /// <summary>
        /// Check all remote files for: 
        /// </summary>
        /// <param name="locals"></param>
        /// <param name="mods">List of modifications. Items are added when syncing is required</param>
        /// <param name="localFolder">folder @ client</param>
        /// <param name="remoteFolder">folder @ server</param>
        /// <param name="recursive">if true, directories are also checked by recalling this function </param>
        private void CheckRemoteFiles(ref List<string> locals, ref List<FTPSyncModification> mods, string localFolder, string remoteFolder, FTPSyncSettings settings)
        {
            this.FtpServer.ChangeWorkingDirectory(remoteFolder);

            FTPFile[] FtpServerFileInfo = this.FtpServer.GetFileInfos();

            string LocalTargetFolder = null;
            string FtpTargetFolder = null;
            bool AddModification = false;
            FileInfo fi = default(FileInfo);

            foreach (FTPFile CurrentFileOrDirectory in FtpServerFileInfo)
            {
                if (settings.Recurse)
                {
                    if (CurrentFileOrDirectory.Dir && CurrentFileOrDirectory.Name != "." && CurrentFileOrDirectory.Name != "..")
                    {
                        LocalTargetFolder = Path.Combine(localFolder, CurrentFileOrDirectory.Name);
                        FtpTargetFolder = string.Format("{0}/{1}", remoteFolder, CurrentFileOrDirectory.Name);

                        CheckRemoteFiles(ref locals, ref mods, LocalTargetFolder, FtpTargetFolder, settings);

                        // set the ftp working folder back to the correct value
                        this.FtpServer.ChangeWorkingDirectory(remoteFolder);
                    }
                }

                if (!CurrentFileOrDirectory.Dir)
                {
                    FTPSyncModification m = new FTPSyncModification();
                    AddModification = false;

                    m.LocalFile = Path.Combine(localFolder, CurrentFileOrDirectory.Name);
                    m.RemoteFile = string.Format("{0}/{1}", remoteFolder, CurrentFileOrDirectory.Name);

                    // check file existence
                    if (!File.Exists(m.LocalFile))
                    {
                        if (settings.WhenMissingLocal != FTPAction.Noop)
                        {
                            AddModification = true;
                            m.Exists = false;
                            m.ModifiedTime = CurrentFileOrDirectory.LastModified;
                            m.Upload = false;
                            m.Action = settings.WhenMissingLocal;
                            BytesTotal += CurrentFileOrDirectory.Size;
                        }
                    }
                    else
                    {
                        // read file info
                        fi = new FileInfo(m.LocalFile);

                        // check file size
                        //if (CurrentFileOrDirectory.Size != fi.Length)
                        //{
                        //    AddModification = true;
                        //    m.Type = "Updated";
                        //    m.Upload = (CurrentFileOrDirectory.Size < fi.Length);
                        //}

                        // check modification time
                        DateTime LocalStamp = TimeZone.CurrentTimeZone.ToLocalTime(fi.LastWriteTime);
                        if (CurrentFileOrDirectory.LastModified != LocalStamp)
                        {
                            m.Exists = true;
                            if (CurrentFileOrDirectory.LastModified < LocalStamp)
                            {
                                if (settings.WhenNewerLocal == FTPAction.Copy)
                                {
                                    AddModification = true;
                                    m.ModifiedTime = LocalStamp;
                                    m.Upload = true;
                                    m.Action = settings.WhenNewerLocal;
                                    BytesTotal += fi.Length;
                                }
                            }
                            else
                            {
                                if (settings.WhenNewerRemote == FTPAction.Copy)
                                {
                                    AddModification = true;
                                    m.ModifiedTime = CurrentFileOrDirectory.LastModified;
                                    m.Upload = false;
                                    m.Action = settings.WhenNewerRemote;
                                    BytesTotal += CurrentFileOrDirectory.Size;
                                }
                            }
                        }

                        // remove from local list
                        locals.Remove(m.LocalFile);
                    }

                    if (AddModification)
                    {
                        mods.Add(m);
                        ShowMod(m);
                    }
                }
            }
        }

        private void ShowMod(FTPSyncModification mod)
        {
            switch (mod.Action)
            {
                case FTPAction.Copy:
                    if (mod.Upload)
                    {
                        ShowStatus(1, string.Format("--> {0} ({1} {2})", mod.LocalFile, (mod.Exists ? "Updated" : "Added"), mod.ModifiedTime.ToShortDateString()));
                    }
                    else
                    {
                        ShowStatus(1, string.Format("<-- {0} ({1} {2})", mod.RemoteFile, (mod.Exists ? "Updated" : "Added"), mod.ModifiedTime.ToShortDateString()));
                    }
                    break;
                case FTPAction.Delete:
                    if (mod.Upload)
                    {
                        ShowStatus(1, string.Format("del local {0}", mod.LocalFile));
                    }
                    else
                    {
                        ShowStatus(1, string.Format("del remote {0}", mod.RemoteFile));
                    }
                    break;
            }
        }

        /// <summary>
        /// Synchronize local files with files on the server
        /// </summary>
        /// <param name="mods">List of modifications to make</param>
        public void SynchronizeFiles(List<FTPSyncModification> mods, bool prompt)
        {
            foreach (FTPSyncModification mod in mods)
            {
                // ask user to delete or overwrite existing file
                if (prompt && ((mod.Action == FTPAction.Delete) || mod.Exists))
                {
                    if (!ConfirmModification(mod))
                    {
                        continue;
                    }
                }

                switch (mod.Action)
                {
                    case FTPAction.Copy:
                        if (mod.Upload)
                        {
                            UploadModification(mod);
                        }
                        else
                        {
                            DownloadModification(mod);
                        }
                        break;
                    case FTPAction.Delete:
                        if (mod.Upload)
                        {
                            DeleteLocalMod(mod);
                        }
                        else
                        {
                            DeleteRemoteMod(mod);
                        }
                        break;
                }
            }
        }

        private bool ConfirmModification(FTPSyncModification mod)
        {
            while (true)
            {
                switch (mod.Action)
                {
                    case FTPAction.Copy:
                        if (mod.Upload)
                        {
                            Console.WriteLine("Overwrite remote file: " + mod.LocalFile + "? Y/N");
                        }
                        else
                        {
                            Console.WriteLine("Overwrite local file: " + mod.RemoteFile + "? Y/N");
                        }
                        break;
                    case FTPAction.Delete:
                        if (mod.Upload)
                        {
                            Console.WriteLine("Delete local file: " + mod.LocalFile + "? Y/N");
                        }
                        else
                        {
                            Console.WriteLine("Delete remote file: " + mod.RemoteFile + "? Y/N");
                        }
                        break;
                }

                string overwrite = Console.ReadLine();
                if (overwrite.ToUpper().Equals("Y"))
                {
                    return true;
                }
                else if (overwrite.ToUpper().Equals("N"))
                {
                    return false;
                }
            }
        }

        private void UploadModification(FTPSyncModification mod)
        {
            if (mod.Exists)
            {
                this.FtpServer.DeleteFile(mod.RemoteFile);
            }

            // check directory
            string dir = mod.RemoteFile.Substring(0, mod.RemoteFile.LastIndexOf('/'));
            CreateDirectoryRemote(dir);

            // upload file
            this.FtpServer.UploadFile(mod.LocalFile, mod.RemoteFile);

            // update file properties
            try
            {
                this.FtpServer.SetLastWriteTime(mod.RemoteFile, mod.ModifiedTime);
            }
            catch (Exception)
            {
            }
        }

        private void DownloadModification(FTPSyncModification mod)
        {
            FileInfo fi;
            // delete old file
            if (mod.Exists)
            {
                File.Delete(mod.LocalFile);
            }

            // check directory
            string dir = Path.GetDirectoryName(mod.LocalFile);
            CreateDirectoryLocal(new DirectoryInfo(dir));

            // download file
            this.FtpServer.DownloadFile(mod.LocalFile, mod.RemoteFile);

            // update file properties
            fi = new FileInfo(mod.LocalFile);
            fi.CreationTime = mod.ModifiedTime;
            fi.LastAccessTime = mod.ModifiedTime;
            fi.LastWriteTime = mod.ModifiedTime;
        }

        private void DeleteLocalMod(FTPSyncModification mod)
        {
            File.Delete(mod.LocalFile);
        }

        private void DeleteRemoteMod(FTPSyncModification mod)
        {
            this.FtpServer.DeleteFile(mod.RemoteFile);
        }

        public void CreateDirectoryLocal(DirectoryInfo dirInfo)
        {
            if (!dirInfo.Exists)
            {
                if (dirInfo.Parent != null)
                {
                    CreateDirectoryLocal(dirInfo.Parent);
                }
                ShowStatus(2, "FTP: Creating dir " + dirInfo.FullName);
                dirInfo.Create();
            }
        }

        public void CreateDirectoryRemote(string dirName)
        {
            if (!this.FtpServer.DirectoryExists(dirName))
            {
                string parent = "";
                int index = dirName.LastIndexOf('/');
                if (index > 0) parent = dirName.Substring(0, index);
                if (parent != "")
                {
                    CreateDirectoryRemote(parent);
                }
                ShowStatus(2, "FTP: Creating dir " + dirName);
                this.FtpServer.CreateDirectory(dirName);
            }
        }

        // status feedback
        private void HandleMessages(object sender, FTPMessageEventArgs e)
        {
            ShowStatus(3, "FTP: " + e.Message);
        }

        // progress feedback
        private void HandleProgress(object sender, BytesTransferredEventArgs e)
        {
            if (BytesTotal == 0)
            {
                return;
            }
            try
            {
                // bug in edtFTPnet: path incorrect, so we need to check filename as well as size
                if ((e.RemoteFile != LastFile) || (e.ByteCount < BytesLast))
                {
                    BytesDone += BytesLast;
                }
                LastFile = e.RemoteFile;
                BytesLast = e.ByteCount;
                int perc = (int)((long)100 * (BytesDone + BytesLast) / BytesTotal);
                if (perc > 100) perc = 100;
                ShowProgress(perc);
            }
            catch
            {
            }
        }

        private void FtpServer_Uploaded(object sender, FTPFileTransferEventArgs e)
        {
            ShowStatus(2, "FTP: Uploaded " + e.RemoteFile);
        }

        private void FtpServer_Downloaded(object sender, FTPFileTransferEventArgs e)
        {
            ShowStatus(2, "FTP: Downloaded " + e.RemoteFile);
        }
    }

    /// <summary>
    /// Modification record
    /// </summary>
    public class FTPSyncModification
    {
        public bool Upload;                 /*if true: upload modification, otherwise download modification*/
        public bool Exists;
        public string LocalFile;
        public string RemoteFile;
        public DateTime ModifiedTime;
        public FTPAction Action;
    }

    public enum FTPAction
    {
        Noop,
        Copy,
        Delete
    }

    /// <summary>
    /// Sync settings
    /// </summary>
    public class FTPSyncSettings
    {
        // go into directories
        public bool Recurse;
        // what to do when missing remote
        public FTPAction WhenMissingRemote;
        // what to do when missing local
        public FTPAction WhenMissingLocal;
        // what to do when newer remote
        public FTPAction WhenNewerRemote;
        // what to do when newer local
        public FTPAction WhenNewerLocal;
    }
}
