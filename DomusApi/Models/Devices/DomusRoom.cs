namespace DomusApi.Models.Devices
{
    public class DomusRoom
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<DomusDevice> Devices { get; set; }

        public DomusRoom(Guid id, string name, List<DomusDevice> devices)
        {
            Id = id;
            Name = name;
            Devices = devices;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
