using Microsoft.ServiceFabric.Services.Runtime;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using VotingCommon;

namespace VotingWeb
{
    internal static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            try
            {
                // The ServiceManifest.XML file defines one or more service type names.
                // Registering a service maps a service type name to a .NET type.
                // When Service Fabric creates an instance of this service type,
                // an instance of the class is created in this host process.

                ServiceRuntime.RegisterServiceAsync("VotingWebType",
                    context => {
                        LoggingManager.CreateLogger(context);
                        return new VotingWeb(context, Log.Logger);
                    }).GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(VotingWeb).Name);

                // Prevents this host process from terminating so services keeps running. 
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }

        //private static void ConfigureLogging(ServiceContext serviceContext)
        //{
        //    AppDomain.CurrentDomain.ProcessExit += (sender, args) => Log.CloseAndFlush();

        //    var configurationSection = serviceContext.CodePackageActivationContext.GetConfigurationPackageObject("Config").Settings.Sections["SerilogConfigSection"];
        //    var elasticSearchEndpoint = configurationSection.Parameters["EsEndPoint"].Value;
        //    var indexFormat = configurationSection.Parameters["IndexFormat"].Value;
        //    var minimumLogLevel = configurationSection.Parameters["MinimumLogLevel"].Value;
        //    var esEndPointUserName = configurationSection.Parameters["EsEndPointUserName"].Value;
        //    var esEndPointPassword = configurationSection.Parameters["EsEndPointPassword"].Value;
        //    var minimumLogEventLevel = (LogEventLevel)Enum.Parse(typeof(LogEventLevel), minimumLogLevel, true);

        //    var loggerConfiguration = new LoggerConfiguration()
        //          .MinimumLevel.ControlledBy(new Serilog.Core.LoggingLevelSwitch(minimumLogEventLevel))
        //          .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticSearchEndpoint))
        //          {
        //              AutoRegisterTemplate = true,
        //              IndexFormat = indexFormat,
        //              AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
        //              MinimumLogEventLevel = LogEventLevel.Debug,
        //              ModifyConnectionSettings = conn =>
        //              {
        //                  conn.BasicAuthentication(esEndPointUserName, esEndPointPassword);
        //                  return conn;
        //              }
        //          })
        //         .Enrich.WithProperty("ServiceName", serviceContext.ServiceName.ToString())
        //         .Enrich.WithProperty("ServiceTypeName", serviceContext.ServiceTypeName)
        //         .Enrich.WithProperty("ReplicaOrInstanceId", serviceContext.ReplicaOrInstanceId)
        //         .Enrich.WithProperty("PartitionId", serviceContext.PartitionId.ToString())
        //         .Enrich.WithProperty("ApplicationName", serviceContext.CodePackageActivationContext.ApplicationName)
        //         .Enrich.WithProperty("ApplicationTypeName", serviceContext.CodePackageActivationContext.ApplicationTypeName)
        //         .Enrich.WithProperty("NodeName", serviceContext.NodeContext.NodeName);

        //    var log = loggerConfiguration.CreateLogger();
        //    Log.Logger = log;
        //}
    }
}
