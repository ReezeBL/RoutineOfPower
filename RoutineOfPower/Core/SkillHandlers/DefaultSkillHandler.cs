using System.Threading.Tasks;
using Loki.Bot;
using Loki.Common;
using Loki.Game;

namespace RoutineOfPower.Core.SkillHandlers
{
    public class DefaultSkillHandler : SkillHandler
    {
        public override async Task<bool> UseAt(int slot, Vector2i position, bool inPlace)
        {
            var skill = ValidateSlot(slot);
            if (skill == null)
                return false;

            await Coroutines.FinishCurrentAction();
            return InternalUse(LokiPoe.InGameState.SkillBarHud.BeginUseAt, slot, inPlace, position);
        }
    }
}