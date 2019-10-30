using System;

namespace DataModel
{
    public class Message
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Text { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}
