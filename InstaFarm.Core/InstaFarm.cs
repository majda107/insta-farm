using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InstaFarm.Core.Models;
using Newtonsoft.Json.Serialization;

namespace InstaFarm.Core
{
    public class InstaFarm
    {
        public List<InstaFarmUser> Users { get; set; }

        public InstaFarm()
        {
            this.Users = new List<InstaFarmUser>();
        }


        public InstaFarmUser Add(string username, string password)
        {
            var user = new InstaFarmUser(username, password);
            this.Users.Add(user);

            return user;
        }


        public async Task LoginAll()
        {
            await Task.WhenAll(this.Users.Select(u => u.LoginAsync()));
        }
    }
}