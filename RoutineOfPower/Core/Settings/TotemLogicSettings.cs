using System.Collections.ObjectModel;
using Loki.Game;
using Loki.Game.GameData;
using Newtonsoft.Json;
using RoutineOfPower.Core.Settings.Misc;

namespace RoutineOfPower.Core.Settings
{
    public class TotemLogicSettings : LogicHandlerSettings
    {
        public TotemLogicSettings() : base(nameof(TotemLogicSettings))
        {
        }

        public int PlaceRange { get; set; } = 60;
        public ObservableCollection<TotemSlotSettings> TotemSettings { get; set; } = new ObservableCollection<TotemSlotSettings>();

        [JsonIgnore] public static readonly Rarity[] Rarities = {Rarity.Normal, Rarity.Magic, Rarity.Rare, Rarity.Unique};
        [JsonIgnore] public static readonly TotemCastType[] CastTypes = {TotemCastType.Support, TotemCastType.Offensive};
        [JsonIgnore] public static readonly int[] Slots = {1, 2, 3, 4, 5, 6, 7, 8};

        public void CreateSettingsForSlots()
        {
            if(!LokiPoe.IsInGame)
                return;
            
            TotemSettings.Clear();

            foreach (var skill in PoeHelpers.GetSkillbarSkills(skill => skill.IsTotem && !skill.IsTrap && !skill.IsMine))
            {
                var skillSettings = new TotemSlotSettings { Name = skill.Name, Slot = skill.Slot};
                TotemSettings.Add(skillSettings);
            }

        }
    }
}