using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace AppMobile.Views
{
    public class MyPatientListView : MyListView<PatientForm>
    {
        public override void Binding(object model, string controllerName)
        {
            //xu ly color va warning text
            var lstPatient = (List<Models.Patient>)model;

            List<ViewInfo> lstViewInfo = new List<ViewInfo>();
            foreach (var p in lstPatient)
            {
                //var patient = Json.Convert<Models.Patient>(p);
                lstViewInfo.Add(new ViewInfo(p));
            }
            model = lstViewInfo;
            base.Binding(model, controllerName);
        }
    }

    class ViewInfo : Models.Patient
    {
        public string WarningText { get; set; }
        public Color Color { get; set; }

        public ViewInfo(Models.Patient p)
        {
            this.Copy(p);
            Color = IsWarning ? Color.FromHex("#e05252") : Color.FromHex("#04AA6D");
            WarningText = IsWarning ? "Bất thường" : "Bình thường";
        }
    }
}
