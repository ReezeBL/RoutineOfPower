using System.Collections.Generic;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Loki.Common;
using Loki.Game;
using Loki.Game.Objects;

namespace RoutineOfPower.Core.SkillHandlers
{
    public class SkillWrapper
    {
        private readonly SkillHandler handler;
        private readonly int slot;
        private readonly Dictionary<string, object> parameters = new Dictionary<string, object>();

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

        public Skill Skill => PoeHelpers.GetSkillFromSlot(slot);

        public bool CanUse()
        {
            return handler.ShouldUse(slot);
        }

        public void AddParameter(string name, object parameter)
        {
            parameters[name] = parameter;
        }

        public T GetParameter<T>(string name)
        {
            if (!parameters.TryGetValue(name, out var parameter))
                return default(T);
            return (T)parameter;
        }
    }
}