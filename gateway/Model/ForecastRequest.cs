
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Gateway.Model
{
    public class ForecastRequest : RequestBase
    {
        //needs to be nullable, otherwise assumes 0 when not present
        [Required]
        [JsonProperty(PropertyName = "num_steps")]
        public Nullable<int> Num_steps { get; set; }

    }
}
