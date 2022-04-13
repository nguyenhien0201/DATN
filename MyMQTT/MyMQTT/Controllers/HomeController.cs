using System;
using System.Collections.Generic;
using System.Text;
using System.Mvc;

namespace MyMQTT.Controllers
{
    class HomeController : BaseController
    {
        public object Start()
        {
            Screen.Warning("Connecting to server");
            if (Client.IsConnected) { }
            return GoFirst();
        }
        public object Login()
        {
            return View();
        }
    }
}
