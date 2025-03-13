namespace DomusApi.Models.Devices
{
    public class DomusControlDevice : DomusDevice
    {
        public DateTime? LastInteractionTime { get; set; }

        public DomusControlDevice(
            Guid id,
            string name,
            ConnectionStatus connectionStatus,
            DomusDeviceMetadata metadata,
            DateTime? lastInteractionTime) : base(id, name, connectionStatus, metadata)
        {
            LastInteractionTime = lastInteractionTime;
        }
    }
}
