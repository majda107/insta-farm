using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using InstaFarm.Core.Modules;

namespace InstaFarm.Core.Chrono
{
    public class InstaFarmTimer : ITickable
    {
        public double Interval { get; set; }
        public double Left { get; private set; }
        public IList<ModuleBase> Modules { get; set; }


        public bool Running { get; private set; }

        public InstaFarmTimer(double interval)
        {
            this.Interval = interval;
            this.Modules = new List<ModuleBase>();

            this.Left = this.Interval;

            ChronoThread.Register(this);
        }


        public void Start() => this.Running = true;
        public void Stop() => this.Running = false;

        public void Reset()
        {
            this.Left = this.Interval;
        }

        public async Task Tick(int interval)
        {
            if (!Running) return;

            this.Left -= (interval / 1000d);
            if (this.Left <= 0)
            {
                this.Reset();
                foreach (var module in Modules) await module.TryExecute();
            }
        }
    }
}