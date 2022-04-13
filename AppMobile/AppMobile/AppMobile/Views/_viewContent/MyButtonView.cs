using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Xamarin.Forms
{
    public class MyButton : Button
    {
        public MyButton() : base()
        {
            this.TextColor = Color.White;
            this.HorizontalOptions = LayoutOptions.FillAndExpand;
            this.VerticalOptions = LayoutOptions.Center;
            this.FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Button));
            this.BackgroundColor = Color.FromHex("#2C50CA");
            this.Margin = new Thickness(40, 25);
        }
        public MyButton(string text) : this()
        {
            this.Text = text;
        }
        public MyButton(string text, Color color) : this(text)
        {
            this.TextColor = color;
        }
    }
}
