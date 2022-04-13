using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Mvc;
using Models;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace AppMobile.Controllers
{
    class BaseController : Controller
    {
        //for ResponseController 
        public DataContext Response { get; set; }
        const string _topic = "do-an-tot-nghiep/20211/20172538";
        public string PrivateTopic => _topic + '/' + ClientId;

        public static User Current_User { get; set; }

        static string _clientId;
        public static string ClientId
        {
            get
            {
                if (_clientId == null)
                {
                    _clientId = Guid.NewGuid().ToString();
                }
                return _clientId;
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

            var c = Engine.GetController<BaseController>("Response");
            if (c != null)
            {
                var action = c.GetMethod("Default");
                if (action != null)
                {
                    c.Response = context;
                    AsyncEngine.CreateThread(() => action.Invoke(c, new object[] { }));
                }
            }
        }
        protected void ConnectMqtt(int checkConnectionSeconds = 0)
        {
            if (_mqttClient != null && _mqttClient.IsConnected) return;
            _mqttClient.MqttMsgPublishReceived += MqttMsgReceived;

            _mqttClient.Connect(ClientId);
            _mqttClient.ConnectionClosed += (s, e) =>
            {

            };

            if (_mqttClient.IsConnected)
            {
                Subscribe(PrivateTopic);
                //Subscribe(_topic);
            }

            if (checkConnectionSeconds > 0)
            {
                int interval = checkConnectionSeconds * 1000;
                AsyncEngine.CreateThread(() =>
                {
                    while (true)
                    {
                        System.Threading.Thread.Sleep(interval);
                        ConnectMqtt();
                    }
                });
            }
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
                context.SetString("#client-id", ClientId);

                if (Current_User != null && string.IsNullOrEmpty(Current_User.Token) == false)
                {
                    context.SetString("#token", Current_User.Token);
                }

                _mqttClient.Publish(topic, GetEncodeBytes(context));
            }
        }
        protected void Publish(string url, object value)
        {
            Publish(_topic, url, value);
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
        public void Message(string text)
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
            {
                await App.Current.MainPage.DisplayAlert("Success", "Your password has been changed.", "OK");
            });
        }
        public void Toast(string message) {
            ICustomToast toast = DependencyService.Get<ICustomToast>();
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                toast.Show(message);
            });
        }
        public void Notification(string title, string message)
        {
            ICustomNotification notification = DependencyService.Get<ICustomNotification>();
            
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                notification.Send(title, message);
            });
        }

        protected T GetValue<T>(object value) { return Json.Convert<T>(value); }
        protected T ConvertArray<T>(object value)
        {
            var jarray = JArray.FromObject(value);
            return jarray.ToObject<T>();
        }

        public void LogAsRole()
        {
            if (Current_User.AuthorName == "Admin")
                App.Execute("admin");
            else if (Current_User.AuthorName == "Doctor")
                App.Execute("doctor");
            else if (Current_User.AuthorName == "Patient")
                App.Execute("patient");
        }
    }
}
