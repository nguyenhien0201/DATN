using System;
namespace Xamarin.Forms
{
    public class MyImage : Image
    {
        static public string GetImageFilePath(string name)
        {
            var imgFileName = name + ".png";
            if (Device.RuntimePlatform == Device.iOS)
            {
                imgFileName = "Images/" + imgFileName;
            }
            return imgFileName;
        }
        public MyImage()
        {
            HorizontalOptions = LayoutOptions.Center;
            VerticalOptions = LayoutOptions.Center;
        }
        public MyImage(string name)
            : this()
        {
            this.Source = ImageSource.FromFile(GetImageFilePath(name));
        }
        public void SetSouce(string name)
        {
            this.Source = ImageSource.FromFile(GetImageFilePath(name));
        }
        public double SquareSize
        {
            get { return this.Width; }
            set { this.WidthRequest = this.HeightRequest = value; }
        }
    }
}
