using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Deployment.Application;
using System.IO;
using System.Diagnostics;
using System.Security.Policy;
using System.Reflection;
using EnterpriseDT.Net.Ftp;

namespace modsync
{
    static class Update
    {
        // updates deployed application using clickonce
        public static bool ClickOnceUpdate()
        {
            // return if not clickonce deployed
            if (!ApplicationDeployment.IsNetworkDeployed)
            {
                return false;
            }
            
            try
            {
                ApplicationDeployment updateCheck = ApplicationDeployment.CurrentDeployment;
                UpdateCheckInfo info = updateCheck.CheckForDetailedUpdate();

                if (info.UpdateAvailable)
                {
                    Console.WriteLine(Strings.Get("AutoUpdate"));
                    updateCheck.Update();
                    Console.WriteLine(Strings.Get("AutoUpdateDone"));
                    Console.ReadKey();
                    return true;
                }
            }
            catch (Exception xcep)
            {
                if (xcep.Message.Contains("WindowsBase Version"))
                {
                    Console.WriteLine(Strings.Get("AutoUpdateDotnet"));
                    Console.ReadKey();
                    Process.Start("http://windowsupdate.microsoft.com");
                }
                else
                {
                    Console.WriteLine(Strings.Get("AutoUpdateError") + " " + xcep.Message);
                    Console.ReadKey();
                }
            }
            return false;
        }

        // updates single executable
        public static void ExecutableUpdate(ref FTPConnection ftpcon)
        {
            // return if clickonce deployed
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                return;
            }

            // return if disabled
            if (Config.settings.ToolVersion == "")
            {
                return;
            }

            // get location and version
            Assembly file = Assembly.GetExecutingAssembly();
            string file_location = file.Location;
            string file_version = FileVersionInfo.GetVersionInfo(file_location).FileVersion;

            int curversion, newversion;
            if (int.TryParse(file_version.Replace(".", ""), out curversion) && int.TryParse(Config.settings.ToolVersion.Replace(".", ""), out newversion))
            {
                // update if newer available
                if (curversion < newversion)
                {
                    string LocalFile = Locations.LocalFolderName_Minecraft + "\\" + Config.settings.ToolDownloadFile;
                    string RemoteFile = Config.ftpsettings.FtpServerFolder + "/" + Config.settings.ToolDownloadFile;
                    try
                    {
                        // download new file
                        Console.WriteLine(Strings.Get("AutoUpdate"));
                        ftpcon.DownloadFile(LocalFile, RemoteFile);

                        // start a process with a delayed file copy
                        Process process = new Process();
                        ProcessStartInfo startInfo = new ProcessStartInfo();
                        startInfo.WindowStyle = ProcessWindowStyle.Normal;
                        startInfo.FileName = "cmd.exe";
                        startInfo.Arguments = "/C ping 127.0.0.1 -n 1 -w 5000 > nul & copy /Y " + LocalFile + " " + file_location + " & " + file_location;
                        process.StartInfo = startInfo;
                        process.Start();
                        Program.Exit(false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(Strings.Get("AutoUpdateError") + " " + ex.Message);
                        Console.ReadKey();
                    }
                }
            }
        }
    }
}
