using System;

namespace DataModel.Models
{
    public class Movie
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Title { get; set; }
        public int Year { get; set; }
        public Guid UserId { get; set; }

        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    }
}
