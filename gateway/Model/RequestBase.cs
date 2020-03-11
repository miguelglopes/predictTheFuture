using Newtonsoft.Json;

namespace Gateway.Model
{
    public class RequestBase
    {
        public string serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
