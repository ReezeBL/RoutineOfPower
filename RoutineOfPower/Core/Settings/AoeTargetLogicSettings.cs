using System.Collections.Generic;
using System.ComponentModel;
using Loki.Game.GameData;
using Newtonsoft.Json;

namespace RoutineOfPower.Core.Settings
{
    public class AoeTargetLogicSettings : OffenseLogicSettings
    {
        public AoeTargetLogicSettings(string handlerName) : base(handlerName)
        {
        }

        [DefaultValue(true)]
        public bool UseMaxRarity { get; set; }

        [DefaultValue(Rarity.Rare)]
        public Rarity MaxRarity { get; set; }

        [DefaultValue(15f)]
        public float AoeRadius { get; set; }

        [DefaultValue(3)]
        public int MinMonstersToAoe { get; set; }

        [JsonIgnore]
        public List<Rarity> PossibleRarities { get; } = new List<Rarity>{Rarity.Normal, Rarity.Magic, Rarity.Rare, Rarity.Unique};
    }
}