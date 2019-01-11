using System.Management.Instrumentation;
using System.ServiceProcess;
using Topshelf;

namespace RMAWindowsService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            HostFactory.Run(config =>
            {
                config.UseNLog();
                config.Service<DataBridge>(instance =>
                {
                    instance.ConstructUsing(() => new DataBridge());
                    instance.WhenStarted(execute => execute.Start()); // Declare WhenStarted is a must
                    instance.WhenStopped(execute => execute.Stop()); // Declare WhenStopped is a must

                });
                config.SetServiceName("RMADataBridge");
                config.SetDisplayName("RMA Data Bridge");
                config.SetDescription("RMA - Sync Data from Sharepoint and Active Directory");
                config.StartAutomatically();

                config.EnableServiceRecovery(options =>
                {
                    options.RestartService(3);
                });

                config.OnException(ex =>
                {
                    // Log ex
                });
            });
        }
    }
}
