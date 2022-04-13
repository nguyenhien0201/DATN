using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace MyMQTT.Controllers
{
    class WarningController : BaseController
    {
        public void Default()
        {
            DataContext v = Json.Convert<DataContext>(Request);
            var PatientID = v.GetString("UserName");

            var p = DB.Patient.Find(PatientID);
            var lu = DB.UserCollection.Values;

            foreach (User i in lu)
            {
                if (i.UserName == PatientID) Publish("Warning/" + i.Token, "warning", p);
                if (i.UserName == p.DoctorID) Publish("Warning/" + i.Token, "warning", p);
            }

            //var p_medicalRecord = DB.MedicalRecord.Find(p.UserName);

            //Random rnd = new Random();
            //int r = rnd.Next(2);

            //var li = p_medicalRecord.ListIndex;
            //p_medicalRecord.ListIndex[r].Value = 9;
            //var index = p_medicalRecord.ListIndex[r];
            //DB.MedicalRecord.Update(PatientID, p_medicalRecord);
        }
    }
}
