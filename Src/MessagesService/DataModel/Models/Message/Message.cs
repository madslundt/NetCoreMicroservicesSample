using System;

namespace DataModel.Models.Message
{
    public class Message
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public Guid UserId { get; set; }
        public string Text { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}
