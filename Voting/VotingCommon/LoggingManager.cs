using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Fabric;

namespace VotingCommon
{
    public class LoggingManager
    {
        public static void CreateLogger(ServiceContext serviceContext)
        {
            AppDomain.CurrentDomain.ProcessExit += (sender, args) => Log.CloseAndFlush();

            var configurationSection = serviceContext.CodePackageActivationContext.GetConfigurationPackageObject("Config").Settings.Sections["SerilogConfigSection"];
            var elasticSearchEndpoint = configurationSection.Parameters["EsEndPoint"].Value;
            var indexFormat = configurationSection.Parameters["IndexFormat"].Value;
            var minimumLogLevel = configurationSection.Parameters["MinimumLogLevel"].Value;
            var esEndPointUserName = configurationSection.Parameters["EsEndPointUserName"].Value;
            var esEndPointPassword = configurationSection.Parameters["EsEndPointPassword"].Value;
            var minimumLogEventLevel = (LogEventLevel)Enum.Parse(typeof(LogEventLevel), minimumLogLevel, true);

            var loggerConfiguration = new LoggerConfiguration()
                  .MinimumLevel.ControlledBy(new Serilog.Core.LoggingLevelSwitch(minimumLogEventLevel))
                  .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticSearchEndpoint))
                  {
                      AutoRegisterTemplate = true,
                      IndexFormat = indexFormat,
                      AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
                      MinimumLogEventLevel = LogEventLevel.Debug,
                      ModifyConnectionSettings = conn =>
                      {
                          conn.BasicAuthentication(esEndPointUserName, esEndPointPassword);
                          return conn;
                      }
                  })
                 .Enrich.WithProperty("ServiceName", serviceContext.ServiceName.ToString())
                 .Enrich.WithProperty("ServiceTypeName", serviceContext.ServiceTypeName)
                 .Enrich.WithProperty("ReplicaOrInstanceId", serviceContext.ReplicaOrInstanceId)
                 .Enrich.WithProperty("PartitionId", serviceContext.PartitionId.ToString())
                 .Enrich.WithProperty("ApplicationName", serviceContext.CodePackageActivationContext.ApplicationName)
                 .Enrich.WithProperty("ApplicationTypeName", serviceContext.CodePackageActivationContext.ApplicationTypeName)
                 .Enrich.WithProperty("NodeName", serviceContext.NodeContext.NodeName);

            var log = loggerConfiguration.CreateLogger();
            Log.Logger = log;
        }
    }
}
