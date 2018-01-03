using System;
using System.Threading.Tasks;
using Loki.Common;

namespace RoutineOfPower.Core.SkillHandlers.Decorators
{
    public class ConditionalDecorator : Decorator
    {
        private readonly Func<int, bool> condition;

        public ConditionalDecorator(Func<int, bool> condition)
        {
            this.condition = condition;
        }

        public override bool ShouldUse(int slot)
        {
            return condition(slot) && handler.ShouldUse(slot);
        }

        public override async Task<bool> UseAt(int slot, Vector2i position, bool inPlace)
        {
            return await handler.UseAt(slot, position, inPlace);
        }
    }
}