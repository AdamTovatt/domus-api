using HueApi;
using HueApi.Models.Responses;

namespace DomusApi.Helpers
{
    public class HueEventHandlingService : BackgroundService
    {
        private LocalHueApi? hueApi;

        public HueEventHandlingService() { }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (hueApi == null)
                hueApi = await HueApiFactory.CreateLocalHueApiAsync();

            hueApi.OnEventStreamMessage += HandleEventStreamMessage;
            await hueApi.StartEventStream(cancellationToken: stoppingToken);

            // Wait until the application is stopping
            stoppingToken.Register(OnStopping);
        }

        private void OnStopping() { }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
        }

        private void HandleEventStreamMessage(string bridgeIp, List<EventStreamResponse> events)
        {
            Console.WriteLine($"{events.Count} new events");

            foreach (EventStreamResponse hueEvent in events)
            {
                foreach (EventStreamData data in hueEvent.Data)
                {
                    Console.WriteLine($"Data: {data.Metadata?.Name} / {data.IdV1}");
                }
            }
        }
    }
}
