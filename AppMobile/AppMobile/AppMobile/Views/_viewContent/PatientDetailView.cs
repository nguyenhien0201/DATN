using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace AppMobile.Views
{
    public class PatientDetailView : StackLayout
    {
        public PatientDetailView() : base()
        {
            BackgroundColor = Color.WhiteSmoke;
            Spacing = 0;
        }
        public virtual void Binding(object model)
        {
            var models = DataContext.FromObject(model);
            var patient = Json.Convert<Models.Patient>(models["Patient"]);

            var i = models["Index"];
            var indexs = JArray.FromObject(i).ToObject<List<object>>();

            AddView(patient, indexs);
        }

        public void AddView(object patient) 
        {
            this.Children.Add(new PatientForm { BindingContext = patient });
            this.Children.Add(new Separate(35, "Thông tin cá nhân"));
            this.Children.Add(new InfoForm { BindingContext = patient });
        }
        public void AddView(object patient, object indexList) 
        {
            AddView(patient);
            this.Children.Add(new Separate(35, "Chỉ số"));
            
            var index = new MyIndexListView();
            index.Margin = new Thickness(20, 5);
            index.Binding(indexList, null);
            
            this.Children.Add(index);
        }
    }
}
