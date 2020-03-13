using Newtonsoft.Json;

namespace Gateway.Model
{
    public class RequestBase
    {
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
