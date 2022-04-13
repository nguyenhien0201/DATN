using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Mvc;

namespace AppMobile.Views.Home
{
    class Login : BaseView<LoginForm, Models.LoginInfo>
    {
        protected override void RenderCore()
        {
            MainContent.BindingContext = Model;
        }
    }
}
