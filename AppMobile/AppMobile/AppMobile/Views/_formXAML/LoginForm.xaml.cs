using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppMobile.Views.Home
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoginForm : ContentView
	{
		public LoginForm ()
		{
			InitializeComponent ();
            
		}

        private void Login_Clicked(object sender, EventArgs e)
        {
            App.Execute("home/Login_Clicked", MainContent.BindingContext);
        }
    }
}