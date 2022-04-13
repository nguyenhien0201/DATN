using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMQTT.Controllers
{
    class DoctorController : BaseController
    {
        public void PatientList()
        {
            //var Token = Request.GetString("#token");

            //var UserTopic = _topic + '/' + Token;
            //Publish(UserTopic, "warning", new object());

            var res = Excute(Request);
            Response(res);
        }
    }
}
