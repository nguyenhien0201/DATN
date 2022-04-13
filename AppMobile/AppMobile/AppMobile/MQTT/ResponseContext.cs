using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace MQTT
{
    public class ResponseContext
    {
        public const string DefaultTopic = "response/default/";
        public string Action { get; set; }
        public object Value { get; set; }

        public int Code { get; set; }
        public string Message { get; set; }

        public JObject ToJson()
        {
            var json = new JObject();
            if (Value != null)
            {
                json.Add("Value", JToken.FromObject(Value));
            }
            if (Action != null)
            {
                json.Add("Action", Action);
            }
            if (Message != null)
            {
                json.Add("Message", Message);
            }
            if (Code != 0)
            {
                json.Add("Code", Code);
            }
            return json;
        }
    }
}

