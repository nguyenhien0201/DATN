using System;
using System.Collections.Generic;
using System.Text;

namespace AppMobile.Controllers
{
    class PatientController : BaseController
    {
        static Models.Patient current;
        public void Default()
        {
            Publish("patient/getpatient", new object());
        }

        public object Default(List<Models.Patient> p)
        {
            ViewData["controller"] = "Patient";
            return View(p);
        }

        public void Detail(DataContext patient)
        {
            var p = patient.ToObject<Models.Patient>();
            current = p;
            Publish("Patient/IndexList", p);
            //return View(p);
        }

        public object Detail(List<Models.Index> indexs)
        {
            DataContext v = new DataContext();
            v.Push("Patient", current);
            v.Push("Index", indexs);

            return View(v);
        }
    }
}
