namespace DataModel.Models.Rating
{
    public class RatingRef
    {
        public RatingEnum Id { get; }

        public string Name { get; }

        public RatingRef()
        { }

        public RatingRef(RatingEnum rating)
        {
            Id = rating;
            Name = rating.GetDescription();
        }
    }
}
    