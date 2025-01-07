using System.Collections.Generic;
#if LINUX
using System.Runtime.InteropServices;
#endif

namespace AndroidSideloader
{
    internal class Donors
    {
        public static int GameNameIndex = 0;
        public static int PackageNameIndex = 1;
        public static int VersionCodeIndex = 2;
        public static int UpdateOrNew = 3;
        /* Game Name
        * Package Name
        * Version Code
        * Update or New app
        */
        public static List<string> newAppProperties = new List<string>();
        public static List<string> donorGameProperties = new List<string>();

        public static List<string[]> donorGames = new List<string[]>();
        public static List<string[]> newApps = new List<string[]>();

        #if WINDOWS
            private static string[] SplitLines(string input)
            {
                return input.Split(new[] { "\r\n" }, System.StringSplitOptions.None);
            }
        #elif LINUX
            private static string[] SplitLines(string input)
            {
                return input.Split(new[] { "\n" }, System.StringSplitOptions.None);
            }
        #endif

        public static void initDonorGames()
        {
            donorGameProperties.Clear();
            donorGames.Clear();
            if (!string.IsNullOrEmpty(MainForm.donorApps))
            {
                string[] gameListSplited = SplitLines(MainForm.donorApps);
                foreach (string game in gameListSplited)
                {
                    if (game.Length > 1)
                    {
                        donorGames.Add(game.Split(';'));
                    }
                }
            }
        }

        public static void initNewApps()
        {
            newApps.Clear();
            if (!string.IsNullOrEmpty(DonorsListViewForm.newAppsForList))
            {
                string[] newListSplited = SplitLines(DonorsListViewForm.newAppsForList);
                foreach (string game in newListSplited)
                {
                    if (game.Length > 1)
                    {
                        newApps.Add(game.Split(';'));
                    }
                }
            }
        }
    }
}
