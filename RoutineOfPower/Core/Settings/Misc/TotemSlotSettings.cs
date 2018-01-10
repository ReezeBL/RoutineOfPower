using System.ComponentModel;
using Loki.Game.GameData;

namespace RoutineOfPower.Core.Settings.Misc
{
    public class TotemSlotSettings
    {
        public TotemCastType Type { get; set; } = TotemCastType.Offensive;
        public int Slot { get; set; }
        public string Name { get; set; }

        public int Timeout { get; set; } = 4000;
        public int MaxTotemCount { get; set; } = 1;
        public float Range { get; set; } = 10f;
        public Rarity MinMonsterRarity { get; set; } = Rarity.Rare;
    }

    public enum TotemCastType
    {
        [Description("Totem support mode")]
        Support,
        [Description("Totem offensive mode")]
        Offensive
    }
}