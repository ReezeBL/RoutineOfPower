using System.ComponentModel;
using Loki.Game.GameData;

namespace RoutineOfPower.Core.Settings.Misc
{
    public class TriggerSettings
    {
        public TriggerType Type { get; set; } = TriggerType.Hp;
        public int MyHpPercent { get; set; } = 75;
        public int MyEsPercent { get; set; } = 75;
        public Rarity MobRarity { get; set; } = Rarity.Normal;
        public int MobCount { get; set; } = 1;
        public int MobRange { get; set; } = 40;
        public int MobHpPercent { get; set; } = 0;
    }

    public enum TriggerType
    {
        [Description("HP percent")]
        Hp,
        [Description("ES percent")]
        Es,
        [Description("Number of monsters nearby")]
        Monsters
    }
}