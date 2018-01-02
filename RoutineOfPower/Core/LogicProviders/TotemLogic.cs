using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using Loki.Bot;
using Loki.Bot.Pathfinding;
using Loki.Game;
using Loki.Game.Objects;
using RoutineOfPower.Core.SkillHandlers;

namespace RoutineOfPower.Core.LogicProviders
{
    public class TotemLogic : ILogicHandler
    {
        public MessageResult Message(Message message)
        {
            return MessageResult.Unprocessed;
        }

        public bool Enabled { get; set; } = true;
        public string Name { get; set; } = "Totems Placement";
        public int Priority { get; set; } = 1;

        public UserControl InterfaceControl { get; } = null;

        private SkillWrapper totemSlot;
        private const int MaxTotems = 1;

        private static bool ShouldPlaceTotem(int slot)
        {
            var skill = LokiPoe.InGameState.SkillBarHud.Slot(slot);
            return skill.DeployedObjects.Select(o => (Monster) o).Count(t => !t.IsDead && t.Distance < 60) < MaxTotems;
        }

        public void CreateInterfaceControl()
        {
        }

        public void Start()
        {
            totemSlot = null;
            var totemSkill = PoeHelpers.GetSkillBarSkill(skill => skill.IsTotem && !skill.IsTrap && !skill.IsMine).FirstOrDefault();
            if (totemSkill != null)
            {
                totemSlot = new SkillWrapper(totemSkill.Slot, new TimeoutSkillHandler(4000, ShouldPlaceTotem));
            }
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

            var distance = LokiPoe.MyPosition.Distance(target.Position);

            if (!ExilePather.CanObjectSee(LokiPoe.Me, target.Position))
                return LogicResult.Unprovided;

            var cachedPosition = LokiPoe.MyPosition.GetPointAtDistanceAfterThis(target.Position, distance / 2f);

            if (await totemSlot.UseAt(cachedPosition, true))
                return LogicResult.Provided;

            return LogicResult.Unprovided;
        }
    }
}