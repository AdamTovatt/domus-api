namespace DomusApi.Models.Devices
{
    public class DomusControlDevice : DomusDevice
    {
        public DateTimeOffset? LastInteractionTime { get; set; }

        public DomusControlDevice(
            Guid id,
            string name,
            ConnectionStatus connectionStatus,
            DomusDeviceMetadata metadata,
            DateTimeOffset? lastInteractionTime) : base(id, name, connectionStatus, metadata)
        {
            LastInteractionTime = lastInteractionTime;
        }

        public DomusControlDevice(
            Guid id,
            string name,
            ConnectionStatus connectionStatus,
            DomusDeviceMetadata metadata) : base(id, name, connectionStatus, metadata) { }
    }
}
