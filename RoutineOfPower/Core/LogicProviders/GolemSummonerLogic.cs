using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using Loki.Bot;
using Loki.Game;
using Loki.Game.GameData;
using Loki.Game.Objects;
using RoutineOfPower.Core.SkillHandlers;

namespace RoutineOfPower.Core.LogicProviders
{
    public class GolemSummonerLogic : ILogicHandler
    {
        private static readonly HashSet<string> Golems = new HashSet<string>
        {
            "Summon Chaos Golem",
            "Summon Ice Golem",
            "Summon Flame Golem",
            "Summon Stone Golem",
            "Summon Lightning Golem"
        };

        private SkillWrapper golemSlot;

        public MessageResult Message(Message message)
        {
            return MessageResult.Unprocessed;
        }

        public bool Enabled { get; set; } = true;
        public string Name { get; set; } = "Summon Golems";
        public int Priority { get; set; } = 0;

        public UserControl InterfaceControl { get; } = null;

        public void CreateInterfaceControl()
        {
        }

        private static bool CanSummonGolem(int slot)
        {
            var skill = LokiPoe.InGameState.SkillBarHud.Slot(slot);
            var max = skill.GetStat(StatTypeGGG.NumberOfGolemsAllowed);
            return skill.NumberDeployed < max;
        }

        public void Start()
        {
            golemSlot = null;
            var golemSkill = PoeHelpers.GetSkillBarSkill(skill => Golems.Contains(skill.Name)).FirstOrDefault();
            if (golemSkill != null)
            {
                golemSlot = new SkillWrapper(golemSkill.Slot, new TimeoutSkillHandler(4000, CanSummonGolem));
            }
        }

        public async Task<LogicResult> OutCombatHandling()
        {
            return await HandleSummon();
        }

        public async Task<LogicResult> CombatHandling(IList<Monster> targets)
        {
            return await HandleSummon();
        }

        private async Task<LogicResult> HandleSummon()
        {
            if (golemSlot == null)
                return LogicResult.Unprovided;

            if (await golemSlot.Use())
                return LogicResult.Provided;

            return LogicResult.Unprovided;
        }
    }
}