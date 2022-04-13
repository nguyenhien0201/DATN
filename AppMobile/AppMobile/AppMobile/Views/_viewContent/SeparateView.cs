using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Xamarin.Forms
{
    public class Separate : Grid
    {
        public Separate() : base()
        {
            BackgroundColor = Color.WhiteSmoke;
        }
        public Separate(int height, string bottom)
            :this()
        {
            HeightRequest = height;
            if (bottom != null)
            {
                this.Children.Add(new Label
                {
                    Text = bottom,
                    TextColor = Color.Gray,
                    VerticalOptions = LayoutOptions.End,
                    FontSize = 14,
                    Margin = new Thickness(20, 5, 5, 5)
                });
            }
        }
        public Separate(int height, string top, string bottom) 
            :this(height, bottom)
        {
            //this.Separate(height, bottom);
            if (top != null)
            {
                this.Children.Add(new Label
                {
                    Text = top,
                    TextColor = Color.Gray,
                    VerticalOptions = LayoutOptions.Start,
                    FontSize = 14,
                    Margin = new Thickness(20, 5, 5, 5),
                });
            }
        }
    }

}
