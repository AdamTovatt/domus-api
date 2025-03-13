namespace DomusApi.Models.Devices
{
    public class DomusDeviceMetadata
    {
        public string? ManufacturerName { get; set; }
        public string? ModelName { get; set; }
        public string? ProductName { get; set; }
        public string? AssignedIcon { get; set; }

        public DomusDeviceMetadata(string? manufacturerName, string? modelName, string? productName, string? assignedIcon)
        {
            ManufacturerName = manufacturerName;
            ModelName = modelName;
            ProductName = productName;
            AssignedIcon = assignedIcon;
        }
    }
}
