using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace AppMobile.Views
{
    public class MyIndexListView :  MyListView<IndexForm>
    {
        public override void Binding(object model, string controllerName)
        {
            //xu ly color va warning text

            var lstIndex = (List<object>)model;
            List<IndexViewInfo> lstViewInfo = new List<IndexViewInfo>();
            foreach (var i in lstIndex)
            {
                var index = Json.Convert<Models.Index>(i);
                lstViewInfo.Add(new IndexViewInfo(index));
            }
            model = lstViewInfo;
            base.Binding(model, controllerName);
        }
    }

    class IndexViewInfo : Models.Index
    {
        public string WarningText { get; set; }
        public Color Color { get; set; }

        public IndexViewInfo(Models.Index p)
        {
            this.Copy(p);
            Color = IsWarning ? Color.FromHex("#e05252") : Color.FromHex("#04AA6D");
            WarningText = IsWarning ? "Bất thường" : "Bình thường";
        }
    }
}
