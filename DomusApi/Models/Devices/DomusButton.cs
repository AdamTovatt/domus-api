
namespace DomusApi.Models.Devices
{
    public class DomusButton : DomusControlDevice
    {
        public string? LastInteractionEventName { get; set; }

        public DomusButton(
            Guid id,
            string name,
            ConnectionStatus connectionStatus,
            DomusDeviceMetadata metadata,
            DateTime? lastInteractionTime,
            string? lastInteractionEventName) : base(id, name, connectionStatus, metadata, lastInteractionTime)
        {
        }
    }
}
