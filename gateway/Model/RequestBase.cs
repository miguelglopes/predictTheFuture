using Newtonsoft.Json;

namespace gateway.Model
{
    public abstract class RequestBase
    {
        public string serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
