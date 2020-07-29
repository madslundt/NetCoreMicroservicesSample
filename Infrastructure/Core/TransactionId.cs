using System;

namespace Infrastructure.Core
{
    public class TransactionId
    {
        public virtual Guid Value { get; } = Guid.NewGuid();
    }
}
