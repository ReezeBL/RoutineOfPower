using System.Threading.Tasks;
using Loki.Common;
using Loki.Game;

namespace RoutineOfPower.Core.SkillHandlers.Decorators
{
    public class HasVaalSoulsDecorator : Decorator
    {
        public override bool ShouldUse(int slot)
        {
            var skill = LokiPoe.InGameState.SkillBarHud.Slot(slot);
            if (skill == null)
                return false;

            return skill.CurrentSouls >= skill.SoulsPerUse && handler.ShouldUse(slot);
        }

        public override async Task<bool> UseAt(int slot, Vector2i position, bool inPlace)
        {
            return await handler.UseAt(slot, position, inPlace);
        }
    }
}