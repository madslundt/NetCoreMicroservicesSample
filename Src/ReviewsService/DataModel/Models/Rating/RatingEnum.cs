using System.ComponentModel;

namespace DataModel.Models.Rating
{
    public enum RatingEnum
    {
        [Description("Very bad")]
        VeryBad = 1,

        [Description("Bad")]
        Bad = 2,

        [Description("Okay")]
        Okay = 3,

        [Description("Good")]
        Good = 4,

        [Description("Very good")]
        VeryGood = 5
    }
}
