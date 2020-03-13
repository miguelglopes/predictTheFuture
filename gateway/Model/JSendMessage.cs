using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Gateway.Model
{
    public class JSendMessage
    {
        [JsonProperty(PropertyName = "data", Order = 3)]
        public object Data { get; set; }

        [JsonProperty(PropertyName = "message", Order = 2)]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "status", Order = 1)]
        public Status Status { get; set; }

        [JsonIgnore] //ignore when serializing
        public string MessageID { get; set; }

        public JSendMessage(Status status, object data = null, string message = null)
        {
            this.Status = status;
            this.Data = data;
            this.Message = message;
        }

        public static JSendMessage GetFailMessage(String message)
        {
            JSendMessage m = new JSendMessage(status: Status.fail);
            JObject o = new JObject();
            o["message"] = message;
            m.Data = o;
            return m;
        }

        public static JSendMessage GetErrorMessage(String message)
        {
            return new JSendMessage(status: Status.error, message: message);
        }

        public bool IsSuccess()
        {
            if (Status.Equals(Status.success))
                return true;
            return false;
        }

        public bool IsFail()
        {
            if (Status.Equals(Status.fail))
                return true;
            return false;
        }

        public bool IsError()
        {
            if (Status.Equals(Status.error))
                return true;
            return false;
        }

        public string Serialize()
        {
            //ignore null values
            JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            return JsonConvert.SerializeObject(this, Formatting.None, settings);

        }
    }

    [JsonConverter(typeof(StringEnumConverter))]

    public enum Status
    {
        [EnumMember(Value = "success")]
        success,
        [EnumMember(Value = "fail")]
        fail,
        [EnumMember(Value = "error")]
        error
    }
}
