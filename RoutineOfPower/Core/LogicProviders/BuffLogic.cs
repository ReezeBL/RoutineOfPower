using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using Loki.Bot;
using Loki.Game;
using Loki.Game.GameData;
using Loki.Game.Objects;
using RoutineOfPower.Core.SkillHandlers;
using RoutineOfPower.Core.SkillHandlers.Decorators;

namespace RoutineOfPower.Core.LogicProviders
{
    public class BuffLogic : ILogicHandler
    {
        private static readonly HashSet<string> BuffNames = new HashSet<string> {"Blood Rage", "Tempest Shield"};

        private readonly List<SkillWrapper> buffSlots = new List<SkillWrapper>(8);
        private IList<Monster> cachedTargets;

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
            foreach (var skill in PoeHelpers.GetSkillbarSkills())
            {
                if (BuffNames.Contains(skill.Name))
                    buffSlots.Add(new SkillWrapper(skill.Slot,
                        new SingleCastHandler().AddDecorator(new DontHaveAuraDecorator(skill.Name))));
                switch (skill.Name)
                {
                    case "Vaal Grace":
                        buffSlots.Add(new SkillWrapper(skill.Slot,
                            new SingleCastHandler().AddDecorator(new ConditionalDecorator(CheckMonsterCount))
                        ));
                        break;
                    case "Vaal Discipline":
                        buffSlots.Add(new SkillWrapper(skill.Slot,
                            new SingleCastHandler().AddDecorator(new ConditionalDecorator(EnergyShieldCheck))
                        ));
                        break;
                }
            }
        }

        public async Task<LogicResult> OutCombatHandling()
        {
            return await Task.FromResult(LogicResult.Unprovided);
        }

        public async Task<LogicResult> CombatHandling(IList<Monster> targets)
        {
            cachedTargets = targets;
            foreach (var slot in buffSlots)
            {
                if (await slot.Use())
                    return LogicResult.Provided;
            }

            return LogicResult.Unprovided;
        }


        private static bool EnergyShieldCheck(int slot)
        {
            return LokiPoe.Me.EnergyShieldPercent <= 60;
        }

        private bool CheckMonsterCount(int slot)
        {
            if (cachedTargets == null)
                return false;

            var normalMonstersCount = 0;
            var magicMonsterCount = 0;
            var strongMonsterCount = 0;

            foreach (var monster in cachedTargets)
            {
                var distance = monster.Distance;
                if (distance <= 25 && monster.Rarity == Rarity.Normal)
                    normalMonstersCount++;
                else if (distance <= 40 && monster.Rarity == Rarity.Magic)
                    magicMonsterCount++;
                else if (distance <= 60 && monster.Rarity >= Rarity.Rare)
                    strongMonsterCount++;
            }

            return normalMonstersCount >= 15 || magicMonsterCount >= 3 || strongMonsterCount >= 1;
        }
    }
}