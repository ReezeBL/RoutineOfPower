using System.Threading.Tasks;
using Loki.Bot;
using Loki.Common;
using Loki.Game;

namespace RoutineOfPower.Core.SkillHandlers
{
    public class SingleCastHandler : SkillHandler
    {
        public override async Task<bool> UseAt(int slot, Vector2i position, bool inPlace)
        {
            await Coroutines.FinishCurrentAction();
            var castResult = InternalUse(LokiPoe.InGameState.SkillBarHud.UseAt, slot, inPlace, position);

            if (!castResult) return false;

            await Coroutines.FinishCurrentAction(false);
            return true;
        }
    }
}