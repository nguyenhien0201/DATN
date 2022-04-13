using System;
using System.Collections.Generic;
using System.Text;

namespace AppMobile.Controllers
{
    class SettingController : BaseController
    {
        public object Default()
        {
            return View();
        }

        public void User()
        {
            Publish("user/Info", new object { });
        }

        public object User(Models.User u)
        {
            return View(u);
        }

        public object Account()
        {
            return View(new Models.LoginInfo { });
        }

        public void ChangePassword(Models.LoginInfo i)
        {
            i.UserName = Current_User.UserName;
            Publish("account/ChangePassword", i);
            //Message("Your password has been changed.");
        }

        public void Noti(string text)
        {
            Message("Your password has been changed.");
        }
    }
}
