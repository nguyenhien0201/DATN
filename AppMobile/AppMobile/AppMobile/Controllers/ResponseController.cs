using System;
using System.Collections.Generic;
using System.Text;
using System.Mvc;
using Models;
using Xamarin.Forms;

namespace AppMobile.Controllers
{
    class ResponseController : BaseController
    {
        public void Default()
        {
            var code = Response.GetString("Code");
            if(code == null)
            {
                var url = Response.Pop<string>("#url");

                var action = GetMethod(url);
                action?.Invoke(this, new object[] { });
            }
            else
            {
                if (Int32.Parse(code) < 0)
                {
                    Toast("Error!");
                }
                else
                {
                    var url = Response.Pop<string>("#url");

                    var action = GetMethod(url);
                    action?.Invoke(this, new object[] { });
                }
            }
        }

        public void account_login()
        {
            DataContext v = Json.Convert<DataContext>(Response["Value"]);
            Current_User = Json.Convert<Models.User>(v);

            var UserTopic = "Warning/" + Current_User.Token;
            Subscribe(UserTopic);

            App.Current.Properties["user"] = v;

            LogAsRole();
        }

        public void account_changepassword()
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
            {
                await App.Current.MainPage.DisplayAlert("Success!", "Your password has been changed.", "OK");
            });
        }

        public void account_logout()
        {
            Current_User.Token = null;
            App.Current.Properties.Clear();
            App.Execute("home");
        }

        public void user_info()
        {
            DataContext v = Json.Convert<DataContext>(Response["Value"]);
            var u = Json.Convert<Models.User>(v);
            Device.BeginInvokeOnMainThread(() =>
            {
                App.Execute("setting/user", u);
            });
        }

        public void patient_indexlist()
        {
            var code = Response.GetString("Code");

            var v = Response["Value"];
            v = ConvertArray<List<Index>>(v);

            Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                App.Execute("patient/detail", v);
            });

        }

        public void doctor_patientlist()
        {
            var v = Response["Value"];
            v = ConvertArray<List<Patient>>(v);

            Device.BeginInvokeOnMainThread(() => {
                App.Execute("patient", v);
            });
        }

        public void warning()
        {
            Response.Pop<string>("#cliend-id");

            var p = Json.Convert<Models.Patient>(Response);

            Notification("Cảnh báo!", String.Format("{0} có bất thường.", p.Name));
        }

        public void patient_getpatient()
        {
            var v = Response["Value"];
            v = ConvertArray<List<Patient>>(v);

            Device.BeginInvokeOnMainThread(() => {
                App.Execute("patient", v);
            });
        }
    }
}

namespace AppMobile
{
    public interface ICustomNotification
    {
        void Send(string title, string message);
    }

    public interface ICustomToast
    {
        void Show(string message);
    }
}
