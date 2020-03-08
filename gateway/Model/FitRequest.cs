using Newtonsoft.Json;

namespace gateway.Model
{
    public class FitRequest
    {
        public int id { get; set; }
        public double[] data { get; set; }

        public string serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
