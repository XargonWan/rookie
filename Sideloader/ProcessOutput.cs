namespace AndroidSideloader
{
    public class ProcessOutput
    {
        public string Output;
        public string Error;

        public ProcessOutput(string output = "", string error = "")
        {
            Output = output;
            Error = error;
        }

        public static ProcessOutput operator +(ProcessOutput a, ProcessOutput b)
        {
            return new ProcessOutput(a.Output + b.Output, a.Error + b.Error);
        }

        public static string GetApplicationDataPath()
        {
#if WINDOWS
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\rookie";
#elif LINUX
            return Environment.GetEnvironmentVariable("XDG_DATA_HOME") + "/rookie";
#else
            throw new PlatformNotSupportedException("Unsupported OS");
#endif
        }
    }
}
