using System;
using System.Collections.Generic;
using System.Text;
using System.Mvc;

namespace AppMobile.Controllers
{
    class HomeController : BaseController
    {
        public void Default()
        {
            if (Client.IsConnected) { }
            if (App.Current.Properties.ContainsKey("user"))
            {
                var v = App.Current.Properties["user"];
                Current_User = Json.Convert<Models.User>(v);

                LogAsRole();
            }
            else
            {
                App.Execute("home/login");
            }
        }

        public void Home()
        {
            LogAsRole();
        }

        public object Login()
        {
            return View(new Models.LoginInfo { UserName = "Doctor", Password = "Doctor" });
        }
        public void Login_Clicked(Models.LoginInfo i)
        {
            Publish("account/login", i);
        }
        public void Logout_Clicked()
        {
            Publish("account/logout", new object { });
        }
        public object TestMqtt(Models.Patient p)
        {
            return View(new Models.LoginInfo { UserName = p.Name });
        }
    }
}

