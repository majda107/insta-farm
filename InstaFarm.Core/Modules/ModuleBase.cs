using System;
using System.Dynamic;
using System.Threading.Tasks;
using InstaFarm.Core.Models;
using InstaFarm.Core.Modules.Meta;

namespace InstaFarm.Core.Modules
{
    public abstract class ModuleBase
    {
        public readonly InstaFarmUser User;
        public ModuleLogger Logger { get; private set; }

        public ModuleBase(InstaFarmUser user)
        {
            this.User = user;
            this.Logger = new ModuleLogger();
        }

        public abstract Task Execute();

        public async Task TryExecute()
        {
            try
            {
                await this.Execute();
            }
            catch (Exception e)
            {
                this.Logger.Log(e.Message);
            }
        }
    }
}