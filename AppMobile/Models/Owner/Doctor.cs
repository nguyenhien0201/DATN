using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Models
{
    public class Doctor : User
    {
        public override string GetObjectIdName() => UserName;
        public string DoctorID => ObjectId;
        public string Specialization{ get; set; } = string.Empty;
        List<Patient> _patients;

        public List<Patient> Patients
        {
            get
            {
                var v = this["Patients"];
                return JArray.FromObject(v).ToObject<List<Patient>>();
            }
            set => Push("Patients", value);
        }

        public void AddPatient(Patient p)
        {
            _patients.Add(p);
        }
        public void RemovePatient(Patient p)
        {
            _patients.Remove(p);
        }
        public object patientlist(User u)
        {
            var doctor = DB.Doctor.Find(u.UserName);
            var rs = doctor.Patients;
            return Ok(rs);
        }
    }
    public class DoctorCollection : DataMap<Doctor>
    {

    }
    partial class DB
    {
        static DoctorCollection _doctor;
        public static DoctorCollection Doctor
        {
            get
            {
                if (_doctor == null)
                {
                    _doctor = new DoctorCollection();
                    _doctor.Load();
                }
                return _doctor;
            }
        }
    }

}
