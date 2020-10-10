using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using InstaFarm.Core.Chrono;
using InstaFarm.Core.Core;
using InstaFarm.Core.Modules;
using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.ResponseWrappers.Business;
using InstagramApiSharp.Helpers;

namespace InstaFarm.Core.Models
{
    public class InstaFarmUser
    {
        public delegate void UserEventHandler(InstaFarmUser s, InstaFarmEventArgs e);

        public event UserEventHandler OnLogin;


        public readonly string Username;


        private string password;

        public string Password
        {
            get => this.password;
            set
            {
                this.password = value;
                this.Api = InstaApiBuilder.CreateBuilder().SetUser(new UserSessionData()
                {
                    UserName = this.Username,
                    Password = this.Password
                }).Build();
            }
        }

        public IInstaApi Api { get; set; }


        private bool loggedIn;

        public bool LoggedIn
        {
            get => this.loggedIn;
            set
            {
                this.loggedIn = value;
                this.OnLogin?.Invoke(this, new InstaFarmEventArgs());
            }
        }

        public IList<ModuleBase> Modules { get; set; }
        public InstaFarmTimer Timer { get; set; }

        public InstaFarmUser(string username, string password)
        {
            this.Username = username;
            this.Password = password;

            this.Modules = new List<ModuleBase>();
            this.Timer = new InstaFarmTimer(60); // 60 second timer
        }

        public void AddModule(ModuleBase module)
        {
            this.Modules.Add(module);
        }


        public async Task<bool> LoginAsync()
        {
            if (this.LoggedIn) return false;

            var res = await this?.Api?.LoginAsync() ?? null;
            this.LoggedIn = res?.Succeeded ?? false;

            return this.LoggedIn;
        }

        public async Task<bool> LogoutAsync()
        {
            if (!this.LoggedIn) return false;

            var res = await this?.Api?.LogoutAsync() ?? null;
            if (res?.Succeeded ?? false) this.LoggedIn = false;

            return this.LoggedIn;
        }

        public async Task<int> FetchFollowersCountAsync()
        {
            if (!this.LoggedIn) return -1;

            var res = await this.Api.UserProcessor.GetUserFollowersAsync(this.Username, PaginationParameters.Empty);
            return res?.Value?.Count ?? -1;
        }
    }
}