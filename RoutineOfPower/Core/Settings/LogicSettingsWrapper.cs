using RoutineOfPower.Core.LogicProviders;

namespace RoutineOfPower.Core.Settings
{
    public class LogicSettingsWrapper
    {
        public bool Enabled { get; set; }
        public int Priority { get; set; }

        public void SetFromHandler(ILogicHandler handler)
        {
            Enabled = handler.Enabled;
            Priority = handler.Priority;
        }

        public void SetToHandler(ILogicHandler handler)
        {
            handler.Enabled = Enabled;
            handler.Priority = Priority;
        }
    }
}