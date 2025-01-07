﻿using JR.Utils.GUI.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AndroidSideloader
{
    internal class GetDependencies
    {
        public static void updatePublicConfig()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                                 | SecurityProtocolType.Tls11
                                                 | SecurityProtocolType.Tls12
                                                 | SecurityProtocolType.Ssl3;

            _ = Logger.Log("Attempting to update public config from main.");

            string configUrl = "https://raw.githubusercontent.com/vrpyou/quest/main/vrp-public.json";
            string fallbackUrl = "https://vrpirates.wiki/downloads/vrp-public.json";

            try
            {
                string resultString;

                // Try fetching raw JSON data from the provided link
                HttpWebRequest getUrl = (HttpWebRequest)WebRequest.Create(configUrl);
                using (StreamReader responseReader = new StreamReader(getUrl.GetResponse().GetResponseStream()))
                {
                    resultString = responseReader.ReadToEnd();
                    _ = Logger.Log($"Retrieved updated config from main: {configUrl}.");
                    File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "vrp-public.json"), resultString);
                    _ = Logger.Log("Public config updated successfully from main.");
                }
            }
            catch (Exception mainException)
            {
                _ = Logger.Log($"Failed to update public config from main: {mainException.Message}, trying fallback.", LogLevel.ERROR);
                try
                {
                    HttpWebRequest getUrl = (HttpWebRequest)WebRequest.Create(fallbackUrl);
                    using (StreamReader responseReader = new StreamReader(getUrl.GetResponse().GetResponseStream()))
                    {
                        string resultString = responseReader.ReadToEnd();
                        _ = Logger.Log($"Retrieved updated config from fallback: {fallbackUrl}.");
                        File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "vrp-public.json"), resultString);
                        _ = Logger.Log("Public config updated successfully from fallback.");
                    }
                }
                catch (Exception fallbackException)
                {
                    _ = Logger.Log($"Failed to update public config from fallback: {fallbackException.Message}.", LogLevel.ERROR);
                }
            }
        }

        // Download required dependencies.
        public static void downloadFiles()
        {
            MainForm.SplashScreen.UpdateBackgroundImage(AndroidSideloader.Properties.Resources.splashimage_deps);
            
            WebClient client = new WebClient();
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var currentAccessedWebsite = "";
            try
            {
                if (!File.Exists("Sideloader Launcher.exe"))
                {
                    currentAccessedWebsite = "github";
                    _ = Logger.Log($"Missing 'Sideloader Launcher.exe'. Attempting to download from {currentAccessedWebsite}");
                    client.DownloadFile("https://github.com/VRPirates/rookie/raw/master/Sideloader%20Launcher.exe", "Sideloader Launcher.exe");
                    _ = Logger.Log($"'Sideloader Launcher.exe' download successful");
                #if WINDOWS
                                if (!File.Exists("Rookie Offline.cmd"))
                                {
                                    currentAccessedWebsite = "github";
                                    _ = Logger.Log($"Missing 'Rookie Offline.cmd'. Attempting to download from {currentAccessedWebsite}");
                                    client.DownloadFile("https://github.com/VRPirates/rookie/raw/master/Rookie%20Offline.cmd", "Rookie Offline.cmd");
                                    _ = Logger.Log($"'Rookie Offline.cmd' download successful");
                                }

                                if (!File.Exists("CleanupInstall.cmd"))
                                {
                                    currentAccessedWebsite = "github";
                                    _ = Logger.Log($"Missing 'CleanupInstall.cmd'. Attempting to download from {currentAccessedWebsite}");
                                    client.DownloadFile("https://github.com/VRPirates/rookie/raw/master/CleanupInstall.cmd", "CleanupInstall.cmd");
                                    _ = Logger.Log($"'CleanupInstall.cmd' download successful");
                                }

                                currentAccessedWebsite = "github";
                                        _ = Logger.Log($"Missing 'AddDefenderExceptions.ps1'. Attempting to download from {currentAccessedWebsite}");
                                        client.DownloadFile("https://github.com/VRPirates/rookie/raw/master/AddDefenderExceptions.ps1", "AddDefenderExceptions.ps1");
                                        _ = Logger.Log($"'AddDefenderExceptions.ps1' download successful");
                #elif LINUX
                                if (!File.Exists("Rookie Offline.sh"))
                                {
                                    currentAccessedWebsite = "github";
                                    _ = Logger.Log($"Missing 'Rookie Offline.sh'. Attempting to download from {currentAccessedWebsite}");
                                    client.DownloadFile("https://github.com/VRPirates/rookie/raw/master/Rookie%20Offline.sh", "Rookie Offline.sh");
                                    _ = Logger.Log($"'Rookie Offline.sh' download successful");
                                }

                                if (!File.Exists("CleanupInstall.sh"))
                                {
                                    currentAccessedWebsite = "github";
                                    _ = Logger.Log($"Missing 'CleanupInstall.sh'. Attempting to download from {currentAccessedWebsite}");
                                    client.DownloadFile("https://github.com/VRPirates/rookie/raw/master/CleanupInstall.sh", "CleanupInstall.sh");
                                    _ = Logger.Log($"'CleanupInstall.sh' download successful");
                                }
                #endif
                }
            }
            catch (Exception ex)
            {
                _ = FlexibleMessageBox.Show($"You are unable to access raw.githubusercontent.com with the Exception:\n{ex.Message}\n\nSome files may be missing (Offline/Cleanup Script, Launcher)");
            }

            try
            {
#if WINDOWS
                string adbPath = $"{Path.GetPathRoot(Environment.SystemDirectory)}RSL\\platform-tools\\adb.exe";
#elif LINUX
                string adbPath = $"{Environment.GetEnvironmentVariable("XDG_DATA_HOME")}/rookie/platform-tools/adb";
#endif
                if (!File.Exists(adbPath)) //if adb is not updated, download and auto extract
                {
#if WINDOWS
                    string platformToolsPath = $"{Path.GetPathRoot(Environment.SystemDirectory)}RSL\\platform-tools";
#elif LINUX
                    string platformToolsPath = $"{Environment.GetEnvironmentVariable("XDG_DATA_HOME")}/rookie/platform-tools";
#endif
                    if (!Directory.Exists(platformToolsPath))
                    {
                        _ = Directory.CreateDirectory(platformToolsPath);
                    }

                    currentAccessedWebsite = "github";
                    _ = Logger.Log($"Missing adb within {platformToolsPath}. Attempting to download from {currentAccessedWebsite}");
                    client.DownloadFile("https://github.com/VRPirates/rookie/raw/master/dependencies.7z", "dependencies.7z");
                    Utilities.Zip.ExtractFile(Path.Combine(Environment.CurrentDirectory, "dependencies.7z"), platformToolsPath);
                    File.Delete("dependencies.7z");
                    _ = Logger.Log($"adb download successful");
                }
            }
            catch (Exception ex)
            {
                _ = FlexibleMessageBox.Show($"You are unable to access raw.githubusercontent.com page with the Exception:\n{ex.Message}\n\nSome files may be missing (ADB)");
                _ = FlexibleMessageBox.Show("ADB was unable to be downloaded\nRookie will now close.");
                Application.Exit();
            }

            string wantedRcloneVersion = "1.68.2";
            bool rcloneSuccess = false;

            rcloneSuccess = downloadRclone(wantedRcloneVersion, false);
            if (!rcloneSuccess) {
                rcloneSuccess = downloadRclone(wantedRcloneVersion, true);
            }
            if (!rcloneSuccess) {
                _ = Logger.Log($"Unable to download rclone", LogLevel.ERROR);
                _ = FlexibleMessageBox.Show("Rclone was unable to be downloaded\nRookie will now close, please use Offline Mode for manual sideloading if needed");
                Application.Exit();
            }
        }


        public static bool downloadRclone(string wantedRcloneVersion, bool useFallback = false)
        {
            try
            {
                bool updateRclone = false;
                string currentRcloneVersion = "0.0.0";

                WebClient client = new WebClient();
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                _ = Logger.Log($"Checking for Local rclone...");
#if WINDOWS
                string dirRclone = Path.Combine(Environment.CurrentDirectory, "rclone");
                string pathToRclone = Path.Combine(dirRclone, "rclone.exe");
#elif LINUX
                string dirRclone = Path.Combine(Environment.GetEnvironmentVariable("XDG_DATA_HOME"), "rookie/rclone");
                string pathToRclone = Path.Combine(dirRclone, "rclone");
#endif
                if (File.Exists(pathToRclone))
                {
                    var versionInfo = FileVersionInfo.GetVersionInfo(pathToRclone);
                    currentRcloneVersion = versionInfo.ProductVersion;
                    Logger.Log($"Current RCLONE Version {currentRcloneVersion}");
                    if (!MainForm.noRcloneUpdating)
                    {
                        if (currentRcloneVersion != wantedRcloneVersion)
                        {
                            updateRclone = true;
                            _ = Logger.Log($"RCLONE Version does not match ({currentRcloneVersion})! Downloading required version ({wantedRcloneVersion})");
                        }
                    }
                } else {
                    updateRclone = true;
                    _ = Logger.Log($"RCLONE exe does not exist, attempting to download");
                }

                if (!Directory.Exists(dirRclone)) {
                    updateRclone = true;
                    _ = Logger.Log($"Missing RCLONE Folder, attempting to download");

                    Directory.CreateDirectory(dirRclone);
                }

                if (updateRclone == true)
                {
                    // Preserve vrp.download.config if it exists
                    string configPath = Path.Combine(dirRclone, "vrp.download.config");
                    string tempConfigPath = Path.Combine(Environment.CurrentDirectory, "vrp.download.config.bak");
                    bool hasConfig = false;

                    if (File.Exists(configPath))
                    {
                        _ = Logger.Log("Preserving vrp.download.config before update");
                        File.Copy(configPath, tempConfigPath, true);
                        hasConfig = true;
                    }

                    MainForm.SplashScreen.UpdateBackgroundImage(AndroidSideloader.Properties.Resources.splashimage_rclone);

                    string architecture = Environment.Is64BitOperatingSystem ? "amd64" : "386";
#if WINDOWS
                    string url = $"https://downloads.rclone.org/v{wantedRcloneVersion}/rclone-v{wantedRcloneVersion}-windows-{architecture}.zip";
#elif LINUX
                    string url = $"https://downloads.rclone.org/v{wantedRcloneVersion}/rclone-v{wantedRcloneVersion}-linux-{architecture}.zip";
#endif
                    if (useFallback == true) {
                        _ = Logger.Log($"Using git fallback for rclone download");
#if WINDOWS
                        url = $"https://raw.githubusercontent.com/VRPirates/rookie/master/dep/rclone-v{wantedRcloneVersion}-windows-{architecture}.zip";
#elif LINUX
                        url = $"https://raw.githubusercontent.com/VRPirates/rookie/master/dep/rclone-v{wantedRcloneVersion}-linux-{architecture}.zip";
#endif
                    }
                    _ = Logger.Log($"Downloading rclone from {url}");

                    _ = Logger.Log("Begin download rclone");
                    client.DownloadFile(url, "rclone.zip");
                    _ = Logger.Log("Complete download rclone");

                    _ = Logger.Log($"Extract {Environment.CurrentDirectory}\\rclone.zip");
                    Utilities.Zip.ExtractFile(Path.Combine(Environment.CurrentDirectory, "rclone.zip"), Environment.CurrentDirectory);
#if WINDOWS
                    string dirExtractedRclone = Path.Combine(Environment.CurrentDirectory, $"rclone-v{wantedRcloneVersion}-windows-{architecture}");
#elif LINUX
                    string dirExtractedRclone = Path.Combine(Environment.CurrentDirectory, $"rclone-v{wantedRcloneVersion}-linux-{architecture}");
#endif
                    File.Delete("rclone.zip");
                    _ = Logger.Log("rclone extracted. Moving files");

                    foreach (string file in Directory.GetFiles(dirExtractedRclone))
                    {
                        string fileName = Path.GetFileName(file);
                        string destFile = Path.Combine(dirRclone, fileName);
                        if (File.Exists(destFile))
                        {
                            File.Delete(destFile);
                        }
                        File.Move(file, destFile);
                    }
                    Directory.Delete(dirExtractedRclone, true);

                    // Restore vrp.download.config if it was backed up
                    if (hasConfig && File.Exists(tempConfigPath))
                    {
                        _ = Logger.Log("Restoring vrp.download.config after update");
                        File.Move(tempConfigPath, configPath);
                    }

                    _ = Logger.Log($"rclone download successful");
                }

                return true;
            }
            catch (Exception ex)
            {
#if LINUX
                _ = Logger.Log($"Unable to download rclone: {ex}", LogLevel.ERROR);
                _ = FlexibleMessageBox.Show("Rclone was unable to be downloaded. Please install rclone manually using your package manager or download it from the official website.");
                System.Diagnostics.Process.Start("zenity", "--error --text='Rclone was unable to be downloaded. Please install rclone manually using your package manager or download it from the official website.'");
#endif
                return false;
            }
        }
    }
}
