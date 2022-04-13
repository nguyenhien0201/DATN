using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AppMobile.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(AppMobile.Droid.AndroidToast))]

namespace AppMobile.Droid
{
    public class AndroidToast : ICustomToast
    {
        public void Show(string message)
        {
            Toast t = Toast.MakeText(Android.App.Application.Context, message, ToastLength.Short);
            Color c = Color.Rgb(64, 64, 64);
            ColorMatrixColorFilter CM = new ColorMatrixColorFilter(new float[]
                {
                    0,0,0,0,c.R,
                    0,0,0,0,c.G,
                    0,0,0,0,c.B,
                    0,0,0,1,0
                });
            t.View.Background.SetColorFilter(CM);
            t.View.FindViewById<TextView>(Android.Resource.Id.Message).SetTextColor(Color.White);
            t.SetGravity(GravityFlags.Center | GravityFlags.Center, 0, 0);
            t.Show();
        }
    }
}