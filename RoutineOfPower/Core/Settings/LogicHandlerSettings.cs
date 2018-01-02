using Loki;
using Loki.Common;

namespace RoutineOfPower.Core.Settings
{
    public abstract class LogicHandlerSettings : JsonSettings
    {
        protected LogicHandlerSettings(string handlerName) : base(GetSettingsFilePath(Configuration.Instance.Name,
            "RoutineOfPower", $"{handlerName}.json"))
        {
        }
    }
}