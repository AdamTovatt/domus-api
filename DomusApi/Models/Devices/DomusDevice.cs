namespace DomusApi.Models.Devices
{
    public class DomusDevice
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ConnectionStatus ConnectionStatus { get; set; }
        public DomusDeviceMetadata Metadata { get; set; }

        public DomusDevice(Guid id, string name, ConnectionStatus connectionStatus, DomusDeviceMetadata metadata)
        {
            Id = id;
            Name = name;
            ConnectionStatus = connectionStatus;
            Metadata = metadata;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
