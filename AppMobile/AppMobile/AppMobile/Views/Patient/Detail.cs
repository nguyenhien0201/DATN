using System;
using System.Collections.Generic;
using System.Text;

namespace AppMobile.Views.Patient
{
    class Detail : MyNavigationItemPage<PatientDetailView, object>
    {
        protected override void RenderCore()
        {
            Caption = "Chi tiết";
            MainContent.Binding(Model);
        }
    }
}
