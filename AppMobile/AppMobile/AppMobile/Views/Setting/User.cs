using System;
using System.Collections.Generic;
using System.Text;

namespace AppMobile.Views.Setting
{
    class User : MyNavigationItemPage<MyUserInfoView, Models.User>
    {
        protected override void RenderCore()
        {
            Caption = "Thông tin người dùng";
            MainContent.Binding(Model);
        }
    }
}
