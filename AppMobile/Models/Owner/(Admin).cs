using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Admin : User
    {
        public override string GetObjectIdName() => UserName;
        public object CreateUser(DataContext context)
        {
            return CreateMQTTResponse(Account.CreateAccount(context), null, null);
        }
    }
    public class AdminCollection : DataMap<Admin>
    {

    }
    partial class DB
    {
        static AdminCollection _admin;
        public static AdminCollection Admin
        {
            get
            {
                if (_admin == null)
                {
                    _admin = new AdminCollection();
                    _admin.Load();
                }
                return _admin;
            }
        }
    }
}
