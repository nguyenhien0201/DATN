using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class Patient : User
    {
        public override string GetObjectIdName() => PhoneNumber;
        public string PatientID => ObjectId;
        public string Address
        {
            get => GetString("Address");
            set => SetString("Address", value);
        }
        public bool IsWarning
        {
            get => GetValue<bool>("IsWarning");
            set => Push("IsWarning", value);
        }

        public bool CheckWarning()
        {
            return (bool)DB.MedicalRecord.Find(PatientID)?.IsWarning;
        }

        public string DoctorID {
            get => GetString("DoctorID");
            set => SetString("DoctorID", value);
        }
        string _doctorName;
        public string DoctorName
        {
            get
            {
                if (_doctorName == null && !string.IsNullOrEmpty(DoctorID))
                {
                    _doctorName = DB.Doctor.Find(DoctorID)?.GetString("Name");
                }
                return _doctorName;
            }
        }

        public object getpatient(User u)
        {
            var p = DB.Patient.Find(u.UserName);
            var res = new List<Patient> { p };
            return Ok(res);
        }
    }

    public class PatientCollection : DataMap<Patient>
    {

    }
    partial class DB
    {
        static PatientCollection _patient;
        public static PatientCollection Patient
        {
            get
            {
                if (_patient == null)
                {
                    _patient = new PatientCollection();
                    _patient.Load();
                }
                return _patient;
            }
        }
    }

}


