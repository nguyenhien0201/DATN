using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppMobile.Views.Setting
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountForm : ContentView
    {
        public AccountForm()
        {
            InitializeComponent();
            confirm.TextChanged += ConfirmPass;
            new_password.TextChanged += ConfirmPass;
        }

        private void ConfirmPass(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(confirm.Text))
            {
                compare.TextColor = Color.White;
            }
            else if (new_password.Text != confirm.Text)
            {
                compare.Text = "Mật khẩu không khớp!";
                compare.TextColor = Color.Red;
            }
            else
            {
                compare.Text = "OK!";
                compare.TextColor = Color.Green;
            }
        }

        private void Change_Password_Clicked(object sender, EventArgs e)
        {
            if (compare.Text == "OK!")
            {
                App.Execute("setting/ChangePassword", MainContent.BindingContext);
            }
        }
    }
}