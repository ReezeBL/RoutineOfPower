using System.Threading.Tasks;
using Loki.Common;
using Loki.Game;

namespace RoutineOfPower.Core.SkillHandlers
{
    public class SkillWrapper
    {
        private readonly int slot;
        private readonly SkillHandler handler;

        public SkillWrapper(int slot, SkillHandler handler)
        {
            this.slot = slot;
            this.handler = handler;
        }

        public async Task<bool> UseAt(Vector2i position, bool inPlace)
        {
            return await handler.HandleSkillAt(slot, position, inPlace);
        }

        public async Task<bool> Use()
        {
            return await handler.HandleSkillAt(slot, LokiPoe.MyPosition, true);
        }

        public bool CanUse()
        {
            return handler.ShouldUse(slot);
        }
    }
}