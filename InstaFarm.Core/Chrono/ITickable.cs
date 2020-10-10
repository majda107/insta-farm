using System.Threading.Tasks;

namespace InstaFarm.Core.Chrono
{
    public interface ITickable
    {
        Task Tick(int interval);
    }
}