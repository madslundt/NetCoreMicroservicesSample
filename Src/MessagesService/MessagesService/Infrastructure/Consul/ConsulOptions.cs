namespace MessagesService.Infrastructure.Consul
{
    public class ConsulOptions
    {
        public object Id { get; set; }
        public string Name { get; set; }
        public string[] Tags { get; set; }
        public string Address { get; set; }
    }
}
