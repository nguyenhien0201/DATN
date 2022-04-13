using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace MyMQTT.Controllers
{
    class UserController : BaseController
    {
        public void Info()
        {
            //var u = new Models.User
            //{
            //    Name = "Lê Quang Huy",
            //    Gender = "Nam",
            //    DateOfBirth = new DateTime(1999, 2, 15),
            //    PhoneNumber = "0898991502",
            //    Email = "h@gmail.com",
            //};
            var token = Request.GetString("#token");
            var u = DB.UserCollection[token] as Models.User;
            
            if(u.AuthorName == "Doctor")
            {
                u = DB.Doctor.Find(u.UserName);
            }
            else if(u.AuthorName == "Patient")
            {
                u = DB.Patient.Find(u.UserName);
            }

            var res = new DataContext();
            res.Push("Code", 0);
            res.Push("Value", u);
            Response(res);
        }
    }
}
