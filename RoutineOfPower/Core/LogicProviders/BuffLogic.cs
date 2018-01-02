using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using Loki.Bot;
using Loki.Game;
using Loki.Game.Objects;
using RoutineOfPower.Core.SkillHandlers;

namespace RoutineOfPower.Core.LogicProviders
{
    public class BuffLogic : ILogicHandler
    {
        private static readonly HashSet<string> BuffNames = new HashSet<string> {"Blood Rage", "Tempest Shield"};

        private readonly List<SkillWrapper> buffSlots = new List<SkillWrapper>(8);

        public MessageResult Message(Message message)
        {
            return MessageResult.Unprocessed;
        }

        public bool Enabled { get; set; } = true;
        public string Name { get; set; } = "Buffer";
        public int Priority { get; set; } = 1;

        public UserControl InterfaceControl { get; } = null;

        public void CreateInterfaceControl()
        {
        }

        public void Start()
        {
            buffSlots.Clear();
            foreach (var skill in PoeHelpers.GetSkillBarSkill(skill => BuffNames.Contains(skill.Name)))
                buffSlots.Add(new SkillWrapper(skill.Slot, new AuraCastHandler(skill.Name)));
        }

        public async Task<LogicResult> OutCombatHandling()
        {
            return await Task.FromResult(LogicResult.Unprovided);
        }

        public async Task<LogicResult> CombatHandling(IList<Monster> targets)
        {
            foreach (var slot in buffSlots)
            {
                if (await slot.Use())
                    return LogicResult.Provided;
            }

            return LogicResult.Unprovided;
        }
    }
}