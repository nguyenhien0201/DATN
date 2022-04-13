using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppMobile.Views.Patient
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PatientDetail : ContentView
	{
		public PatientDetail ()
		{
			InitializeComponent ();
            patientForm.Children.Add(new PatientForm());

            var i = new IndexForm();
            var index1_patient1 = new Models.Index { Name = "SPO2", Value = 90, Unit = "%" };
            i.BindingContext = index1_patient1;
            IndexList.Children.Add(i);

            var i2 = new IndexForm();
            var index1_patient12 = new Models.Index { Name = "Nhiệt độ", Value = 37, Unit = "°C" };
            i2.BindingContext = index1_patient12;
            IndexList.Children.Add(i2);

            var i3 = new IndexForm();
            var index1_patient13 = new Models.Index { Name = "Nhịp tim", Value = 66, Unit = "bpm" };
            i3.BindingContext = index1_patient13;
            IndexList.Children.Add(i3);
        }
	}
}