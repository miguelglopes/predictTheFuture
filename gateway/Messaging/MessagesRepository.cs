using Gateway.Model;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Gateway.Messaging
{
    public class MessagesRepository : ConcurrentDictionary<string, TaskCompletionSource<ResponseMessage>>
    {


    }
}