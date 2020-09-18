using CloudEventsDemo.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CloudEventsDemo.ConsoleA
{
    class Program
    {
        #region Sample Data

        static BasicPayload p1 = new BasicPayload()
        {
            Id = 1,
            Description = "publisher payload",
            Tags = new List<string>() { "tag0", "tag1", "tag2", "tag3" }
        };

        static ExtendedPayload p2 = new ExtendedPayload()
        {
            Id = 2,
            Description = "publisher payload, extended",
            Tags = new List<string>() { "tag10", "tag11", "tag12", "tag13" },
            Properties = new Dictionary<string, object>() { { "p0", "v0" }, { "p1", "v1" } }
        };

        #endregion

        #region App-specific CloudEvents configuration

        /// <summary>
        ///  Consumer-managed mapping between 
        ///  declarative CloudEvents types AND 
        ///  actual .NET types used to deserialize the event payload
        /// </summary>
        static Dictionary<string, Type> subscriberTypeMappings = new Dictionary<string, Type>(){ 
            { "urn:my-app:basic-payload", typeof(ConsumerPayload)},
            { "urn:my-app:extended-payload", typeof(ConsumerPayload)} };

        #endregion

        static IHostBuilder ConfigureHost(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true);
                    config.AddEnvironmentVariables();

                    if (args != null)
                        config.AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services
                    .AddOptions()
                    .AddLogging(builder => builder
                           .AddConfiguration(hostContext.Configuration.GetSection("Logging"))
                           .AddConsole(options =>
                           {
                               options.IncludeScopes = true;
                           })
                           .AddDebug()
                    )
                    .AddTransient<ICloudEventWriter, CloudEventWriter>(sp =>
                    {
                        return new CloudEventWriter("my-app", new Uri("http://cloud-events-demo.com/console-a"),
                            sp.GetRequiredService<ILogger<CloudEventWriter>>());
                    })
                    .AddTransient<ICloudEventReader, CloudEventReader>(sp =>
                    {
                        return new CloudEventReader(subscriberTypeMappings,
                            sp.GetRequiredService<ILogger<CloudEventReader>>());
                    });
                });

            return builder;
        }

        static async Task Workload(CancellationTokenSource cancellationSource)
        {
            byte[] eventBytes;
            ConsumerPayload consumerPayload;
            string eventWithEnvelope;

            var ceWriter = HostHelpers.serviceProvider.GetService<ICloudEventWriter>();
            var ceReader = HostHelpers.serviceProvider.GetService<ICloudEventReader>();

            // to see the full envelope and payload sent to the transport-specific broker, set logger verbosity to "Debug"
            eventBytes = ceWriter.GetBytes<BasicPayload>(p1,
                eventSubject: $"New event with basic payload, id = {p1.Id}");
            consumerPayload = ceReader.GetPayload(eventBytes) as ConsumerPayload;
            Console.WriteLine();

            eventBytes = ceWriter.GetBytes<ExtendedPayload>(p2,
                eventSubject: $"New event with extended payload, id = {p2.Id}");
            consumerPayload = ceReader.GetPayload(eventBytes) as ConsumerPayload;
            Console.WriteLine();

            eventBytes = ceWriter.GetBytes<KeyValuePair<string, string>>(new KeyValuePair<string, string>("pKey", "pValue"),
                eventSubject: $"New event without a consumer type mapping");
            consumerPayload = ceReader.GetPayload(eventBytes) as ConsumerPayload; // null
            Console.WriteLine();

            await Task.CompletedTask;
            cancellationSource.Cancel();
        }

        static void Main(string[] args)
        {
            CancellationTokenSource cancelSrc = new CancellationTokenSource();
            CancellationToken cancelToken = cancelSrc.Token;

            try
            {
                Console.WriteLine("Console A configuring host...");
                var hostBuilder = ConfigureHost(args);

                Console.WriteLine("Console A started...");
                Task.WaitAll(new Task[] {
                    hostBuilder.UseConsoleLifetime()
                        .Build()
                        .GetServiceProvider()
                        .RunAsync(cancelToken),//RunConsoleAsync(cancelToken) + custom handling to retain the service provider
                    Workload(cancelSrc)
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine($"cloudEventsDemo.ConsoleA: failed with exception '{ex.Message}', of type '{ex.GetType().Name}'");
            }
            finally 
            {
                cancelSrc.Dispose();
                Console.WriteLine("Console A exiting...");
            }
        }
    }
}
