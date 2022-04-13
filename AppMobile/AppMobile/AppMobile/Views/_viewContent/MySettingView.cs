using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace AppMobile.Views
{
    public class MySettingView : MyMenuView
    {
        public MySettingView() : base()
        {
            this.Separate(50, null, "Tài khoản");
            //tai khoan
            var acounts = new List<MyMenuItemInfo> {

                new MyMenuItemInfo{IconName = "user", Text = "Thông tin cá nhân",Url = "setting/user",  },
                new MyMenuItemInfo{IconName = "verified", Text = "Đổi mật khẩu",Url = "setting/account", },

            };
            this.AddGroup(acounts);
            this.Separate(50, null, "Tổng quan");
            //tong quan
            var sysSets = new List<MyMenuItemInfo>
            {
                new MyMenuItemInfo{IconName = "message", Text = "Phản hồi",Url = "setting/feedback" },
                new MyMenuItemInfo{IconName = "info", Text = "Giới thiệu",Url = "setting/info" },
            };
            this.AddGroup(sysSets);
            this.Separate(50, null, null);
            //button dang xuat
            var button = new MyButton("Đăng xuất");
            button.Clicked += (s, e) =>
            {
                App.Execute("home/Logout_Clicked");
            };

            this.Children.Add(button);
        }
    }
}
