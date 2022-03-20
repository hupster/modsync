using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace modsync
{
    using ProfileList = Dictionary<string, Dictionary<string, string>>;

    static class Profiles
    {
        // updates launcher_profiles.json to use a specific profile, game version and java location
        public static void Update(string profile, string version, string javaArgs)
        {
            string file = "";
            if (File.Exists(Locations.LauncherProfiles))
            {
                file = File.ReadAllText(Locations.LauncherProfiles, Encoding.ASCII);
            }
            string file_org = file;

            // read profiles
            int prof_idx;
            ProfileList profiles = ReadProfiles(ref file, out prof_idx);

            // add and update entry 
            if (!profiles.ContainsKey(profile))
            {
                profiles.Add(profile, new Dictionary<string, string>());
            }
            profiles[profile]["name"] = profile;
            profiles[profile]["lastVersionId"] = version;
            if (File.Exists(Locations.Javaw))
            {
                profiles[profile]["javaDir"] = Locations.Javaw.Replace("\\", "\\\\");
            }
            if (javaArgs != "")
            {
                profiles[profile]["javaArgs"] = javaArgs;
            }

            // remove profile that can be created by forge install
            profiles.Remove("forge");

            // write profiles
            string prof_str = WriteProfiles(profiles, profile);

            // insert profile section
            file = file.Insert(prof_idx, prof_str);
            file = file.Replace(",\n}", "\n}");

            if (file != file_org)
            {
                if (File.Exists(Locations.LauncherProfiles))
                {
                    File.Copy(Locations.LauncherProfiles, Locations.LauncherProfiles + ".org", true);
                }
                File.WriteAllText(Locations.LauncherProfiles, file, Encoding.ASCII);
            }
        }

        // read profile string into object
        static ProfileList ReadProfiles(ref string file, out int prof_idx)
        {
            ProfileList profiles = new ProfileList();

            try
            {
                string str_prof = "\"profiles\": {";
                prof_idx = file.IndexOf(str_prof);
                int idx, idx_open, idx_close;
                if (prof_idx >= 0)
                {
                    idx = prof_idx + str_prof.Length;

                    while (true)
                    {
                        // look for profile block
                        idx_open = file.IndexOf("{", idx);
                        idx_close = file.IndexOf("}", idx);
                        if ((idx_open == -1) || (idx_open > idx_close))
                        {
                            break;
                        }

                        // found new profile
                        string name = ParseQuotedStr(file.Substring(idx, idx_open - idx));
                        profiles.Add(name, new Dictionary<string, string>());

                        // parse the values
                        string[] pars = file.Substring(idx_open, idx_close - idx_open).Split('\n');
                        foreach (string par in pars)
                        {
                            string[] vals = par.Split(':');
                            if (vals.Length == 2)
                            {
                                profiles[name].Add(ParseQuotedStr(vals[0]), ParseQuotedStr(vals[1]));
                            }
                        }
                        idx = idx_close + 1;
                    }

                    // remove profile section from file
                    idx_open = file.LastIndexOf("\n", prof_idx);
                    idx_close = file.IndexOf("\n", idx_close);
                    file = file.Remove(idx_open + 1, idx_close - idx_open);
                    prof_idx = idx_open + 1;

                    // also remove selectedProfile line
                    idx = file.IndexOf("\"selectedProfile\":");
                    if (idx >= 0)
                    {
                        idx_open = file.LastIndexOf("\n", idx);
                        idx_close = file.IndexOf("\n", idx);
                        file = file.Remove(idx_open + 1, idx_close - idx_open);
                    }
                    return profiles;
                }
            }
            catch (Exception)
            {
                Console.WriteLine(Strings.Get("ProfileError"));
            }

            // bad file, create empty one
            file = "{\n}";
            prof_idx = 2;
            return profiles;
        }

        static string ParseQuotedStr(string str)
        {
            int i = str.IndexOf("\"");
            int j = str.LastIndexOf("\"");
            return str.Substring(i + 1, j - i - 1);
        }

        // write profile object into string
        static string WriteProfiles(ProfileList profiles, string selectedprofile)
        {
            string str_prof = "  \"profiles\": {\n";
            foreach (KeyValuePair<string, Dictionary<string, string>> profile in profiles)
            {
                str_prof += string.Format("    \"{0}\": {{\n", profile.Key);
                foreach (KeyValuePair<string, string> pars in profile.Value)
                {
                    str_prof += string.Format("      \"{0}\": \"{1}\",\n", pars.Key, pars.Value);
                }
                str_prof = str_prof.Substring(0, str_prof.Length - 2);
                str_prof += "\n    },\n";
            }
            str_prof = str_prof.Substring(0, str_prof.Length - 2);
            str_prof += "\n  },\n";
            str_prof += "  \"selectedProfile\": \"" + selectedprofile + "\",\n";
            return str_prof;
        }
    }
}
