using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace AppMobile.Views
{
    class MyUserInfoView : PatientDetailView
    {
        public MyUserInfoView() : base()
        {
            BackgroundColor = Color.WhiteSmoke;
            Spacing = 0;

        }
        public override void Binding(object model)
        {
            var models = DataContext.FromObject(model);
            var patient = Json.Convert<Models.User>(models);


            AddView(patient);
        }
    }
}
