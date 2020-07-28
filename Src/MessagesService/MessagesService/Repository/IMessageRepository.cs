using DataModel.Models.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MessagesService.Repository
{
    public interface IMessageRepository
    {
        Task CreateMessage(Message message, CancellationToken cancellationToken);
    }
}
