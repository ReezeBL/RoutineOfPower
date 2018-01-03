using System.Diagnostics;
using System.Threading.Tasks;
using Loki.Common;

namespace RoutineOfPower.Core.SkillHandlers.Decorators
{
    public class TimeoutDecorator : Decorator
    {
        private readonly int timeout;
        private readonly Stopwatch timer;

        public TimeoutDecorator(int timeout)
        {
            this.timeout = timeout;
            timer = new Stopwatch();
            timer.Start();
        }

        public override bool ShouldUse(int slot)
        {
            return timer.ElapsedMilliseconds > timeout && handler.ShouldUse(slot);
        }

        public override async Task<bool> UseAt(int slot, Vector2i position, bool inPlace)
        {
            var casted = await handler.UseAt(slot, position, inPlace);
            if(casted)
                timer.Restart();
            return casted;
        }
    }
}