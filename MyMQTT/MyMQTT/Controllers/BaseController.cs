using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Mvc;

using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MyMQTT.Controllers
{
    class BaseController : Controller
    {
        //for ResponseController 
        public DataContext Request { get; set; }
        public const string _topic = "do-an-tot-nghiep/20211/20172538";
        public string ClientId { get; set; }
        public string Url { get; set; }

        static string _serverId;
        public static string ServerId
        {
            get
            {
                if (_serverId == null)
                {
                    _serverId = Guid.NewGuid().ToString();
                }
                return _serverId;
            }
        }
        static MqttClient _mqttClient;
        public MqttClient Client
        {
            get
            {
                if (_mqttClient == null)
                {
                    _mqttClient = new MqttClient(
                    "broker.emqx.io",
                    1883,
                    false,
                    MqttSslProtocols.None,
                    null,
                    null
                );

                    ConnectMqtt(5);
                }
                return _mqttClient;
            }
        }
        //
        T GetMqttMessage<T>(MqttMsgPublishEventArgs e)
        {
            string content = System.Text.Encoding.UTF8.GetString(e.Message);
            var context = Newtonsoft.Json.Linq.JObject
                .Parse(content)
                .ToObject<T>();
            return context;
        }
        byte[] GetEncodeBytes(object v)
        {
            var content = Newtonsoft.Json.Linq.JObject.FromObject(v).ToString();
            return System.Text.Encoding.UTF8.GetBytes(content);
        }
        void MqttMsgReceived(object sender, MqttMsgPublishEventArgs e)
        {
            var context = GetMqttMessage<DataContext>(e);

            var request = new RequestContext(context.GetString("#url"));

            var c = Engine.GetController<BaseController>(request.ControllerName);
            c.ClientId = context.GetString("#client-id");
            c.Url = context.GetString("#url");
            c.Url = c.Url.ToLower().Replace('/', '_');
            Screen.Write(String.Format("\n>>>\tcid: {0} \n\turl: {1} \n\ttime: {2}\n", c.ClientId, c.Url, DateTime.Now.ToString()));
            if (c != null)
            {
                if (request.ActionName == null)
                {
                    request.ActionName = "Default";
                }
                var action = c.GetMethod(request.ActionName);
                if (action != null)
                {
                    c.Request = context;
                    AsyncEngine.CreateThread(() => action.Invoke(c, new object[] { }));
                }
            }
        }
        protected void ConnectMqtt(int checkConnectionSeconds = 0)
        {
            if (_mqttClient != null && _mqttClient.IsConnected) return;
            _mqttClient.MqttMsgPublishReceived += MqttMsgReceived;

            //string topic = MQTT.ResponseContext.DefaultTopic + _clientId;
            _mqttClient.Connect(ServerId);
            _mqttClient.ConnectionClosed += (s, e) =>
            {
                Screen.Warning("Connection closed");
            };

            if (_mqttClient.IsConnected)
            {
                Screen.Write("done\n");
                //Subscribe(PrivateTopic);
                Subscribe(_topic);
                //Publish("account/login", new Models.LoginInfo { UserName = "Admin", Password = "Admin" });
            }

            //if (checkConnectionSeconds > 0)
            //{
            //    int interval = checkConnectionSeconds * 1000;
            //    AsyncEngine.CreateThread(() =>
            //    {
            //        while (true)
            //        {
            //            System.Threading.Thread.Sleep(interval);
            //            ConnectMqtt();
            //        }
            //    });
            //}
        }

        protected void Subscribe(string topic)
        {
            _mqttClient.Subscribe(new string[] { topic }, new byte[] { 0 });
        }
        protected void Publish(string topic, string url, object value)
        {
            if (_mqttClient == null || _mqttClient.IsConnected == false)
            {
                ConnectMqtt();
            }
            if (_mqttClient.IsConnected)
            {
                var context = value == null ? new DataContext() : DataContext.FromObject(value);
                context.SetString("#url", url);
                _mqttClient.Publish(topic, GetEncodeBytes(context));
            }
        }
        protected void Publish(string url, object value)
        {
            Publish(_topic, url, value);
        }
        public void Response(object res)
        {
            Publish(_topic + '/' + ClientId, Url, res);
        }
        protected void Disconnect()
        {
            if (_mqttClient != null && _mqttClient.IsConnected)
            {
                _mqttClient.Disconnect();
            }
        }

        public object GoFirst()
        {
            return RedirectToAction("Default");
        }
        public object GoHome()
        {
            return Redirect("Home");
        }

        protected virtual object Excute(object requestContext)
        {
            var context = DataContext.FromObject(requestContext);
            var url = context.Pop<string>("#url");
            if (url == null) return null;

            var s = url.Split('/');
            var token = context.GetString("#token");

            var actor = new Models.User();
            if (!string.IsNullOrEmpty(token))
            {
                actor = Models.DB.UserCollection[token] as Models.User;
                if (actor == null) return null;
            }

            var actionName = s[1].ToLower();
            var type = Models.DB.UserCollection.GetActor(actor.AuthorName);
            var method = type.GetMethod(actionName);
            
            if (method != null)
                {
                    var res = (DataContext)method.Invoke(actor, new object[] { actor });
                    if (res == null) return null;

                    res.Push("#url", url.ToLower().Replace('/', '_'));
                    return res;
                }

            return null;
        }
    }
}
