
namespace DomusApi.Models.Devices
{
    public class DomusButtonPad : DomusControlDevice
    {
        public List<DomusSubButton> Buttons { get; set; }

        public DomusButtonPad(
            Guid id,
            string? name,
            ConnectionStatus connectionStatus,
            DomusDeviceMetadata metadata,
            List<DomusSubButton> buttons) : base(id, name ?? "Unknown button pad", connectionStatus, metadata)
        {
            Buttons = buttons;
        }
    }
}
