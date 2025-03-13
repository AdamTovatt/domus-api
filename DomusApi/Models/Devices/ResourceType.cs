using System.Runtime.Serialization;

namespace DomusApi.Models.Devices
{
    public enum ResourceType
    {
        [EnumMember(Value = "device")]
        Device,

        [EnumMember(Value = "bridge_home")]
        BridgeHome,

        [EnumMember(Value = "room")]
        Room,

        [EnumMember(Value = "zone")]
        Zone,

        [EnumMember(Value = "service_group")]
        ServiceGroup,

        [EnumMember(Value = "light")]
        Light,

        [EnumMember(Value = "button")]
        Button,

        [EnumMember(Value = "relative_rotary")]
        RelativeRotary,

        [EnumMember(Value = "temperature")]
        Temperature,

        [EnumMember(Value = "light_level")]
        LightLevel,

        [EnumMember(Value = "motion")]
        Motion,

        [EnumMember(Value = "camera_motion")]
        CameraMotion,

        [EnumMember(Value = "entertainment")]
        Entertainment,

        [EnumMember(Value = "contact")]
        Contact,

        [EnumMember(Value = "tamper")]
        Tamper,

        [EnumMember(Value = "grouped_light")]
        GroupedLight,

        [EnumMember(Value = "grouped_motion")]
        GroupedMotion,

        [EnumMember(Value = "grouped_light_level")]
        GroupedLightLevel,

        [EnumMember(Value = "device_power")]
        DevicePower,

        [EnumMember(Value = "device_software_update")]
        DeviceSoftwareUpdate,

        [EnumMember(Value = "zigbee_connectivity")]
        ZigbeeConnectivity,

        [EnumMember(Value = "zgp_connectivity")]
        ZgpConnectivity,

        [EnumMember(Value = "bridge")]
        Bridge,

        [EnumMember(Value = "zigbee_device_discovery")]
        ZigbeeDeviceDiscovery,

        [EnumMember(Value = "homekit")]
        Homekit,

        [EnumMember(Value = "matter")]
        Matter,

        [EnumMember(Value = "matter_fabric")]
        MatterFabric,

        [EnumMember(Value = "scene")]
        Scene,

        [EnumMember(Value = "entertainment_configuration")]
        EntertainmentConfiguration,

        [EnumMember(Value = "public_image")]
        PublicImage,

        [EnumMember(Value = "auth_v1")]
        AuthV1,

        [EnumMember(Value = "behavior_script")]
        BehaviorScript,

        [EnumMember(Value = "behavior_instance")]
        BehaviorInstance,

        [EnumMember(Value = "geofence_client")]
        GeofenceClient,

        [EnumMember(Value = "geolocation")]
        Geolocation,

        [EnumMember(Value = "smart_scene")]
        SmartScene
    }
}
