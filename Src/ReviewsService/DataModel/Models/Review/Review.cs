using DataModel.Models.Rating;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataModel.Models.Review
{
    public class Review
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }
        public Guid MovieId { get; set; }

        public string Text { get; set; }

        public RatingRef RatingRef { get; set; }
        public RatingEnum Rating { get; set; }

        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    }
}
