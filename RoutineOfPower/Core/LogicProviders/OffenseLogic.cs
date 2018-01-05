using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using log4net;
using Loki.Bot;
using Loki.Common;
using Loki.Game;
using Loki.Game.GameData;
using Loki.Game.Objects;
using RoutineOfPower.Core.Settings;
using RoutineOfPower.Core.SkillHandlers;

namespace RoutineOfPower.Core.LogicProviders
{
    public abstract class OffenseLogic : ILogicHandler
    {
        private static readonly ILog Log = Logger.GetLoggerInstanceForType();
        private Skill cachedSkill;
        private SkillHandler cachedSkillHandler;

        private Func<Rarity, int, bool> flaskHook;

        protected abstract OffenseLogicSettings Settings { get; }

        public virtual MessageResult Message(Message message)
        {
            if (message.Id == "SetFlaskHook")
            {
                var func = message.GetInput<Func<Rarity, int, bool>>();
                flaskHook = func;
                Log.Info("[Routinr] Flask hook has been set.");
                return MessageResult.Processed;
            }

            return MessageResult.Unprocessed;
        }

        public bool Enabled { get; set; } = true;
        public abstract string Name { get; set; }
        public int Priority { get; set; }

        public abstract UserControl InterfaceControl { get; }


        public abstract void CreateInterfaceControl();

        public void Start()
        {
        }

        public abstract Task<LogicResult> OutCombatHandling();
        public abstract Task<LogicResult> CombatHandling(IList<Monster> targets);

        protected async Task<bool> ProccessTarget(Monster monster)
        {
            if (!monster.IsActive)
                return true;

            var cachedPosition = monster.Position;
            
            var moveResult = PoeHelpers.MoveInRange(monster, Settings.Range);

            switch (moveResult)
            {
                case MoveResult.TargetTooFar:
                    return false;
                case MoveResult.MoveFailed:
                    return true;
                case MoveResult.MoveSuccseed:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            PoeHelpers.DisableAlwaysHiglight();

            //TODO: failing to get monster stats with exception sometimes, wtf
            try
            {
                flaskHook?.Invoke(monster.Rarity, (int) monster.HealthPercentTotal);
            }
            catch (Exception)
            {
                // ignored
            }

            var skill = LokiPoe.InGameState.SkillBarHud.Slot(Settings.Slot);
            if (skill != cachedSkill || cachedSkillHandler == null)
            {
                cachedSkill = skill;
                cachedSkillHandler = SkillHandler.GetSkillHandler(cachedSkill);
            }


            var result =
                await cachedSkillHandler.HandleSkillAt(Settings.Slot, cachedPosition, Settings.AttackInPlace);
            return result;
        }
    }
}