using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using Loki.Bot;
using Loki.Game;
using Loki.Game.Objects;
using RoutineOfPower.Core.Settings;
using RoutineOfPower.Core.SkillHandlers;
using RoutineOfPower.Core.SkillHandlers.Decorators;
using RoutineOfPower.GUI;

namespace RoutineOfPower.Core.LogicProviders
{
    public class BuffLogic : ILogicHandler
    {
        private static readonly HashSet<string> BuffNames = new HashSet<string> {"Blood Rage", "Tempest Shield"};

        private readonly List<SkillWrapper> buffSlots = new List<SkillWrapper>(8);
        private readonly BuffLogicSettings settings = new BuffLogicSettings();

        public MessageResult Message(Message message)
        {
            return MessageResult.Unprocessed;
        }

        public bool Enabled { get; set; } = true;
        public string Name { get; set; } = "Buffer";
        public int Priority { get; set; } = 1;

        public UserControl InterfaceControl { get; private set; }

        public void CreateInterfaceControl()
        {
            InterfaceControl = new BuffLogicGui(settings);
        }

        public void Start()
        {
            buffSlots.Clear();
            foreach (var skill in PoeHelpers.GetSkillbarSkills())
            {
                if (BuffNames.Contains(skill.Name))
                    buffSlots.Add(new SkillWrapper(skill.Slot,
                        new SingleCastHandler().AddDecorator(new DontHaveAuraDecorator(skill.Name))));

                if (skill.IsVaalSkill)
                {
                    var predicate = settings.GetBuffCondition(skill.Name);
                    buffSlots.Add(new SkillWrapper(skill.Slot, 
                        new SingleCastHandler()
                            .AddDecorator(new DontHaveAuraDecorator(skill.Name))
                            .AddDecorator(new ConditionalDecorator(predicate))
                            .AddDecorator(new HasVaalSoulsDecorator())
                        ));
                }
            }
        }

        public async Task<LogicResult> OutCombatHandling()
        {
            return await Task.FromResult(LogicResult.Unprovided);
        }

        public async Task<LogicResult> CombatHandling(IList<Monster> targets)
        {
            if (PoeHelpers.HasDangerousNeighbours(LokiPoe.MyPosition, targets))
                return LogicResult.Unprovided;

            foreach (var slot in buffSlots)
                if (await slot.Use())
                    return LogicResult.Provided;

            return LogicResult.Unprovided;
        }
    }
}