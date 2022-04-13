using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace MyMQTT
{
    class Program
    {
        static void Main(string[] args)
        {
            DB.Mapping(new DataBase(Environment.CurrentDirectory, "MainDB"));
            DB.UserCollection.RegisterActors(
                typeof(Admin),
                typeof(Doctor),
                typeof(Patient),
                typeof(User)
                );
            System.Mvc.Engine.Register(new Program(), result => { });
            System.Mvc.Engine.Execute("home/start");

            while (true) { }
        }
    }
}
