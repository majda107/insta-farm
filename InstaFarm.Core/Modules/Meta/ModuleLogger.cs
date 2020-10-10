namespace InstaFarm.Core.Modules.Meta
{
    public class ModuleLogger
    {
        public delegate void LoggerEventHandler(ModuleLogger o, string m);

        public event LoggerEventHandler OnValue;

        public void Log(string message)
        {
            this.OnValue?.Invoke(this, message);
        }
    }
}