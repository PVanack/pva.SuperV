using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using pva.Helpers;
using System.Net.NetworkInformation;

namespace pva.SuperV.TestContainers
{
    public class TDengineContainer : IDisposable
    {
        private IContainer? tdEngineContainer;
        private bool disposedValue;

        private static void WaitForPort(int port)
        {
            const int MaxWaitIndex = 50;
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            int index = 0;
            while (index < MaxWaitIndex && ipGlobalProperties.GetActiveTcpConnections().Any(pr => pr.LocalEndPoint.Port == port && pr.State == TcpState.Established))
            {
                Thread.Sleep(100);
                index++;
            }
            if (index == MaxWaitIndex)
            {
                throw new ApplicationException($"Port 6030 is in use after {MaxWaitIndex * 100} msecs");
            }
        }

        public async Task<string> StartTDengineContainerAsync()
        {
            if (tdEngineContainer is null)
            {
                WaitForPort(6030);
                tdEngineContainer = new ContainerBuilder()
                    .WithImage("tdengine/tdengine:3.3.6.0")
                    .WithPortBinding(6030)
                    .WithPortBinding(6031)
                    .WithPortBinding(6032)
                    .WithPortBinding(6033)
                    .WithPortBinding(6034)
                    .WithPortBinding(6035)
                    .WithPortBinding(6036)
                    .WithPortBinding(6037)
                    .WithPortBinding(6038)
                    .WithPortBinding(6039)
                    .WithPortBinding(6040)
                    .WithPortBinding(6041)
                    .WithPortBinding(6042)
                    .WithPortBinding(6043)
                    .WithPortBinding(6044)
                    .WithPortBinding(6045)
                    .WithPortBinding(6046)
                    .WithPortBinding(6047)
                    .WithPortBinding(6048)
                    .WithPortBinding(6049)
                    .WithPortBinding(6050)
                    .WithPortBinding(6051)
                    .WithPortBinding(6052)
                    .WithPortBinding(6053)
                    .WithPortBinding(6054)
                    .WithPortBinding(6055)
                    .WithPortBinding(6056)
                    .WithPortBinding(6057)
                    .WithPortBinding(6058)
                    .WithPortBinding(6059)
                    .WithPortBinding(6060)
                    .WithExtraHost("buildkitsandbox", "127.0.0.1")
                    .WithWaitStrategy(
                        Wait.ForUnixContainer()
                        .UntilExternalTcpPortIsAvailable(6030, strategy => strategy.WithTimeout(TimeSpan.FromSeconds(15)))
                    )
                    .Build();

                // Start the container.
                try
                {
                    Task tdEngineStartAsync = tdEngineContainer.StartAsync();
                    await tdEngineStartAsync.WaitAsync(TimeSpan.FromSeconds(30));
                    // Wait to make sure the processes in container are ready and running.
                    await WaitForTDengineToBeReady();
                }
                catch (Exception)
                {
                    await StopTDengineContainerAsync();
                    throw;
                }
            }
            return $"host={tdEngineContainer.Hostname};port={tdEngineContainer.GetMappedPublicPort(6030)};username=root;password=taosdata";
        }

        private async ValueTask WaitForTDengineToBeReady()
        {
            bool connected = false;
            int index = 0;
            while (!connected && index < 10)
            {
                SystemCommand.Run("taos", $"-h {tdEngineContainer!.Hostname} -P {tdEngineContainer.GetMappedPublicPort(6030)} -s \"show dnodes\"", out string output, out _);
                connected = output.Contains("ready");
                if (!connected)
                {
                    Thread.Sleep(500);
                    index++;
                }
            }
            if (!connected)
            {
                var (Stdout, Stderr) = await tdEngineContainer!.GetLogsAsync();
                throw new ApplicationException($"Can't connect to TDengine container {tdEngineContainer!.Hostname}! Out: {Stdout}. Error: {Stderr}");
            }
        }

        public async Task<long> StopTDengineContainerAsync()
        {
            if (tdEngineContainer is not null)
            {
                await tdEngineContainer.StopAsync()
                    .ConfigureAwait(false);
                long exitCode = await tdEngineContainer.GetExitCodeAsync();
                WaitForPort(6030);
                tdEngineContainer = null;
                return exitCode;
            }
            return 0;
        }

        protected virtual async Task Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    await StopTDengineContainerAsync();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Task.Run(async () => await Dispose(disposing: true));
            GC.SuppressFinalize(this);
        }
    }
}
