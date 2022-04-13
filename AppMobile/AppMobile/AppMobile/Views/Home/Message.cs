using System;
using System.Mvc;
using Xamarin.Forms;

namespace AppMobile.Views
{
    public class Message : BaseView<StackLayout, object>
    {
        public static void Show(string text)
        {
            App.Current.MainPage.DisplayAlert("AKS", text, "OK");
        }
        public static async void Ask(string text, string accept, string cancel, Action callback)
        {
            if (await App.Current.MainPage.DisplayAlert("AKS", text, accept, cancel))
            {
                callback?.Invoke();
            }
        }

        //cancel create page
        protected override void RenderCore()
        {
            try
            {
                Show(Model.ToString());
            }
            catch (Exception e)
            {

            }
            
        }
        protected override StackLayout CreatMainContent()
        {
            return null;
        }
        protected override Page CreatePage()
        {
            return null;
        }
    }
}
