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
    internal class SingleTargetLogic : OffenseLogic
    {
        private UserControl interfaceControl;

        public SingleTargetLogic()
        {
            //To ensure that it will be handled after all
            Priority = -1;
        }

        protected override OffenseLogicSettings Settings { get; } = new OffenseLogicSettings(nameof(SingleTargetLogic));

        public override string Name { get; set; } = "Single Target Skill";

        public override UserControl InterfaceControl => interfaceControl;

        public override void CreateInterfaceControl()
        {
            interfaceControl = new SingleTargetGui(Settings);
        }

        public override async Task<LogicResult> OutCombatHandling()
        {
            return await Task.FromResult(LogicResult.Unprovided);
        }

        public override async Task<LogicResult> CombatHandling(IList<Monster> targets)
        {
            var target = targets.FirstOrDefault();
            if (target == null)
            {
                await Coroutines.FinishCurrentAction();
                return LogicResult.Unprovided;
            }
            var result = await ProccessTarget(target);

            return result ? LogicResult.Provided : LogicResult.Unprovided;
        }
    }
}