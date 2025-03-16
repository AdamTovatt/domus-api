using DomusApi.Helpers.Enums;
using DomusApi.Models.Devices;
using HueApi;
using HueApi.Models;
using HueApi.Models.Sensors;
using System.Collections.Concurrent;

namespace DomusApi.Helpers
{
    public class HomeHelper
    {
        private readonly LocalHueApi hueApi;
        private readonly CacheManager cacheManager = new();

        public HomeHelper(LocalHueApi hueApi)
        {
            this.hueApi = hueApi;
        }

        private async Task InitializeCachedValuesAsync()
        {
            cacheManager.AddCache(await LoadCacheAsync(hueApi.GetRoomsAsync()));
            cacheManager.AddCache(await LoadCacheAsync(hueApi.GetDevicesAsync()));
            cacheManager.AddCache(await LoadCacheAsync(hueApi.GetLightsAsync()));
            cacheManager.AddCache(await LoadCacheAsync(hueApi.GetButtonsAsync()));
            cacheManager.AddCache(await LoadCacheAsync(hueApi.GetContactSensorsAsync()));
            cacheManager.AddCache(await LoadCacheAsync(hueApi.GetMotionsAsync()));
            cacheManager.AddCache(await LoadCacheAsync(hueApi.GetZigbeeConnectivityAsync()));
            cacheManager.AddCache(await LoadCacheAsync(hueApi.GetLightLevelsAsync()));
            cacheManager.AddCache(await LoadCacheAsync(hueApi.GetTemperaturesAsync()));
        }

        private async Task<ConcurrentDictionary<Guid, T>> LoadCacheAsync<T>(Task<HueResponse<T>> apiCall) where T : HueResource
        {
            HueResponse<T> response = await apiCall;
            return new ConcurrentDictionary<Guid, T>(response.Data.ToDictionary(item => item.Id, item => item));
        }

        public ConcurrentDictionary<Guid, T>? GetCache<T>() where T : class
        {
            return cacheManager.GetCache<T>();
        }

        public async Task<List<DomusRoom>> GetRoomsAsync()
        {
            await InitializeCachedValuesAsync();

            ConcurrentDictionary<Guid, Room>? roomsCache = GetCache<Room>();
            ConcurrentDictionary<Guid, Device>? devicesCache = GetCache<Device>();

            if (roomsCache == null || devicesCache == null)
            {
                throw new InvalidOperationException("Caches have not been initialized.");
            }

            Dictionary<string, DomusRoom> rooms = new Dictionary<string, DomusRoom>();
            HashSet<Guid> processedRoomChildren = new HashSet<Guid>();

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
                        processedRoomChildren.Add(device.Id);
                    }
                }

                DomusRoom domusRoom = new DomusRoom(roomId, name, domusDevices);
                rooms.Add(domusRoom.Name, domusRoom);
            }

            foreach (Guid deviceId in devicesCache.Keys)
            {
                if (processedRoomChildren.Contains(deviceId)) continue;

                DomusDevice domusDevice = GetAsDomusDevice(devicesCache[deviceId]);
            }

            return rooms.Values.ToList();
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

                    PatchDomusDeviceMetadata(domusDeviceMetadata, ref deviceName, light);

                    DomusLight domusLight = new DomusLight(id, deviceName, connectionStatus, domusDeviceMetadata, light.On.IsOn, light.Id);
                    return domusLight;
                }
                else if (IsButtonPad(device, out List<ButtonResource>? buttonResources))
                {
                    if (buttonResources == null || buttonResources.Count == 0)
                        throw new InvalidDataException($"Device with name {deviceName} and id {device.Id} was said to be button but the button service(s) type was unexpectedly missing");

                    List<DomusSubButton> subButtons = GetSubButtons(buttonResources);

                    DomusButtonPad domusButton = new DomusButtonPad(id, deviceName, connectionStatus, domusDeviceMetadata, subButtons);
                    return domusButton;
                }
                else if (IsMotionDetector(device, out MotionResource? motionResource))
                {

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

        private List<DomusSubButton> GetSubButtons(List<ButtonResource> buttonResources)
        {
            List<DomusSubButton> result = new List<DomusSubButton>();

            foreach (ButtonResource buttonResource in buttonResources)
            {
                DomusSubButton subButton = new DomusSubButton(buttonResource.Id, buttonResource?.Button?.ButtonReport?.Updated);
                result.Add(subButton);
            }

            return result;
        }

        private void PatchDomusDeviceMetadata(DomusDeviceMetadata domusDeviceMetadata, ref string? deviceName, HueResource? resourceIdentifier)
        {
            if (resourceIdentifier == null || resourceIdentifier.Metadata == null) return;

            if (domusDeviceMetadata.AssignedIcon == null)
                domusDeviceMetadata.AssignedIcon = resourceIdentifier.Metadata.Archetype;

            deviceName = resourceIdentifier.Metadata.Name;
        }

        private ConnectionStatus GetConnectionStatusForDevice(Device device)
        {
            ResourceIdentifier? zigbeeService = device.Services?.FirstOrDefault(
                (ResourceIdentifier x) => EnumStringMapper<ResourceType>.GetEnum(x.Rtype) == ResourceType.ZigbeeConnectivity);

            if (zigbeeService == null)
            {
                return ConnectionStatus.Unknown;
            }

            ConcurrentDictionary<Guid, ZigbeeConnectivity>? zigbeeConnectivityCache = GetCache<ZigbeeConnectivity>();

            if (zigbeeConnectivityCache == null)
            {
                throw new InvalidOperationException("ZigbeeConnectivity cache has not been initialized.");
            }

            if (zigbeeConnectivityCache.TryGetValue(zigbeeService.Rid, out ZigbeeConnectivity? connectivity))
            {
                if (connectivity.Status == ConnectivityStatus.connected)
                {
                    return ConnectionStatus.Connected;
                }
                else if (connectivity.Status == ConnectivityStatus.disconnected)
                {
                    return ConnectionStatus.Disconnected;
                }
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

            ConcurrentDictionary<Guid, HueApi.Models.Light>? lightsCache = GetCache<HueApi.Models.Light>();

            if (lightsCache == null)
            {
                throw new InvalidOperationException("Lights cache has not been initialized.");
            }

            foreach (ResourceIdentifier service in device.Services)
            {
                if (EnumStringMapper<ResourceType>.GetEnum(service.Rtype) == ResourceType.Light)
                {
                    if (lightsCache.TryGetValue(service.Rid, out HueApi.Models.Light? foundLight))
                    {
                        light = foundLight;
                        return true;
                    }
                }
            }

            light = null;
            return false;
        }

        private bool IsButtonPad(Device device, out List<ButtonResource>? buttonResources)
        {
            buttonResources = GetAllResourcesOfType<ButtonResource>(device.Services);

            return buttonResources.Count > 0;
        }

        private bool IsMotionDetector(Device device, out MotionResource? motionResource)
        {
            motionResource = GetFirstResourceOfType<MotionResource>(device.Services);

            return motionResource != null;
        }

        private List<T> GetAllResourcesOfType<T>(List<ResourceIdentifier>? identifiers) where T : HueResource
        {
            List<T> result = new List<T>();

            if (identifiers == null)
                return result;

            ResourceType? expectedResourceType = EnumTypeMapper<ResourceType>.GetEnumFromType(typeof(T));

            if (expectedResourceType == null)
                throw new NotImplementedException($"Getting all resources of type {typeof(T).Name} is not supported.");

            ConcurrentDictionary<Guid, T>? resourceCache = GetCache<T>();

            if (resourceCache == null)
                throw new InvalidOperationException($"Cache for {typeof(T).Name} has not been initialized.");

            foreach (ResourceIdentifier resource in identifiers)
            {
                if (EnumStringMapper<ResourceType>.GetEnum(resource.Rtype) == expectedResourceType)
                {
                    if (resourceCache.TryGetValue(resource.Rid, out T? resourceItem))
                    {
                        result.Add(resourceItem);
                    }
                }
            }

            return result;
        }

        private T? GetFirstResourceOfType<T>(List<ResourceIdentifier>? identifiers) where T : HueResource
        {
            if (identifiers == null || identifiers.Count == 0)
                return null;

            ResourceType? expectedResourceType = EnumTypeMapper<ResourceType>.GetEnumFromType(typeof(T));

            if (expectedResourceType == null)
                throw new NotImplementedException($"Getting first resource of type {typeof(T).Name} is not supported.");

            ConcurrentDictionary<Guid, T>? resourceCache = GetCache<T>();

            if (resourceCache == null)
                throw new InvalidOperationException($"Cache for {typeof(T).Name} has not been initialized.");

            foreach (ResourceIdentifier resource in identifiers)
                if (EnumStringMapper<ResourceType>.GetEnum(resource.Rtype) == expectedResourceType)
                    if (resourceCache.TryGetValue(resource.Rid, out T? resourceItem))
                        return resourceItem;

            return null;
        }
    }
}
