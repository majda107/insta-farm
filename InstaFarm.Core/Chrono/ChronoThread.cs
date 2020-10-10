using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace InstaFarm.Core.Chrono
{
    public static class ChronoThread
    {
        public const int TICK = 1000;


        public static event EventHandler OnTick;
        public static bool Running { get; private set; }
        public static List<ITickable> Tickables { get; private set; } = new List<ITickable>();

        private static Thread thread;

        public static void Register(ITickable tickable)
        {
            Tickables.Add(tickable);
            if (!Running)
            {
                Running = true;
                thread = new Thread(() =>
                {
                    while (Running)
                    {
                        OnTick?.Invoke(null, EventArgs.Empty);
                        foreach (var t in Tickables) t.Tick(TICK);
                        Thread.Sleep(TICK);
                    }
                });

                thread.Start();
            }
        }
    }
}