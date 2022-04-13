using System;
using System.Collections.Generic;
using System.Text;

namespace AppMobile.Views.Setting
{
    class Account: MyNavigationItemPage<AccountForm, Models.LoginInfo>
    {
        protected override void RenderCore()
        {
            Caption = "Đổi mật khẩu";
            MainContent.BindingContext = Model;
        }
    }
}
