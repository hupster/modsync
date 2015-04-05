using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Deployment.Application;
using System.IO;
using System.Diagnostics;
using System.Security.Policy;

namespace modsync
{
    static class Update
    {
        public static bool AutoUpdate()
        {
            if (!ApplicationDeployment.IsNetworkDeployed)
            {
                return true;
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
                    return false;
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
            return true;
        }

        public static void ReplaceShortCut()
        {
            // replace shortcut to minecraft with shortcut to launcher
            string RemoveLink = Locations.LocalFolderName_Desktop + "\\" + Config.settings.DesktopShortcutToRemove;
            string CreateLink = Locations.LocalFolderName_Desktop + "\\" + Config.settings.DesktopShortcutToCreate;
            if ((Config.settings.DesktopShortcutToCreate != "") && !File.Exists(CreateLink))
            {
                if (WriteAppRefFile(CreateLink))
                {
                    if ((Config.settings.DesktopShortcutToRemove != "") && File.Exists(RemoveLink))
                    {
                        File.Delete(RemoveLink);
                    }
                }
            }
        }

        // write application reference
        static bool WriteAppRefFile(string FileLocation)
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                try
                {
                    // create the file
                    StreamWriter sw = new StreamWriter(FileLocation, false, Encoding.Unicode);
                    ApplicationSecurityInfo asi = new ApplicationSecurityInfo(AppDomain.CurrentDomain.ActivationContext);

                    // write additional parameters
                    sw.Write(ApplicationDeployment.CurrentDeployment.UpdateLocation.ToString() + "#modsync.application, ");
                    sw.Write("Culture=neutral, PublicKeyToken=");
                    byte[] pk = asi.ApplicationId.PublicKeyToken;
                    for (int i = 0; i < pk.GetLength(0); i++)
                    {
                        sw.Write("{0:x2}", pk[i]);
                    }
                    sw.Write(", processorArchitecture=" + asi.ApplicationId.ProcessorArchitecture);
                    sw.Close();
                    return true;
                }
                catch (Exception)
                {
                }
            }
            return false;
        }
    }
}
