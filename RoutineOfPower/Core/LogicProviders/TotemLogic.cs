using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using Loki.Bot;
using Loki.Bot.Pathfinding;
using Loki.Game;
using Loki.Game.Objects;
using RoutineOfPower.Core.Settings;
using RoutineOfPower.Core.SkillHandlers;
using RoutineOfPower.Core.SkillHandlers.Decorators;

namespace RoutineOfPower.Core.LogicProviders
{
    public class TotemLogic : ILogicHandler
    {
        private const int MaxTotems = 1;
        private readonly TotemLogicSettings settings = new TotemLogicSettings();

        private SkillWrapper totemSlot;

        public MessageResult Message(Message message)
        {
            return MessageResult.Unprocessed;
        }

        public bool Enabled { get; set; } = true;
        public string Name { get; set; } = "Totems Placement";
        public int Priority { get; set; } = 1;

        public UserControl InterfaceControl { get; } = null;

        public void CreateInterfaceControl()
        {
        }

        public void Start()
        {
            totemSlot = null;
            var totemSkill = PoeHelpers.GetSkillbarSkills(skill => skill.IsTotem && !skill.IsTrap && !skill.IsMine)
                .FirstOrDefault();
            if (totemSkill != null)
                totemSlot = new SkillWrapper(totemSkill.Slot,
                    new SingleCastHandler()
                        .AddDecorator(new TimeoutDecorator(4000))
                        .AddDecorator(new ConditionalDecorator(ShouldPlaceTotem)));
        }

        public async Task<LogicResult> OutCombatHandling()
        {
            return await Task.FromResult(LogicResult.Unprovided);
        }

        public async Task<LogicResult> CombatHandling(IList<Monster> targets)
        {
            if (totemSlot == null || !totemSlot.CanUse())
                return LogicResult.Unprovided;

            var target = targets.FirstOrDefault();
            if (target == null)
                return LogicResult.Unprovided;

            if (settings.UseAsSupport)
                return await HandleSupportLogic(targets);
            
            return await HandleOffensiveLogic(target);
        }

        private static bool ShouldPlaceTotem(int slot)
        {
            var skill = LokiPoe.InGameState.SkillBarHud.Slot(slot);
            return skill.DeployedObjects.Select(o => (Monster) o).Count(t => !t.IsDead && t.Distance < 60) < MaxTotems;
        }

        private async Task<LogicResult> HandleOffensiveLogic(NetworkObject target)
        {
            var distance = LokiPoe.MyPosition.Distance(target.Position);

            if (!ExilePather.CanObjectSee(LokiPoe.Me, target.Position))
                return LogicResult.Unprovided;

            var cachedPosition = LokiPoe.MyPosition.GetPointAtDistanceAfterThis(target.Position, distance / 2f);

            if (await totemSlot.UseAt(cachedPosition, true))
                return LogicResult.Provided;

            return LogicResult.Unprovided;
        }

        private async Task<LogicResult> HandleSupportLogic(IList<Monster> targets)
        {
            var tooDangerous = PoeHelpers.HasDangerousNeighbours(LokiPoe.MyPosition, targets);
            if (tooDangerous)
                return LogicResult.Unprovided;

            var bestTarget = targets.First();

            var position = LokiPoe.MyPosition;
            var distance = position.Distance(bestTarget.Position);

            if (ExilePather.CanObjectSee(LokiPoe.Me, bestTarget))
                position = LokiPoe.MyPosition.GetPointAtDistanceAfterThis(bestTarget.Position, distance / 2f);

            if (await totemSlot.UseAt(position, true))
                return LogicResult.Provided;

            return LogicResult.Unprovided;
        }
    }
}