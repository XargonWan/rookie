using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
#if LINUX
using System.Runtime.InteropServices;
#endif
using System.Text;
using System.Windows.Forms;

namespace AndroidSideloader.Utilities
{
    internal class GeneralUtilities
    {
        public static long GetDirectorySize(string folderPath)
        {
            DirectoryInfo di = new DirectoryInfo(folderPath);
            return di.EnumerateFiles("*", SearchOption.AllDirectories).Sum(fi => fi.Length);
        }

        public static string RandomPackageName()
        {
            return $"com.{GeneralUtilities.randomString(rand.Next(3, 8))}.{GeneralUtilities.randomString(rand.Next(3, 8))}";
        }
        public static string CommandOutput = "";
        public static string CommandError = "";

        public static void ExecuteCommand(string command)
        {
            #if WINDOWS
                        string shell = "cmd.exe";
                        string shellArgs = "/c " + command;
            #elif LINUX
                        string shell = "/bin/bash";
                        string shellArgs = "-c \"" + command + "\"";
            #endif

                        ProcessStartInfo processInfo = new ProcessStartInfo(shell, shellArgs)
                        {
                            CreateNoWindow = true,
                            UseShellExecute = false,
                            RedirectStandardError = true,
                            RedirectStandardOutput = true
                        };

            Process process = Process.Start(processInfo);

            process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
                CommandOutput += e.Data;
            process.BeginOutputReadLine();

            process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
                CommandError += e.Data;
            process.BeginErrorReadLine();

            process.WaitForExit();

            process.Close();
        }

        public static void Melt()
        {
            #if WINDOWS
                        string shell = "cmd.exe";
                        string shellArgs = "/C choice /C Y /N /D Y /T 5 & Del \"" + Application.ExecutablePath + "\"";
            #elif LINUX
                        string shell = "/bin/bash";
                        string shellArgs = "-c \"sleep 5 && rm -f '" + Application.ExecutablePath + "'\"";
            #endif

            _ = Process.Start(new ProcessStartInfo()
            {
                Arguments = shellArgs,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                FileName = shell
            });
            Environment.Exit(0);
        }

        private static readonly Random rand = new Random();
        public static string randomString(int length)
        {
            string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            StringBuilder res = new StringBuilder();

            int randomInteger = rand.Next(0, valid.Length);
            while (0 < length--)
            {
                _ = res.Append(valid[randomInteger]);
                randomInteger = rand.Next(0, valid.Length);
            }
            return res.ToString();
        }

        public static ProcessOutput startProcess(string process, string path, string command)
        {
            _ = Logger.Log($"Ran process {process} with command {command} in path {path}");
            Process cmd = new Process();
#if WINDOWS
            cmd.StartInfo.FileName = "cmd.exe";
#elif LINUX
            cmd.StartInfo.FileName = "/bin/bash";
#endif
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.RedirectStandardError = true;
            cmd.StartInfo.WorkingDirectory = path;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            _ = cmd.Start();
            cmd.StandardInput.WriteLine(command);
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            string error = cmd.StandardError.ReadToEnd();
            string output = cmd.StandardOutput.ReadToEnd();
            _ = Logger.Log($"Output: {output}");
            _ = Logger.Log($"Error: {error}", LogLevel.ERROR);
            return new ProcessOutput(output, error);
        }

    }
}
