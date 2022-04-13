using System;
using System.Collections.Generic;
namespace Xamarin.Forms
{
    public class MyButtonEventArgs : EventArgs
    {
        public bool Handled;
    }
    public class MyMenuItemInfo
    {
        public string Text { get; set; }
        public string Url { get; set; }
        public string ClassName { get; set; }
        public string IconName { get; set; }
        public int IconSize { get; set; }
        public Color IconColor { get; set; }
        public string Description { get; set; }

        public string Name { get; set; }
        public bool BeginGroup { get; set; }
        public bool Disabled { get; set; }
        public bool IsActive { get; set; }

        public List<MyMenuItemInfo> Childs { get; set; }
    }

    public class MyMenuItemView : MyTableLayout
    {
        MyImage _icon;
        public MyImage Icon => _icon;

        Label _label;
        public Label Label => _label;

        public string Url { get; set; }

        MyMenuItemView()
            : base(2, 2)
        {
            //2 cot, set width cot 0, cot 1 khong set
            this.SetWidths(65, 0);
            this.SetHeights(65, 0);

            _icon = new MyImage {  };
            Grid.SetRowSpan(_icon, 2);
            //_icon.SetValue(RowSpanProperty, 2);
            _icon.SquareSize = 20;

            _label = new Label { VerticalOptions = LayoutOptions.Center, FontSize = 17, TextColor = Color.Black };
                
            Grid.SetRowSpan(_label, 2);

            this.Add(0, 1, _label);
            this.Add(_icon);
        }
        public MyMenuItemView(MyMenuItemInfo info)
            : this()
        {
            Icon.SetSouce(info.IconName);
            //Icon.Foreground = info.IconColor;

            Url = info.Url;
            _label.Text = info.Text;
            
            SetSubcontent(info.Description);
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();
            //if (Url != null || Clicked != null)
            {
                var btn = new Button { BackgroundColor = Color.Transparent };
                btn.SetValue(RowSpanProperty, 2);
                btn.SetValue(ColumnSpanProperty, 2);

                btn.Clicked += delegate {

                    if (Clicked != null)
                    {
                        var e = new MyButtonEventArgs();
                        Clicked.Invoke(this, e);

                        if (e.Handled) return;
                    }

                    if (Url != null)
                    {
                        System.Mvc.Engine.Execute(Url);
                    }
                };

                this.Add(btn);
            }
        }

        public void SetSubcontent(View subcontent)
        {
            Grid.SetRowSpan(_label, 1);
            //_label.Margin = new Thickness(0, 10, 0, 0);
           
            _label.VerticalOptions = LayoutOptions.Center;

            
            subcontent.VerticalOptions = LayoutOptions.Start;
            //subcontent.Margin = new Thickness(0, -5, 0, 0);
            Grid.SetRow(subcontent, 1);
            Grid.SetColumn(subcontent, 1);

            var r = Grid.GetRowSpan(subcontent);
            var c = Grid.GetRow(subcontent);
            this.Add(1, 1, subcontent);
        }
        public void SetSubcontent(string text)
        {
            if (text != null)
            {
                this.SetSubcontent(new Label
                {
                    Text = text,
                    FontSize = 14,
                    TextColor = Color.Blue,
                });
            }
            
        }

        public void SetExtendedContent(View extendedContent)
        {
            if (this.ColumnDefinitions.Count < 3)
            {
                this.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });
            }
            this.Add(0, 2, extendedContent);
            extendedContent.SetValue(RowSpanProperty, 2);
            extendedContent.VerticalOptions = LayoutOptions.Center;
            extendedContent.HorizontalOptions = LayoutOptions.Center;
        }

        public event Action<MyMenuItemView, MyButtonEventArgs> Clicked;
    }

    
    public class MyMenuView : StackLayout
    {
        public MyMenuView()
        {
            this.Spacing = 0;
        }

        public void Separate(int height, string top, string bottom)
        {
            var box = new Grid {
                HeightRequest = height,
                BackgroundColor = Color.WhiteSmoke
            };

            if (top != null)
            {
                box.Children.Add(new Label {
                    Text = top,
                    TextColor = Color.Gray,
                    VerticalOptions = LayoutOptions.Start,
                    FontSize = 14,
                    Margin = new Thickness(15, 5, 5, 5),
                });
            }

            if (bottom != null)
            {
                box.Children.Add(new Label
                {
                    Text = bottom,
                    TextColor = Color.Gray,
                    VerticalOptions = LayoutOptions.End,
                    FontSize = 14,
                    Margin = new Thickness(15, 5, 5, 5)
                });
            }

            this.Children.Add(box);
        }

        public MyMenuItemView Add(MyMenuItemView item)
        {
            var hr = new BoxView {
                HeightRequest = 1,
                Color = Color.WhiteSmoke,
                VerticalOptions = LayoutOptions.End,
                //Margin = new Thickness(IconSize, 0, 0, 0),
            };
            var grid = new Grid {
                Children = { item, hr },
            };
            this.Children.Add(grid);

            ItemAdded?.Invoke(item);

            return item;
        }

        public MyMenuItemView Add(MyMenuItemInfo info)
        {
            var item = new MyMenuItemView(info);
            return Add(item);
        }

        public void AddGroup(List<MyMenuItemInfo> infos)
        {
            foreach(var i in infos)
            {
                this.Add(i);
            }
        }

        public event Action<MyMenuItemView> ItemAdded;

        System.Collections.IEnumerable _itemsSource;
        public System.Collections.IEnumerable ItemsSource
        {
            get { return _itemsSource; }
            set
            {
                if (_itemsSource == value) return;
                this.Children.Clear();

                if((_itemsSource = value)!= null)
                {
                    foreach(MyMenuItemInfo info in value)
                    {
                        this.Add(info);
                    }
                }
            }
        }
    }
}
