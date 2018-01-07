using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Loki.Game;
using Loki.Game.GameData;
using RoutineOfPower.Core.Settings.Misc;

namespace RoutineOfPower.Core.Settings
{
    public class BuffLogicSettings : LogicHandlerSettings
    {
        public List<VaalAuraInfo> VaalAuraInfos { get; set; } = new List<VaalAuraInfo>();

        public BuffLogicSettings() : base(nameof(BuffLogicSettings))
        {
            if (VaalAuraInfos.Count > 0) return;

            VaalAuraInfos.Add(new VaalAuraInfo
            {
                Name = "Vaal Discipline",
                Triggers = new ObservableCollection<TriggerSettings>
                {
                    new TriggerSettings {Type = TriggerType.Es, MyEsPercent = 60}
                }
            });

            VaalAuraInfos.Add(new VaalAuraInfo
            {
                Name = "Vaal Grace",
                Triggers = new ObservableCollection<TriggerSettings>
                {
                    new TriggerSettings {Type = TriggerType.Monsters, MobCount = 1, MobRarity = Rarity.Rare},
                    new TriggerSettings {Type = TriggerType.Monsters, MobCount = 1, MobRarity = Rarity.Unique}
                }
            });

            VaalAuraInfos.Add(new VaalAuraInfo
            {
                Name = "Vaal Haste",
                Triggers = new ObservableCollection<TriggerSettings>
                {
                    new TriggerSettings() {Type = TriggerType.Monsters, MobCount = 1, MobRarity = Rarity.Unique}
                }
            });
        }

        public Func<int, bool> GetBuffCondition(string name)
        {
            var buff = VaalAuraInfos.FirstOrDefault(aura => aura.Name == name);
            if (buff == null)
                return slot => false;
            var condition = buff.Triggers.Select(GetPredicateFromTrigger).Aggregate((func1, func2) => slot => func1(slot) || func2(slot)); //God, save me
            return condition;
        }

        private static Func<int, bool> GetPredicateFromTrigger(TriggerSettings trigger)
        {
            switch (trigger.Type)
            {
                case TriggerType.Hp:
                    return slot => LokiPoe.Me.HealthPercentTotal < trigger.MyHpPercent;
                case TriggerType.Es:
                    return slot => LokiPoe.Me.EnergyShieldPercent < trigger.MyEsPercent;
                case TriggerType.Monsters:
                    return slot =>
                        PoeHelpers.NumberOfHostileMonstersNear(LokiPoe.Me, trigger.MobRange, trigger.MobRarity) >= trigger.MobCount;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}