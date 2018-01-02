using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using Loki.Bot;
using Loki.Game.Objects;
using RoutineOfPower.Core.Settings;
using RoutineOfPower.GUI;

namespace RoutineOfPower.Core.LogicProviders
{
    public class AoeTargetLogic : OffenseLogic
    {
        private readonly AoeTargetLogicSettings settings = new AoeTargetLogicSettings(nameof(AoeTargetLogic));
        private UserControl interfaceControl;

        protected override OffenseLogicSettings Settings => settings;

        public override string Name { get; set; } = "Aoe Target Skill";

        public override UserControl InterfaceControl => interfaceControl;


        public override void CreateInterfaceControl()
        {
            interfaceControl = new AoeTargetGui(settings);
        }

        public override async Task<LogicResult> OutCombatHandling()
        {
            return await Task.FromResult(LogicResult.Unprovided);
        }

        public override async Task<LogicResult> CombatHandling(IList<Monster> targets)
        {
            var bestTarget = targets.FirstOrDefault();
            if (bestTarget == null)
            {
                await Coroutines.FinishCurrentAction();
                return LogicResult.Unprovided;
            }

            var bestTargetPosition = bestTarget.Position;
            var numberOfMonsters =
                targets.Count(monster => bestTargetPosition.Distance(monster.Position) <= settings.AoeRadius);

            if (numberOfMonsters < settings.MinMonstersToAoe)
                return LogicResult.Unprovided;

            if (settings.UseMaxRarity && bestTarget.Rarity >= settings.MaxRarity)
                return LogicResult.Unprovided;

            var result = await ProccessTarget(bestTarget);
            return result ? LogicResult.Provided : LogicResult.Unprovided;
        }
    }
}