using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace MyMQTT.Controllers
{
    class AccountController : BaseController
    {
        public void login()
        {
            var acc = Json.Convert<Account>(Request);
            var res = acc.Login();
            Response(res);
        }
        public void logout()
        {
            var res = new User().Logout(Request);
            Response(res);
        }
        public void changepassword()
        {
            var res = new User().ChangePassword(Request);
            Response(res);
        }
    }
}
