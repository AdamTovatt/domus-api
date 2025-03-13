using DomusApi.Models.Devices;
using HueApi;
using HueApi.Models;
using HueApi.Models.Sensors;
using System.Collections.Concurrent;

namespace DomusApi.Helpers
{
    public class HomeHelper
    {
        private LocalHueApi hueApi;

        private ConcurrentDictionary<Guid, Room> roomsCache = new ConcurrentDictionary<Guid, Room>();
        private ConcurrentDictionary<Guid, Device> devicesCache = new ConcurrentDictionary<Guid, Device>();
        private ConcurrentDictionary<Guid, HueApi.Models.Light> lightsCache = new ConcurrentDictionary<Guid, HueApi.Models.Light>();
        private ConcurrentDictionary<Guid, ButtonResource> buttonsCache = new ConcurrentDictionary<Guid, ButtonResource>();
        private ConcurrentDictionary<Guid, ContactSensor> contactSensorsCache = new ConcurrentDictionary<Guid, ContactSensor>();
        private ConcurrentDictionary<Guid, MotionResource> motionSensorsCache = new ConcurrentDictionary<Guid, MotionResource>();
        private ConcurrentDictionary<Guid, ZigbeeConnectivity> zigbeeConnectivityCache = new ConcurrentDictionary<Guid, ZigbeeConnectivity>();
        private ConcurrentDictionary<Guid, LightLevel> lightLevelsCache = new ConcurrentDictionary<Guid, LightLevel>();

        public HomeHelper(LocalHueApi hueApi)
        {
            this.hueApi = hueApi;
        }

        private async Task InitializeCachedValuesAsync()
        {
            HueResponse<Room> roomsResponse = await hueApi.GetRoomsAsync();
            HueResponse<Device> devicesResponse = await hueApi.GetDevicesAsync();
            HueResponse<HueApi.Models.Light> lightsResponse = await hueApi.GetLightsAsync();
            HueResponse<ButtonResource> buttonsResponse = await hueApi.GetButtonsAsync();
            HueResponse<ContactSensor> contactSensorsResponse = await hueApi.GetContactSensorsAsync();
            HueResponse<MotionResource> motionsResponse = await hueApi.GetMotionsAsync();
            HueResponse<ZigbeeConnectivity> zigbeeResponse = await hueApi.GetZigbeeConnectivityAsync();
            HueResponse<LightLevel> lightLevelResponse = await hueApi.GetLightLevelsAsync();

            roomsCache = new ConcurrentDictionary<Guid, Room>(roomsResponse.Data.ToDictionary(r => r.Id, r => r));
            devicesCache = new ConcurrentDictionary<Guid, Device>(devicesResponse.Data.ToDictionary(d => d.Id, d => d));
            lightsCache = new ConcurrentDictionary<Guid, HueApi.Models.Light>(lightsResponse.Data.ToDictionary(l => l.Id, l => l));
            buttonsCache = new ConcurrentDictionary<Guid, ButtonResource>(buttonsResponse.Data.ToDictionary(b => b.Id, b => b));
            contactSensorsCache = new ConcurrentDictionary<Guid, ContactSensor>(contactSensorsResponse.Data.ToDictionary(c => c.Id, c => c));
            motionSensorsCache = new ConcurrentDictionary<Guid, MotionResource>(motionsResponse.Data.ToDictionary(m => m.Id, m => m));
            zigbeeConnectivityCache = new ConcurrentDictionary<Guid, ZigbeeConnectivity>(zigbeeResponse.Data.ToDictionary(z => z.Id, z => z));
            lightLevelsCache = new ConcurrentDictionary<Guid, LightLevel>(lightLevelResponse.Data.ToDictionary(l => l.Id, l => l));
        }

        public async Task<List<DomusRoom>> GetRoomsAsync()
        {
            await InitializeCachedValuesAsync();

            List<DomusRoom> result = new List<DomusRoom>();

            foreach (Guid roomId in roomsCache.Keys)
            {
                Room room = roomsCache[roomId];
                string name = room.Metadata?.Name ?? "Unknown room name";
                List<DomusDevice> domusDevices = new List<DomusDevice>();

                foreach (ResourceIdentifier resourceIdentifier in room.Children)
                {
                    if (devicesCache.TryGetValue(resourceIdentifier.Rid, out Device? device))
                    {
                        domusDevices.Add(GetAsDomusDevice(device));
                    }
                }

                DomusRoom domusRoom = new DomusRoom(roomId, name, domusDevices);
                result.Add(domusRoom);
            }

            return result;
        }

        private DomusDevice GetAsDomusDevice(Device device)
        {
            Guid id = device.Id;
            ConnectionStatus connectionStatus = GetConnectionStatusForDevice(device);
            string? deviceName = device.Metadata?.Name;
            DomusDeviceMetadata domusDeviceMetadata = new DomusDeviceMetadata(device.ProductData.ManufacturerName, device.ProductData.ModelId, device.ProductData.ProductName, device.Metadata?.Archetype);

            if (device.Services != null)
            {
                if (IsLight(device, out HueApi.Models.Light? light))
                {
                    if (light == null)
                        throw new InvalidDataException($"Device with name {deviceName} and id {device.Id} was said to be light but the light service type was unexpectedly null");

                    if (light.Metadata != null)
                    {
                        if (domusDeviceMetadata.AssignedIcon == null)
                            domusDeviceMetadata.AssignedIcon = light.Metadata.Archetype;

                        deviceName = light.Metadata.Name;
                    }

                    DomusLight domusLight = new DomusLight(id, deviceName ?? "Unknown light", connectionStatus, domusDeviceMetadata, light.On.IsOn, light.Id);
                    return domusLight;
                }
                else
                {
                    throw new InvalidDataException($"Device with name {deviceName} and id {device.Id} is of unsupported type: {device.Type}");
                }
            }
            else
                throw new InvalidDataException($"Device with name {deviceName} and id {device.Id} is missing services object");

            throw new NotImplementedException();
        }

        private ConnectionStatus GetConnectionStatusForDevice(Device device)
        {
            ResourceIdentifier? zigbeeService = device.Services?.FirstOrDefault(x => EnumStringMapper<ResourceType>.GetEnum(x.Rtype) == ResourceType.ZigbeeConnectivity);

            if (zigbeeService == null)
                return ConnectionStatus.Unknown;

            if (zigbeeConnectivityCache.TryGetValue(zigbeeService.Rid, out ZigbeeConnectivity? connectivity))
            {
                if (connectivity.Status == ConnectivityStatus.connected)
                    return ConnectionStatus.Connected;
                else if (connectivity.Status == ConnectivityStatus.disconnected)
                    return ConnectionStatus.Disconnected;
            }

            return ConnectionStatus.Unknown;
        }

        private bool IsLight(Device device, out HueApi.Models.Light? light)
        {
            if (device.Services == null)
            {
                light = null;
                return false;
            }

            foreach (ResourceIdentifier service in device.Services)
            {
                if (EnumStringMapper<ResourceType>.GetEnum(service.Rtype) == ResourceType.Light)
                {
                    light = lightsCache[service.Rid];
                    return true;
                }
            }

            light = null;
            return false;
        }
    }
}
