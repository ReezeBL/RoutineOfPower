using System.Threading.Tasks;
using Loki.Common;
using Loki.Game;

namespace RoutineOfPower.Core.SkillHandlers.Decorators
{
    public class DontHaveAuraDecorator : Decorator
    {
        private readonly string auraName;

        public DontHaveAuraDecorator(string auraName)
        {
            this.auraName = auraName;
        }

        public override bool ShouldUse(int slot)
        {
            return !LokiPoe.Me.HasAura(auraName) && handler.ShouldUse(slot);
        }

        public override async Task<bool> UseAt(int slot, Vector2i position, bool inPlace)
        {
            return await handler.UseAt(slot, position, inPlace);
        }
    }
}