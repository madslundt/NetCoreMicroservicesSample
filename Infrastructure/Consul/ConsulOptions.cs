namespace Infrastructure.Consul
{
    public class ConsulOptions
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string[] Tags { get; set; }
        public string Address { get; set; }
    }
}
