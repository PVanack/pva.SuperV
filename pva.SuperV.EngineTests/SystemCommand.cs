using System.Diagnostics;

namespace pva.SuperV.EngineTests
{
    public static class SystemCommand
    {
        public static void Run(string command, out string output, out string error, string directory = null)
        {
            string commandInterpreter = String.Empty;
            string actualCommand = command;
            if (OperatingSystem.IsWindows())
            {
                commandInterpreter = "cmd.exe";
                actualCommand = $"/c {actualCommand}";
            }
            else if (OperatingSystem.IsLinux())
            {
                commandInterpreter = "/bin/bash";
            }
            using Process process = new()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = commandInterpreter,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    Arguments = actualCommand,
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
