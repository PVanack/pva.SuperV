using System.Diagnostics;

namespace pva.SuperV.EngineTests
{
    public static class SystemCommand
    {
        public static void Run(string command, string args, out string output, out string error, string? directory = null)
        {
            string actualCommand = String.Empty;
            string arguments = command;
            if (OperatingSystem.IsWindows())
            {
                actualCommand = "cmd.exe";
                arguments = $"/c {command} {args}";
            }
            else if (OperatingSystem.IsLinux())
            {
                actualCommand = command;
                arguments = args;
            }
            using Process process = new()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = actualCommand,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    Arguments = arguments,
                    CreateNoWindow = true,
                    WorkingDirectory = directory ?? string.Empty,
                }
            };
            process.Start();
            process.WaitForExit();
            output = process.StandardOutput.ReadToEnd();
            error = process.StandardError.ReadToEnd();
        }
    }

}
