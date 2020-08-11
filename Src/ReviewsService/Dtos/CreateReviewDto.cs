using DataModel.Models.Rating;
using System;

namespace ReviewsService.Dtos
{
    public class CreateReviewDto
    {
        public Guid MovieId { get; set; }
        public string Text { get; set; }
        public RatingEnum Rating { get; set; }
    }
}
