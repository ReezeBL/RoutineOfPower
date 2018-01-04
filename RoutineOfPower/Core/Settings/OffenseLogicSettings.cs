using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace RoutineOfPower.Core.Settings
{
    public class OffenseLogicSettings : LogicHandlerSettings
    {
        public OffenseLogicSettings(string handlerName) : base(handlerName)
        {
        }

        [DefaultValue(2)]
        public int Slot { get; set; }

        [DefaultValue(15)]
        public float Range { get; set; }

        [DefaultValue(true)]
        public bool AttackInPlace { get; set; }

        [JsonIgnore]
        public List<int> PossibleSlots { get; } = new List<int> {-1, 1, 2, 3, 4, 5, 6, 7, 8};
    }
}