using System.ComponentModel;

namespace DataModel.Models.UserStatus
{
    public enum UserStatusEnum
    {
        [Description("Waiting for confirmation")]
        WaitingConfirmation = 0,

        [Description("Active")]
        Active = 1,

        [Description("Deactive")]
        Deactive = 2
    }
}
