
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Gateway.Model
{

    public class FitRequest : RequestBase
    {
        [Required]
        [JsonProperty(PropertyName = "data")]
        public double[] Data { get; set; }

    }
}
