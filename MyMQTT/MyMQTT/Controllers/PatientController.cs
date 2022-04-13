using System;
using System.Collections.Generic;
using System.Text;
using Models;

namespace MyMQTT.Controllers
{
    class PatientController : BaseController
    {
        public object Default()
        {
            return View();
        }
        public void indexlist()
        {
            var u = Json.Convert<Models.User>(Request);
            var li = DB.MedicalRecord.Find(u.UserName).ListIndex;

            var res = new DataContext();
            res.Push("Code", 0);
            res.Push("Value", li);
            Response(res);
        }
        public void GetPatient()
        {
            var res = Excute(Request);
            Response(res);
        }
    }
}
