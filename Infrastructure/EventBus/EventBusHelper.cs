namespace Infrastructure.EventBus
{
    public static class EventBusHelper
    {
        public static string GetTypeName<T>()
        {
            var name = typeof(T).FullName.ToLower().Replace("+", ".");

            if (typeof(T) is IEvent)
            {
                name += "_event";
            }
            else if (typeof(T) is ICommand)
            {
                name += "_command";
            }

            return name;
        }
    }
}
