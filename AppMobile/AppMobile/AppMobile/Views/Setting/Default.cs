using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace AppMobile.Views.Setting
{
    class Default : MyNavigationRootPage<MySettingView, object>
    {
        protected override void RenderCore()
        {
            Caption = "Cài đặt";
        }
    }

    
}
