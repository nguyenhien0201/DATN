using System;
using System.Collections.Generic;
using System.Mvc;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace AppMobile
{
    public partial class App : Application
    {
        static public void Execute(string url, params object[] args)
        {
            System.Mvc.Engine.Execute(url, args);
        }

        public App()
        {
            InitializeComponent();
            //MainPage = new MainPage();

            System.Mvc.Engine.Register(this, result =>
            {
                var view = result?.View;
                if (view == null) return;
                if (view != null)
                {
                   
                    if (view is Views.INavigationItemPage)
                    {
                        var root = (NavigationPage)MainPage;
                        var item = (Page)view.Content;

                        root.PushAsync(item);
                    }
                    else
                    {
                        if (view.Content is ContentPage) MainPage = (Page)view.Content;
                        else 
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                MainPage = new ContentPage();
                                MainPage = (Page)view.Content;
                            });
                        }
                    }
                }
            });

            App.Execute("Home");
        }
        
        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

    }

}

namespace System
{
    public static class Screen
    {
        public static int Width { get; set; }
        public static int Height { get; set; }
    }
}
//#region My Pages
//namespace AppMobile
//{

//    class MyApp : System.Mvc.Engine
//    {
//        static BsonData.DataBase _mainDb;
//        public static BsonData.DataBase MainDb => _mainDb;

//        public static void Init(Application app)
//        {
//            Register(app, result => {

//                var view = result.View;
//                var page = (Page)((System.Mvc.IRenderer)view).GetResult();

//                if (page == null)
//                {
//                    return;
//                }

//                if (page != null)
//                {
//                    if (page is IRootPage)
//                    {
//                        ((IDisposable)app.MainPage).Dispose();
//                        app.MainPage = page;
//                    }
//                    else if (page is MyNavigationItemPage)
//                    {
//                        var np = (MyNavigationItemPage)page;
//                        var root = (MyNavigationPage)app.MainPage;

//                        root.Back(np.Back);
//                        root.PushAsync(page);
//                    }
//                }
//            });

//            StyleSheetMap.SetClass<MyButton>(new StyleSheet
//            {
//                Background = "2C50CA",
//                Foreground = "fff",
//                BorderSize = 1,
//                CornerRadius = 8,
//            });

//            _mainDb = new BsonData.DataBase(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Data");

//            Execute("home");
//        }

//        public static void Exit()
//        {
//            System.Environment.Exit(0);
//        }
//    }



//    public interface IRootPage : IDisposable
//    {
//        Page TopPage { get; }
//    }

//    public class MyContentPage : ContentPage, IDisposable
//    {
//        IRenderer _renderer;
//        public IRenderer Renderer
//        {
//            get => _renderer;
//            set
//            {
//                _renderer = value;
//                View content = value.MainContent;
//                if (value.Scrollable)
//                {
//                    content = new ScrollView { Content = content };
//                }
//                this.Content = new Grid
//                {
//                    Children = { content },
//                };
//            }
//        }
//        public void Dispose()
//        {
//            ((IDisposable)Renderer)?.Dispose();
//        }
//    }
//    public class MySimplePage : MyContentPage, IRootPage
//    {
//        public Page TopPage => this;
//    }
//    public class MyCarouselPage : CarouselPage, IRootPage
//    {
//        public Page TopPage => this.CurrentPage;

//        public void Dispose()
//        {
//            foreach (MyContentPage page in this.Children)
//                page.Dispose();
//        }
//    }
//    public class MyNavigationPage : NavigationPage, IRootPage
//    {
//        public Page TopPage => base.CurrentPage;
//        public void Dispose()
//        {
//            foreach (MyContentPage page in this.Navigation.NavigationStack)
//                page.Dispose();
//        }

//        public MyNavigationPage(Page root) : base(root)
//        {
//            this.Popped += MyNavigationPage_Popped;
//        }

//        private void MyNavigationPage_Popped(object sender, NavigationEventArgs e)
//        {
//            ((IDisposable)e.Page).Dispose();
//        }

//        public void Back(int count)
//        {
//            while (count > 0 && CurrentPage != RootPage)
//            {
//                ((MyContentPage)CurrentPage).Dispose();
//                this.PopAsync();
//            }
//        }
//    }
//    public class MyNavigationItemPage : MyContentPage
//    {
//        public int Back { get; set; }
//    }
//}
//#endregion

//#region Renderers
//namespace AppMobile
//{
//    public interface IRenderer 
//    {
//        bool Scrollable { get; }
//        string Title { get; }
//        MyTableLayout MainContent { get; }
//    }

//    public class Renderer<TModel> : System.Mvc.Renderer<MyTableLayout, TModel>, IDisposable, IRenderer
//    {
//        public bool Scrollable
//        {
//            get; set;
//        }
//        public string Title { get; set; }

//        public event Action Disposing;
//        public virtual void Dispose()
//        {
//            Disposing?.Invoke();
//        }

//        public virtual void BeginWaiting(Action callback)
//        {
//            var indic = new MyActivityIndicator();
//            indic.SetBlending(0.43, Color.Black).Show(MainContent);
//        }

//        public void EndWaiting()
//        {
//            var indic = MainContent.Children[MainContent.Children.Count - 1] as MyActivityIndicator;
//            if (indic != null)
//            {
//                indic.Hide();
//            }
//        }

//        /// <summary>
//        /// Tao PAGE chua MainView
//        /// </summary>
//        /// <param name="view"></param>
//        /// <returns></returns>
//        protected virtual MyContentPage CreatePage()
//        {
//            return new MySimplePage();
//        }

//        /// <summary>
//        /// Tao PAGE tra ve cho Engine
//        /// </summary>
//        /// <returns></returns>
//        public override object GetResult()
//        {
//            var page = CreatePage();
//            page.Renderer = this;
//            page.Title = this.Title;

//            return page;
//        }

//        protected override void RenderCore(Controller controller, ViewDataDictionary viewData, TModel model, MyTableLayout mainContent)
//        {
//            base.RenderCore(controller, viewData, model, mainContent);
//            LoadItems();
//        }
//        protected virtual void LoadItems()
//        {
//            foreach (var m in this.GetType().GetMethods())
//            {
//                if (m.Name[0] == '_')
//                {
//                    MainContent.AddRow((View)m.Invoke(this, new object[0]));
//                }
//            }
//        }
//    }

//    public class ToolbarPageRenderer<TModel> : Renderer<TModel>
//    {

//        protected ToolbarItem CreateToolBarItem(string key, string text, string url)
//        {
//            var tbi = new ToolbarItem
//            {
//                Text = text
//            };
//            //if (key != null)
//            //{
//            //    var ir = new MyIcon(key);
//            //    tbi.IconImageSource = new MyImage(key).Source;

//            //}
//            if (url != null) { tbi.Clicked += (s, e) => MyApp.Execute(url); }

//            return tbi;
//        }

//        protected virtual List<ToolbarItem> GetToolBarItems()
//        {
//            return new List<ToolbarItem> {
//                //CreateToolBarItem("main", "Home", "Device"),
//                CreateToolBarItem("menu", "Menu", "Setting"),
//            };
//        }

//        public override object GetResult()
//        {
//            var page = (Page)base.GetResult();

//            var toolBarItems = GetToolBarItems();
//            if (toolBarItems != null)
//            {
//                foreach (var item in toolBarItems)
//                    page.ToolbarItems.Add(item);
//            }

//            return page;
//        }
//    }

//    public class RootNavigationPageRenderer<TModel> : ToolbarPageRenderer<TModel>
//    {
//        public override object GetResult()
//        {
//            return new MyNavigationPage((Page)base.GetResult());
//        }
//    }

//    public class ItemNavigationPageRenderer<TModel> : ToolbarPageRenderer<TModel>
//    {
//        protected override MyContentPage CreatePage()
//        {
//            return new MyNavigationItemPage();
//        }
//    }

//    public class CarouselRenderer<TModel> : Renderer<TModel>
//    {
//    }

//    //public class Waiting : System.Mvc.IRenderer, IView
//    //{
//    //    public void Render(Controller controller)
//    //    {
//    //    }
//    //    public object GetResult()
//    //    {
//    //        var ts = new System.Threading.ThreadStart(() => {
//    //            var page = (MyContentPage)((IRootPage)App.Current.MainPage).TopPage;

//    //            page.Dispatcher.BeginInvokeOnMainThread(() => {
//    //                var indic = new MyActivityIndicator();
//    //                indic.SetBlending(0.43, Color.Black).Show(page);
//    //            });
//    //        });
//    //        new System.Threading.Thread(ts).Start();
//    //        return null;
//    //    }
//    //}
//}
//#endregion