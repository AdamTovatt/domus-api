using DomusApi.Models;
using DomusApi.Repositories;
using HueApi;
using HueApi.Models.Clip;

namespace DomusApi.Helpers
{
    public class HueApiFactory
    {
        public async static Task<LocalHueApi> CreateLocalHueApiAsync()
        {
            HueCredentials? hueCredentials = await HueCredentialsRepository.Instance.GetAsync();

            if (hueCredentials == null)
                hueCredentials = await SetupHueApi();

            if (hueCredentials == null)
                throw new ArgumentNullException(nameof(hueCredentials), "Missing HUE credentials. Call the setup endpoint first.");

            LocalHueApi localHueApi = new LocalHueApi(hueCredentials.IpAddress, hueCredentials.Username);
            return localHueApi;
        }

        private static async Task<HueCredentials> SetupHueApi()
        {
            RegisterEntertainmentResult? registerResult = await LocalHueApi.RegisterAsync("192.168.68.106", "domus-api", "hercules", true);

            if (registerResult == null || registerResult.Username == null || registerResult.StreamingClientKey == null || registerResult.Ip == null)
                throw new Exception("Register result was null but no exception was thrown");

            HueCredentials credentials = new HueCredentials(0, registerResult.StreamingClientKey, registerResult.Ip, registerResult.Username);
            await HueCredentialsRepository.Instance.InsertAsync(credentials);

            return credentials;
        }
    }
}
