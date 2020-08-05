namespace DataModel.Models.UserStatus
{
    public class UserStatusRef
    {
        public UserStatusEnum Id { get; }

        public string Name { get; }

        public UserStatusRef()
        {}

        public UserStatusRef(UserStatusEnum userStatus)
        {
            Id = userStatus;
            Name = userStatus.GetDescription();
        }
    }
}
