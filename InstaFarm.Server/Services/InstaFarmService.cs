using InstaFarm.Core.Models;

namespace InstaFarm.Server.Services
{
    public class InstaFarmService
    {
        public InstaFarm.Core.InstaFarm Farm { get; set; }
        public InstaFarmUser Selected { get; set; }

        public InstaFarmService()
        {
            this.Farm = new InstaFarm.Core.InstaFarm();
        }
    }
}