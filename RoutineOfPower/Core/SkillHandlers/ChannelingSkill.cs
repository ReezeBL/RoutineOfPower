using System;
using System.Threading.Tasks;
using Loki.Bot;
using Loki.Common;
using Loki.Game;

namespace RoutineOfPower.Core.SkillHandlers
{
    public class ChannelingSkill : SkillHandler
    {
        private readonly Func<bool> stopChannel;


        public ChannelingSkill(Func<bool> stopChannel = null)
        {
            if (stopChannel == null)
                stopChannel = () => false;
            this.stopChannel = stopChannel;
        }


        public override async Task<bool> UseAt(int slot, Vector2i position, bool inPlace)
        {
            var skill = LokiPoe.InGameState.SkillBarHud.Slot(slot);

            var currentSkill = LokiPoe.Me.CurrentAction.Skill;
            var isChanneling = LokiPoe.Me.HasCurrentAction && currentSkill == skill;

            if (isChanneling && stopChannel())
            {
                await Coroutines.FinishCurrentAction();
                return true;
            }

            if (isChanneling)
            {
                LokiPoe.Input.SetMousePos(position, false);
                return true;
            }

            return InternalUse(LokiPoe.InGameState.SkillBarHud.BeginUseAt, slot, inPlace, position);
        }
    }
}