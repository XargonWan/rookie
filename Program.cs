using AndroidSideloader.Utilities;
using System;
using System.IO;
using System.Security.Permissions;
#if LINUX
using Gtk;
#else
using System.Windows.Forms;
#endif

namespace AndroidSideloader
{
    internal static class Program
    {
        private static readonly SettingsManager settings = SettingsManager.Instance;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        #if !LINUX
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        #endif
        private static void Main()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(CrashHandler);
            #if LINUX
            Application.Init();
            form = new MainForm();
            form.Show();
            Application.Run();
            #else
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            form = new MainForm();
            Application.Run(form);
            #endif
        }
        public static MainForm form;

        private static void CrashHandler(object sender, UnhandledExceptionEventArgs args)
        {
            // Capture unhandled exceptions and write to file.
            Exception e = (Exception)args.ExceptionObject;
            string innerExceptionMessage = (e.InnerException != null)
                ? e.InnerException.Message
                : "None";
            string date_time = DateTime.Now.ToString("dddd, MMMM dd @ hh:mmtt (UTC)");
            File.WriteAllText(Sideloader.CrashLogPath, $"Date/Time of crash: {date_time}\nMessage: {e.Message}\nInner Message: {innerExceptionMessage}\nData: {e.Data}\nSource: {e.Source}\nTargetSite: {e.TargetSite}\nStack Trace: \n{e.StackTrace}\n\n\nDebuglog: \n\n\n");
            // If a debuglog exists we append it to the crashlog.
            if (File.Exists(settings.CurrentLogPath))
            {
                File.AppendAllText(Sideloader.CrashLogPath, File.ReadAllText($"{settings.CurrentLogPath}"));
            }
        }
    }
}
