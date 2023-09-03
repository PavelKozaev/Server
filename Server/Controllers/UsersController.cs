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
            Thread.Sleep(5);
            return new[]
            {
                new User("Pavel", "Kozaev", "North"),
                new User("Pavel", "Kozaev", "South"),

            };             
        }

        public async Task<User[]> IndexAsync()
        {
            await Task.Delay(5);
            return new[]
            {
                new User("Pavel", "Kozaev", "North"),
                new User("Pavel", "Kozaev", "South"),

            };
        }
    }
}
