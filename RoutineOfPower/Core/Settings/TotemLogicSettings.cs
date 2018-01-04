using System.ComponentModel;

namespace RoutineOfPower.Core.Settings
{
    public class TotemLogicSettings : LogicHandlerSettings
    {
        public TotemLogicSettings() : base(nameof(TotemLogicSettings))
        {
        }

        [DefaultValue(true)]
        public bool UseAsSupport { get; set; }
    }
}