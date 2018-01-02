using System.Threading.Tasks;
using Loki.Bot;
using Loki.Common;
using Loki.Game;

namespace RoutineOfPower.Core.SkillHandlers
{
    public class SingleCastHandler : SkillHandler
    {
        protected override async Task<bool> UseAt(int slot, Vector2i position, bool inPlace)
        {
            var castResult = InternalUse(LokiPoe.InGameState.SkillBarHud.UseAt, slot, inPlace, position);
            if (castResult)
            {
                await Coroutines.LatencyWait();
                await Coroutines.FinishCurrentAction(false);
                await Coroutines.LatencyWait();
                return true;
            }
            return false;
        }
    }
}