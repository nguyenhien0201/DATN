using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Models
{
    public class MedicalRecord : BaseModel
    {
        public string PatientID {
            get => GetString("PatientID");
            set => SetString("PatientID", value);
        }
        public string DiseaseName { get; set; }
        public bool IsWarning => CheckWarning();

        List<Index> _listIndex;

        public List<Index> ListIndex
        {
            get {
                var v = this["ListIndex"];
                return JArray.FromObject(v).ToObject<List<Index>>();
            }
            set => Push("ListIndex", value);
        }

        public void AddIndex(Index v)
        {
            _listIndex.Add(v);
        }
        public void RemoveIndex(Index v)
        {
            _listIndex.Remove(v);
        }

        public bool CheckWarning()
        {
            foreach(var v in ListIndex)
            {
                if (v.IsWarning) return true;
            }
            return false;
        }
    }

    public class Index : BaseModel
    {
        public string Name {
            get => GetString("Name");
            set => SetString("Name", value);
        }
        public double Value {
            get => GetValue<double>("Value");
            set => Push("Value", value);
        }
        public string Unit {
            get => GetString("Unit");
            set => SetString("Unit", value);
        }
        public bool IsWarning
        {
            get => GetValue<bool>("IsWarning");
            set => Push("IsWarning", value);
        }
        public DateTime LastMeasure { get; set; }

    }

    public class MedicalRecordCollection : DataMap<MedicalRecord>
    {
        DataContextFilterPath _filter;
        public DataContextFilterPath Filter
        {
            get
            {
                if (_filter == null)
                {
                    _filter = new DataContextFilterPath("Doctor", "Patient");
                    _filter.Start(this.Values, "Doctor");

                }
                return _filter;
            }
        }

    }
    partial class DB
    {
        static MedicalRecordCollection _medicalRecord;
        public static MedicalRecordCollection MedicalRecord
        {
            get
            {
                if (_medicalRecord == null)
                {
                    _medicalRecord = new MedicalRecordCollection();
                    _medicalRecord.Load();
                }
                return _medicalRecord;
            }
        }
    }


}
