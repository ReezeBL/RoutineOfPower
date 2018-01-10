using System;
using System.Linq;
using System.Threading.Tasks;
using Loki.Common;
using Loki.Game.Objects;

namespace RoutineOfPower.Core.SkillHandlers.Decorators
{
    public class DeployedObjectsDecorator : Decorator
    {
        private readonly int maxCount;
        private readonly Func<NetworkObject, bool> filter;

        public DeployedObjectsDecorator(int maxCount, Func<NetworkObject, bool> filter = null)
        {
            this.maxCount = maxCount;
            this.filter = filter ?? (obj => true);
        }

        public override bool ShouldUse(int slot)
        {
            var skill = PoeHelpers.GetSkillFromSlot(slot);
            if (skill == null)
                return false;

            var deployedCount = skill.DeployedObjects.Count(filter);
            return deployedCount < maxCount && handler.ShouldUse(slot);
        }

        public override async Task<bool> UseAt(int slot, Vector2i position, bool inPlace)
        {
            return await handler.UseAt(slot, position, inPlace);
        }
    }
}