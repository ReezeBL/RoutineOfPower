using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using log4net;
using Loki.Bot;
using Loki.Common;
using Loki.Game;
using Loki.Game.Objects;
using RoutineOfPower.Core.SkillHandlers;

namespace RoutineOfPower.Core.LogicProviders
{
    public class AurasLogic : ILogicHandler
    {
        private static readonly ILog Log = Logger.GetLoggerInstanceForType();

        private static readonly HashSet<string> AuraNames = new HashSet<string>
        {
            "Anger",
            "Clarity",
            "Determination",
            "Discipline",
            "Grace",
            "Haste",
            "Hatred",
            "Purity of Elements",
            "Purity of Fire",
            "Purity of Ice",
            "Purity of Lightning",
            "Vitality",
            "Wrath",
            "Envy"
        };

        private readonly SkillHandler caster = SkillHandler.GetSkillHandler("short");

        private int auraSlot = -1;

        public MessageResult Message(Message message)
        {
            return MessageResult.Unprocessed;
        }

        public bool Enabled { get; set; } = true;
        public string Name { get; set; } = "Auras";
        public int Priority { get; set; }

        public UserControl InterfaceControl { get; private set; }


        public void CreateInterfaceControl()
        {
            InterfaceControl = null;
        }

        public void Start()
        {
            auraSlot = -1;
            var hasAuras = false;
            foreach (var skill in LokiPoe.InGameState.SkillBarHud.Skills)
            {
                var tags = skill.SkillTags;
                var skillName = skill.Name;

                if (tags.Contains("aura") && !tags.Contains("vaal") || AuraNames.Contains(skillName) ||
                    skill.IsAurifiedCurse || skill.IsConsideredAura)
                {
                    if (skill.Slot != -1 && auraSlot != -1)
                        auraSlot = skill.Slot;
                    hasAuras = true;
                }
            }

            if (auraSlot == -1 && hasAuras)
                auraSlot = 8;
        }

        public async Task<LogicResult> OutCombatHandling()
        {
            if (auraSlot == -1)
                return LogicResult.Unprovided;

            var skillChanged = false;
            var cachedSkill = LokiPoe.InGameState.SkillBarHud.Slot(auraSlot);

            foreach (var auraSkill in LokiPoe.InGameState.SkillBarHud.Skills)
                if (auraSkill.IsAurifiedCurse)
                {
                    if (!auraSkill.AmICursingWithThis && auraSkill.CanUse(ignoreOnSkillBar: true))
                        skillChanged |= await TryUseAura(auraSkill);
                }

                else if (auraSkill.IsConsideredAura)
                {
                    if (!auraSkill.AmIUsingConsideredAuraWithThis && auraSkill.CanUse(ignoreOnSkillBar: true))
                        skillChanged |= await TryUseAura(auraSkill);
                }

                else if (auraSkill.SkillTags.Contains("aura") && !auraSkill.SkillTags.Contains("vaal") ||
                         AuraNames.Contains(auraSkill.Name))
                {
                    if (!LokiPoe.Me.HasAura(auraSkill.Name) && auraSkill.CanUse(ignoreOnSkillBar: true))
                        skillChanged |= await TryUseAura(auraSkill);
                }

            if (skillChanged)
            {
                var result = LokiPoe.InGameState.SkillBarHud.SetSlot(auraSlot, cachedSkill);
                if (result != LokiPoe.InGameState.SetSlotResult.None)
                    Log.Error($"[Rotutine] Failed to set skill slot: {result} {auraSlot}");

                await Coroutines.LatencyWait(10);
                return LogicResult.Provided;
            }

            return LogicResult.Unprovided;
        }

        public async Task<LogicResult> CombatHandling(IList<Monster> targets)
        {
            return await Task.FromResult(LogicResult.Unprovided);
        }

        private async Task<bool> TryUseAura(Skill skill)
        {
            if (skill.Slot != -1)
            {
                await caster.HandleSkillAt(skill.Slot, LokiPoe.MyPosition, true);
                return false;
            }
            var result = LokiPoe.InGameState.SkillBarHud.SetSlot(auraSlot, skill);
            if (result != LokiPoe.InGameState.SetSlotResult.None)
            {
                Log.Error($"[Rotutine] Failed to set skill slot: {result} {auraSlot}");
                return false;
            }

            await caster.HandleSkillAt(auraSlot, LokiPoe.MyPosition, true);
            await Coroutines.LatencyWait(10);
            return true;
        }
    }
}