using System;
using System.Collections.Generic;
using System.Text;

namespace MQTT
{
    public class PublishContext
    {
        public string UrlAction { get; set; }
        public object Value { get; set; }

        public PublishContext() { }
        public PublishContext(string UrlAction, object value)
        {
            this.UrlAction = UrlAction;
            this.Value = value;
        }
    }
}
