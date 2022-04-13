using System;
using System.Collections.Generic;
using System.Text;

namespace AppMobile.Controllers
{
    class DoctorController : BaseController
    {
        public void Default()
        {
            Publish("doctor/patientlist", new object());
        }
    }
}
