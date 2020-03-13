using Gateway.Model;
using System.Collections.Concurrent;
using System.Threading.Tasks;


//TODO DOCUMENTATION


namespace Gateway.Messaging
{
    public class MessageRepository : ConcurrentDictionary<string, TaskCompletionSource<JSendMessage>>
    {


    }
}