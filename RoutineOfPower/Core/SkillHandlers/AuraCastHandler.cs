using System.Threading.Tasks;
using Loki.Bot;
using Loki.Common;
using Loki.Game;
using Loki.Game.Objects;

namespace RoutineOfPower.Core.SkillHandlers
{
    public class AuraCastHandler : SingleCastHandler
    {
        private readonly string auraName;
        public override bool ShouldUse(int slot)
        {
            return !LokiPoe.Me.HasAura(auraName) && base.ShouldUse(slot);
        }

        public AuraCastHandler(string auraName)
        {
            this.auraName = auraName;
        }
    }
}