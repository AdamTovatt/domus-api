using DomusApi.Helpers;
using HueApi;
using HueApi.Models;
using HueApi.Models.Sensors;

namespace DomusApiTests
{
    [TestClass]
    public class HueApiTests
    {
        private static LocalHueApi hueApi = null!;

        [ClassInitialize]
        public static async Task BeforeAll(TestContext testContext)
        {
            hueApi = await HueApiFactory.CreateLocalHueApiAsync();
        }

        [TestMethod]
        public async Task GetButtons()
        {
            HueResponse<ButtonResource> buttons = await hueApi.GetButtonsAsync();
        }


        [TestMethod]
        public async Task GetDevices()
        {
            HueResponse<Device> devices = await hueApi.GetDevicesAsync();
            HueResponse<Device> device = await hueApi.GetDeviceAsync(Guid.Parse("18cc6941-98cc-45e7-ae30-f6e27870f7eb"));
        }

        [TestMethod]
        public async Task GetServices()
        {
            HueResponse<ContactSensor> services = await hueApi.GetContactSensorsAsync();
            HueResponse<DevicePower> powers = await hueApi.GetDevicePowersAsync();
            HueResponse<HueApi.Models.Light> lightService = await hueApi.GetLightsAsync();
            HueResponse<ButtonResource> buttons = await hueApi.GetButtonsAsync();
        }

        [TestMethod]
        public async Task GetRooms()
        {
            HueResponse<Room> rooms = await hueApi.GetRoomsAsync();
        }

        [TestMethod]
        public async Task GetGroupedLights()
        {
            HueResponse<GroupedLight> lights = await hueApi.GetGroupedLightsAsync();
        }

        [TestMethod]
        public async Task HomeHelperGetRooms()
        {
            HomeHelper home = new HomeHelper(hueApi);

            await home.GetRoomsAsync();
        }
    }
}