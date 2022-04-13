using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class User : Account
    {
        public string Name
        {
            get => GetString("Name");
            set => SetString("Name", value);
        }
        public string Gender
        {
            get => GetString("Gender");
            set => SetString("Gender", value);
        }
        public DateTime DateOfBirth
        {
            get => GetValue<DateTime>("DateOfBirth");
            set => Push("DateOfBirth", value);
        }
        public string PhoneNumber
        {
            get => GetString("PhoneNumber");
            set => SetString("PhoneNumber", value);
        }
        public string Email
        {
            get => GetString("Email");
            set => SetString("Email", value);
        }
        public string Token
        {
            get => GetString("Token");
            set => SetString("Token", value);
        }
        public int Age
        {
            get
            {
                return DateTime.Now.Year - DateOfBirth.Year;
            }
        }

        public object ChangePassword(DataContext context)
        {
            var un = context.GetString("UserName").ToLower();
            var ps = un.JoinMD5(context.GetString("Password")).ToMD5();

            Password = ps;

            DB.Accounts.FindAndUpdate<Account>(un, acc => acc.Password = ps);
            return Ok(null);
        }
        public object Logout(DataContext context)
        {
            var token = context.GetString("#token");
            if (token != null)
            {
                DB.UserCollection.Remove(token);
            }
            return Ok(null);
        }
    }
    public class UserCollection : DataContext
    {
        DataContext _actors = new DataContext();
        public void RegisterActors(params Type[] actorTypes)
        {
            foreach (var type in actorTypes)
            {
                _actors.Add(type.Name, type);
            }
        }
        public Type GetActor(string name)
        {
            return (Type)_actors[name];
        }
        public object CreateUser(Account acc)
        {
            var time = DateTime.Now;
            var token = acc.UserName.JoinMD5(time.Ticks);

            var authorname = acc.GetString("AuthorName");
            var user = Activator.CreateInstance(GetActor(authorname)) as User;
            user.Copy(acc);
            user.Push("#login-time", time);
            user.Token = token;

            Add(token, user);

            acc.Push("Token", token);
            return acc;
        }

    }
    partial class DB
    {
        public static UserCollection UserCollection { get; private set; } = new UserCollection();
    }
}



