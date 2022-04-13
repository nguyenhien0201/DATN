using System;
using System.Collections.Generic;
using System.Text;
using Models;

namespace AppMobile.Views.Patient
{
    class Default : MyNavigationRootPage<MyPatientListView, object>
    {
        protected override void RenderCore()
        {
            Caption = "Danh sách bệnh nhân";
            MainContent.Binding(Model, ControllerName);
        }
    }
    
}
