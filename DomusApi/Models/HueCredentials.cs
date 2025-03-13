namespace DomusApi.Models
{
    public class HueCredentials
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string KeyValue { get; set; }
        public string IpAddress { get; set; }

        public HueCredentials(int id, string keyValue, string ipAddress, string username)
        {
            Id = id;
            Username = username;
            KeyValue = keyValue;
            IpAddress = ipAddress;
        }
    }
}
