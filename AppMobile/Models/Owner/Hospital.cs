using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Hospital : BaseModel
    {
        public override string GetObjectIdName() => "HospitalID";
        public string HospitalID => ObjectId;

        public string Name { get; set; }
        public Address Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
    public class Address
    {
        public string City { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Ward { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;

        public override string ToString()
        {
            return String.Format("{0}, Phường {1}, Quận {2}, Thành phố {3}", Details, Ward, District, City);
        }
    }
}

