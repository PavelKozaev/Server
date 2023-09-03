using Server.ItSelf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Controllers
{
    public record User(string Name, string Surname, string Login);
    public class UsersController : IController
    {
        public User[] Index()
        {
            return new[]
            {
                new User("Pavel", "Kozaev", "North"),
                new User("Pavel", "Kozaev", "South"),

            };             
        } 
    }
}
