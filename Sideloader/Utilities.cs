using AndroidSideloader.Utilities;
using System;
using System.IO;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace AndroidSideloader
{
    internal class SideloaderUtilities
    {
        private static readonly SettingsManager settings = SettingsManager.Instance;
        public static bool CheckFolderIsObb(string path)
        {
            string[] files = Directory.GetFiles(path);

            foreach (string file in files)
            {
                if (file.EndsWith(".obb") || (Path.GetDirectoryName(file).Contains(".") && !Path.GetDirectoryName(file).Contains("_data")))
                {
                    return true;
                }
            }

            return false;
        }

        private static string uuid = null;
        public static string UUID()
        {
            uuid = settings.UUID;
            if (string.IsNullOrEmpty(uuid) != true)
            {
                return uuid;
            }

            var bytes = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytes);
            }

            uuid = BitConverter.ToString(bytes).Replace("-", "");

            settings.UUID = uuid;
            settings.Save();

            return uuid;
        }

        public static string GetAppDataPath()
        {
#if WINDOWS
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "rookie");
#elif LINUX
            string xdgDataHome = Environment.GetEnvironmentVariable("XDG_DATA_HOME");
            if (string.IsNullOrEmpty(xdgDataHome))
            {
                xdgDataHome = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "rookie");
            }
            return Path.Combine(xdgDataHome, "rookie");
#else
            throw new PlatformNotSupportedException("Unsupported OS");
#endif
        }
    }
}
