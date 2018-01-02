using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Loki.Common;

namespace RoutineOfPower.Core.SkillHandlers
{
    public class TimeoutSkillHandler : SingleCastHandler
    {
        private readonly int timeout;
        private readonly Func<int, bool> condition;

        private readonly Stopwatch timer = new Stopwatch();

        public TimeoutSkillHandler(int timeout, Func<int, bool> condition = null)
        {
            this.timeout = timeout;
            this.condition = condition ?? (slot => true);
            timer.Start();
        }

        public override bool ShouldUse(int slot)
        {
            return base.ShouldUse(slot) && timer.ElapsedMilliseconds >= timeout && condition(slot);
        }

        protected override async Task<bool> UseAt(int slot, Vector2i position, bool inPlace)
        {
            var casted = await base.UseAt(slot, position, inPlace);
            if(casted)
                timer.Restart();
            return casted;
        }
    }
}