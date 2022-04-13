using System;
using System.Collections.Generic;
using System.Text;
using System.Mvc;
using Xamarin.Forms;

namespace AppMobile.Views
{
    public abstract class BaseView<TView, TModel> : IView
        where TView : View, new()
    {
        public object Content => CreatePage();
        public ViewDataDictionary ViewBag { get; set; }
        public void Render(object model)
        {
            Model = (TModel)model;
            MainContent = CreatMainContent();
            RenderCore();
        }

        //mine
        public TModel Model { get; private set; }
        public TView MainContent { get; private set; }
        protected string Caption { get; set; } = "Default";
        protected virtual string ControllerName => (string)ViewBag["controller"];

        protected virtual TView CreatMainContent()
        {
            return new TView();
        }
        protected virtual Page CreatePage()
        {
            return new ContentPage
            {
                Title = Caption,
                Content = new ScrollView { Content = MainContent },
            };
        }

        protected abstract void RenderCore();
    }

    public interface IRootPage { }
    public abstract class MyNavigationRootPage<TView, TModel> : BaseView<TView, TModel>,
                                                                IRootPage
             where TView : View, new()
    {
        ToolbarItem GetToolbarItem(string name, string iconName, string url)
        {
            return new ToolbarItem(name, iconName + ".png", () =>
            {
                App.Execute(url);
            });
        }

        protected override Page CreatePage()
        {
            var page = new NavigationPage((Page)base.CreatePage());

            page.ToolbarItems.Add(GetToolbarItem("Home", "home", "home/home"));
            page.ToolbarItems.Add(GetToolbarItem("Setting", "setting", "setting"));

            return page;
        }
    }

    public interface INavigationItemPage { }
    public abstract class MyNavigationItemPage<TView, TModel> : BaseView<TView, TModel>,
                                                                INavigationItemPage
        where TView : View, new()
    {

    }

    public class MyListView<TContent> : StackLayout
        where TContent : View, new()
    {
        public virtual void Binding(object model, string controllerName)
        {
            var models = (IEnumerable<object>)model;
            foreach (var p in models)
            {
                var item = new TContent
                {
                    
                };

                try
                {
                    item.BindingContext = p;

                }
                catch (Exception ex) { }
                if (string.IsNullOrEmpty(controllerName) == false)
                {
                    DataContext patient = DataContext.FromObject(p);
                    TapGestureRecognizer tap = new TapGestureRecognizer();
                    tap.Tapped += (s, e) =>
                    {
                        App.Execute(controllerName + "/detail", patient);
                    };
                    item.GestureRecognizers.Add(tap);
                }
                this.Children.Add(item);
            }
        }
    }

}