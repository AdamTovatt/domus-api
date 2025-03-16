
namespace DomusApi.Models.Devices
{
    public class DomusLight : DomusDevice
    {
        public bool IsOn { get; set; }
        public Guid LightServiceId { get; set; }

        public DomusLight(
            Guid id,
            string? name,
            ConnectionStatus connectionStatus,
            DomusDeviceMetadata metadata,
            bool isOn,
            Guid lightServiceId) : base(id, name ?? "Unknown light", connectionStatus, metadata)
        {
            IsOn = isOn;
            LightServiceId = lightServiceId;
        }
    }
}
