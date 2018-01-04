using System.ComponentModel;

namespace RoutineOfPower.Core.Settings
{
    public class GolemSummonerSettings : LogicHandlerSettings
    {
        public GolemSummonerSettings() : base(nameof(GolemSummonerSettings))
        {
        }

        [DefaultValue(false)]
        public bool SummonInCombat { get; set; }
    }
}