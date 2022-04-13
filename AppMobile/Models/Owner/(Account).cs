using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Account : BaseModel
    {
        public string UserName {
            get => GetString("UserName");
            set => SetString("UserName", value);
        }
        public string Password
        {
            get => GetString("Password");
            set => SetString("Password", value);
        }
        //public string Token { get; set; }
        public string AuthorName {
            get => GetString("AuthorName");
            set => SetString("AuthorName", value);
        }
        public DateTime LastLogin { get; set; }

        //bool CheckAuthor(string author)
        //{
        //    return (string.IsNullOrEmpty(AuthorName));
        //}
        protected DataContext CreateMQTTResponse(int code, string message, object value)
        {
            var context = new DataContext();
            context.Push("Code", code);
            context.Push("Message", message);
            context.Push("Value", value);
            
            return context;
        }
        protected virtual object Ok(object value)
        {
            return CreateMQTTResponse(0, null, value);
        }
        protected virtual object Error(int code, string message)
        {
            return CreateMQTTResponse(code, message, null);
        }
        public object Login()
        {
            //var acc = context.ChangeType<Account>();
            
            int code = TryLogin();

            if (code != 0)
            {
                return Error(code, null);
            }
            var user = DB.UserCollection.CreateUser(this);
            return Ok(user);
        }
        int TryLogin()
        {
            var un = UserName.ToLower();
            var acc = DB.Accounts.FindById<Account>(un);
            if (acc == null)
            {
                return -1;
            }

            if (acc.Password != un.JoinMD5(Password).ToMD5())
            {
                return -2;
            }

            this.Copy(acc);
            this.Remove("Password");
            return 0;
        }
        public static int CreateAccount(string userName, string password, string authorname)
        {
            var db = DB.Accounts;
            if (db.Contains(userName))
            {
                return -1;
            }

            if (DB.UserCollection.GetActor(authorname) == null)
            {
                return -2;
            }

            userName = userName.ToLower();
            if (password == null)
            {
                password = userName;
            }

            var acc = new Account();

            acc.Push("UserName", userName);
            acc.Push("Password", userName.JoinMD5(password).ToMD5());
            acc.Push("AuthorName", authorname);
            DB.Accounts.Insert(userName, acc);

            CreatPseudoDB();

            return 0;
        }

        public static void CreatPseudoDB()
        {
            //create address
            var address = "Hà Nội";
            var address1 = "Tp.HCM";

            //create and add account
            var acc_admin1 = CreateAccount("Admin1", "Admin1", "Admin");
            var acc_admin2 = CreateAccount("Admin2", "Admin2", "Admin");
            var acc_doctor = CreateAccount("Doctor", "Doctor", "Doctor");
            var acc_doctor1 = CreateAccount("Doctor1", "Doctor1", "Doctor");
            var acc_doctor2 = CreateAccount("Doctor2", "Doctor2", "Doctor");
            var acc_patient = CreateAccount("Patient", "Patient", "Patient");
            var acc_patient1 = CreateAccount("Patient1", "Patient1", "Patient");
            var acc_patient2 = CreateAccount("Patient2", "Patient2", "Patient");
            var acc_patient3 = CreateAccount("Patient3", "Patient3", "Patient");
            var acc_patient4 = CreateAccount("Patient4", "Patient4", "Patient");
            var acc_patient5 = CreateAccount("Patient5", "Patient5", "Patient");

            //create admin
            var admin = new Admin
            {
                Name = "Admin.Huy",
                Gender = "Nam",
                DateOfBirth = new DateTime(1999, 2, 15),
                PhoneNumber = "147",
                Email = "huyl1201@gmail.com",
                UserName = "Admin"
            };
            var admin1 = new Admin
            {
                Name = "Admin.Hiền",
                Gender = "Nữ",
                DateOfBirth = new DateTime(1999, 2, 15),
                PhoneNumber = "258",
                Email = "hu0201@gmail.com",
                UserName = "Admin1"
            };
            var admin2 = new Admin
            {
                Name = "Admin.Hiệp",
                Gender = "Nam",
                DateOfBirth = new DateTime(1999, 2, 15),
                PhoneNumber = "369",
                Email = "hu1@gmail.com",
                UserName = "Admin2"
            };

            //create patient
            var patient = new Patient
            {
                Name = "Lê Quang Huy",
                Gender = "Nam",
                DateOfBirth = new DateTime(1999, 2, 15),
                PhoneNumber = "0898991502",
                Email = "huyl150201@gmail.com",
                Address = address,
                UserName = "Patient",
                DoctorID = "Doctor",
            };
            var patient1 = new Patient
            {
                Name = "Nguyễn Thị Hiền",
                Gender = "Nữ",
                DateOfBirth = new DateTime(1999, 1, 2),
                PhoneNumber = "0359942269",
                Email = "nguyenhien211999@gmail.com",
                Address = address,
                UserName = "Patient1",
                DoctorID = "Doctor"
            };
            var patient2 = new Patient
            {
                Name = "Lê Sĩ Hiệp",
                Gender = "Nam",
                DateOfBirth = new DateTime(1999, 3, 19),
                PhoneNumber = "0373693529",
                Email = "hiep193@gmail.com",
                Address = address1,
                UserName = "Patient2",
                DoctorID = "Doctor1"
            };
            var patient3 = new Patient
            {
                Name = "Lê Huy",
                Gender = "Nam",
                DateOfBirth = new DateTime(1999, 2, 15),
                PhoneNumber = "123",
                Email = "huyl@gmail.com",
                Address = address,
                UserName = "Patient3",
                DoctorID = "Doctor1"

            };
            var patient4 = new Patient
            {
                Name = "Nguyễn Hiền",
                Gender = "Nữ",
                DateOfBirth = new DateTime(1999, 1, 2),
                PhoneNumber = "456",
                Email = "nguyenhien@gmail.com",
                Address = address,
                UserName = "Patient4",
                DoctorID = "Doctor2"
            };
            var patient5 = new Patient
            {
                Name = "Lê Hiệp",
                Gender = "Nam",
                DateOfBirth = new DateTime(1999, 3, 19),
                PhoneNumber = "789",
                Email = "hiepl@gmail.com",
                Address = address1,
                UserName = "Patient5",
                DoctorID = "Doctor2"
            };

            var patient_list = new List<Patient>
            {
                patient, patient1
            };
            var patient_list1 = new List<Patient>
            {
                patient2, patient3
            };
            var patient_list2 = new List<Patient>
            {
                patient4, patient5
            };

            //create doctor
            var doctor = new Doctor
            {
                Name = "Doctor.Huy",
                Gender = "Nam",
                DateOfBirth = new DateTime(1999, 2, 15),
                PhoneNumber = "987",
                Email = "l150201@gmail.com",
                UserName = "Doctor",
                Patients = patient_list,
            };
            var doctor1 = new Doctor
            {
                Name = "Doctor.Hiền",
                Gender = "Nữ",
                DateOfBirth = new DateTime(1999, 2, 15),
                PhoneNumber = "654",
                Email = "n211999@gmail.com",
                UserName = "Doctor1",
                Patients = patient_list1,
            };
            var doctor2 = new Doctor
            {
                Name = "Doctor.Hiệp",
                Gender = "Nam",
                DateOfBirth = new DateTime(1999, 2, 15),
                PhoneNumber = "321",
                Email = "l190203@gmail.com",
                UserName = "Doctor2",
                Patients = patient_list2,
            };

            //create index
            var index1_patient = new Index { Name = "SPO2", Value = 99, Unit = "%", IsWarning = false };
            var index2_patient = new Index { Name = "Temp", Value = 39, Unit = "°C", IsWarning = true };
            var index3_patient = new Index { Name = "Pulse", Value = 78, Unit = "bpm", IsWarning = false };
            var list_index_patient = new List<Index> { index1_patient, index2_patient, index3_patient };

            var index1_patient1 = new Index { Name = "SPO2", Value = 97, Unit = "%", IsWarning = false };
            var index2_patient1 = new Index { Name = "Temp", Value = 37, Unit = "°C", IsWarning = false };
            var index3_patient1 = new Index { Name = "Pulse", Value = 80, Unit = "bpm", IsWarning = false };
            var list_index_patient1 = new List<Index> { index1_patient1, index2_patient1, index3_patient1 };

            var index1_patient2 = new Index { Name = "SPO2", Value = 2, Unit = "%", IsWarning = false };
            var index2_patient2 = new Index { Name = "Temp", Value = 2, Unit = "°C", IsWarning = false };
            var index3_patient2 = new Index { Name = "Pulse", Value = 2, Unit = "bpm", IsWarning = false };
            var list_index_patient2 = new List<Index> { index1_patient2, index2_patient2, index3_patient2 };

            var index1_patient3 = new Index { Name = "SPO2", Value = 3, Unit = "%", IsWarning = false };
            var index2_patient3 = new Index { Name = "Temp", Value = 3, Unit = "°C", IsWarning = false };
            var index3_patient3 = new Index { Name = "Pulse", Value = 3, Unit = "bpm", IsWarning = false };
            var list_index_patient3 = new List<Index> { index1_patient3, index2_patient3, index3_patient3 };

            var index1_patient4 = new Index { Name = "SPO2", Value = 4, Unit = "%", IsWarning = false };
            var index2_patient4 = new Index { Name = "Temp", Value = 4, Unit = "°C", IsWarning = false };
            var index3_patient4 = new Index { Name = "Pulse", Value = 4, Unit = "bpm", IsWarning = false };
            var list_index_patient4 = new List<Index> { index1_patient4, index2_patient4, index3_patient4 };

            var index1_patient5 = new Index { Name = "SPO2", Value = 5, Unit = "%", IsWarning = true };
            var index2_patient5 = new Index { Name = "Temp", Value = 5, Unit = "°C", IsWarning = false };
            var index3_patient5 = new Index { Name = "Pulse", Value = 5, Unit = "bpm", IsWarning = false };
            var list_index_patient5 = new List<Index> { index1_patient5, index2_patient5, index3_patient5 };

            //create medical record
            var medical_record_patient = new MedicalRecord { PatientID = patient.UserName, DiseaseName = "Bệnh", ListIndex = list_index_patient };
            var medical_record_patient1 = new MedicalRecord { PatientID = patient1.UserName, DiseaseName = "Bệnh1", ListIndex = list_index_patient1 };
            var medical_record_patient2 = new MedicalRecord { PatientID = patient2.UserName, DiseaseName = "Bệnh2", ListIndex = list_index_patient2 };
            var medical_record_patient3 = new MedicalRecord { PatientID = patient3.UserName, DiseaseName = "Bệnh3", ListIndex = list_index_patient3 };
            var medical_record_patient4 = new MedicalRecord { PatientID = patient4.UserName, DiseaseName = "Bệnh4", ListIndex = list_index_patient4 };
            var medical_record_patient5 = new MedicalRecord { PatientID = patient5.UserName, DiseaseName = "Bệnh5", ListIndex = list_index_patient5 };

            patient.IsWarning = medical_record_patient.CheckWarning();
            patient1.IsWarning = medical_record_patient1.CheckWarning();
            patient2.IsWarning = medical_record_patient2.CheckWarning();
            patient3.IsWarning = medical_record_patient3.CheckWarning();
            patient4.IsWarning = medical_record_patient4.CheckWarning();
            patient5.IsWarning = medical_record_patient5.CheckWarning();

            //add to DB
            //add admin
            DB.Admin.Insert(admin.UserName, admin);
            DB.Admin.Insert(admin1.UserName, admin1);
            DB.Admin.Insert(admin2.UserName, admin2);

            //add patient
            foreach (var p in patient_list)
            {
                DB.Patient.Insert(p.UserName, p);
            }
            foreach (var p in patient_list1)
            {
                DB.Patient.Insert(p.UserName, p);
            }
            foreach (var p in patient_list2)
            {
                DB.Patient.Insert(p.UserName, p);
            }

            //add doctor
            DB.Doctor.Insert(doctor.UserName, doctor);
            DB.Doctor.Insert(doctor1.UserName, doctor1);
            DB.Doctor.Insert(doctor2.UserName, doctor2);

            //add medical record
            DB.MedicalRecord.Insert(medical_record_patient.PatientID, medical_record_patient);
            DB.MedicalRecord.Insert(medical_record_patient1.PatientID, medical_record_patient1);
            DB.MedicalRecord.Insert(medical_record_patient2.PatientID, medical_record_patient2);
            DB.MedicalRecord.Insert(medical_record_patient3.PatientID, medical_record_patient3);
            DB.MedicalRecord.Insert(medical_record_patient4.PatientID, medical_record_patient4);
            DB.MedicalRecord.Insert(medical_record_patient5.PatientID, medical_record_patient5);
        }
        public static int CreateAccount(DataContext context)
        {
            var acc = context.ChangeType<Account>();
            return CreateAccount(acc.UserName, acc.Password, acc.AuthorName);
        }
    }
    partial class DB
    {
        static Collection _accounts;
        public static Collection Accounts
        {
            get
            {
                const string ad = "Admin";
                if (_accounts == null)
                {
                    _accounts = Main.GetCollection<Account>();
                    Account.CreateAccount(ad, ad, ad);
                }
                return _accounts;
            }
        }
    }

}
