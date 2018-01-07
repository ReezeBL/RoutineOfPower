using System.Collections.Generic;
using System.ComponentModel;
using Loki.Game.GameData;
using Newtonsoft.Json;

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

        [JsonIgnore] public static readonly TriggerType[] TriggerTypes = {TriggerType.Hp, TriggerType.Es, TriggerType.Monsters};

        [JsonIgnore] public static readonly Rarity[] Rarities = {Rarity.Normal, Rarity.Magic, Rarity.Rare, Rarity.Unique};
    }
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

    