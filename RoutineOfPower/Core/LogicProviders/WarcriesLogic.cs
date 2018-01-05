using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using Loki.Bot;
using Loki.Game;
using Loki.Game.Objects;
using RoutineOfPower.Core.SkillHandlers;
using RoutineOfPower.Core.SkillHandlers.Decorators;

namespace RoutineOfPower.Core.LogicProviders
{
    public class WarcriesLogic : ILogicHandler
    {
        private static readonly HashSet<string> WarCries = new HashSet<string>{ "Enduring Cry", "Abyssal Cry", "Rallying Cry" };
        public MessageResult Message(Message message)
        {
            return MessageResult.Unprocessed;
        }

        private SkillWrapper warcrySlot;

        public bool Enabled { get; set; } = true;
        public string Name { get; set; } = "War Cries";
        public int Priority { get; set; } = 0;
        public UserControl InterfaceControl { get; } = null;
        public void CreateInterfaceControl()
        {
            
        }

        public void Start()
        {
            warcrySlot = null;
            var skill = PoeHelpers.GetSkillbarSkills(barSkill => WarCries.Contains(barSkill.Name)).FirstOrDefault();
            if(skill == null)
                return;

            SkillHandler handler = null;
            switch (skill.Name)
            {
                case "Enduring Cry":
                    handler = new SingleCastHandler()
                        .AddDecorator(new TimeoutDecorator(4000))
                        .AddDecorator(new ConditionalDecorator(EnduranceChargesTimeout));
                    break;
                case "Rallying Cry":
                    handler = new SingleCastHandler()
                        .AddDecorator(new TimeoutDecorator(4000))
                        .AddDecorator(new ConditionalDecorator(RailingCryTimeout));
                    break;
                case "Abyssal Cry":
                    handler = new SingleCastHandler()
                        .AddDecorator(new TimeoutDecorator(4000));
                    break;
            }

            if(handler == null)
                return;

            warcrySlot = new SkillWrapper(skill.Slot, handler);
        }

        private static bool RailingCryTimeout(int arg)
        {
            return (LokiPoe.Me.Auras.FirstOrDefault(aura => aura.Name == "Rallying Cry")?.TimeLeft.Seconds ?? 0) < 2;
        }

        public async Task<LogicResult> OutCombatHandling()
        {
            return await Task.FromResult(LogicResult.Unprovided);
        }

        public async Task<LogicResult> CombatHandling(IList<Monster> targets)
        {
            if (warcrySlot == null)
                return LogicResult.Unprovided;
            if (!targets.Any(monster => monster.Distance <= 60))
                return LogicResult.Unprovided;

            if (await warcrySlot.Use())
                return LogicResult.Provided;
            return LogicResult.Unprovided;

        }

        private static bool EnduranceChargesTimeout(int slot)
        {
            return (LokiPoe.Me.EnduranceChargeAura?.TimeLeft.Seconds ?? 0) < 5;
        }
    }
}