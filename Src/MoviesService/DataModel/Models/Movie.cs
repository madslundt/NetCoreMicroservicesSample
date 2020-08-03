using System;
using System.Collections.Generic;
using System.Text;

namespace DataModel.Models
{
    public class Movie
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Title { get; set; }
        public int Year { get; set; }
        public Guid UserId { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}
