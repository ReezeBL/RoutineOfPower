using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using log4net;
using Loki.Bot;
using Loki.Bot.Pathfinding;
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
            var cachedPosition = monster.Position;
            var myPosition = LokiPoe.MyPosition;

            var distance = myPosition.Distance(ref cachedPosition);
            var canSee = ExilePather.CanObjectSee(LokiPoe.Me, cachedPosition, !RoutineSettings.Instance.LeaveFrame);
            var pathDistance = ExilePather.PathDistance(myPosition, cachedPosition,
                dontLeaveFrame: !RoutineSettings.Instance.LeaveFrame);
            var hasProximity = monster.HasProximityShield;


            var skipPathing = monster.Rarity == Rarity.Unique &&
                              (monster.Metadata.Contains("KitavaBoss/Kitava") ||
                               monster.Metadata.Contains("VaalSpiderGod/Arakaali"));

            if (pathDistance.CompareTo(float.MaxValue) == 0 && !skipPathing)
            {
                Log.ErrorFormat(
                    "[Logic] Could not determine the path distance to the best target. Now blacklisting it.");
                Blacklist.Add(monster.Id, TimeSpan.FromMinutes(1), "Unable to pathfind to.");
                return true;
            }

            if (pathDistance > RoutineSettings.Instance.CombatRange && !skipPathing)
            {
                PoeHelpers.EnableAlwaysHiglight();
                return false;
            }


            if (!canSee && !skipPathing)
            {
                if (!PlayerMover.MoveTowards(cachedPosition))
                    Log.ErrorFormat("[Logic] MoveTowards failed for {0}.", cachedPosition);
                return true;
            }


            if (distance > Settings.Range || distance > 10 && hasProximity)
            {
                var range = hasProximity ? 10 : Settings.Range;
                var rangedLocation = myPosition.GetPointAtDistanceBeforeEnd(cachedPosition, range);
                if (!PlayerMover.MoveTowards(rangedLocation))
                    Log.ErrorFormat("[Logic] MoveTowards failed for {0}.", rangedLocation);
                return true;
            }

            PoeHelpers.DisableAlwaysHiglight();

            //if (flaskHook != null && flaskHook(monster.Rarity, (int) monster.HealthPercentTotal))
            //    return true;

            flaskHook?.Invoke(monster.Rarity, (int)monster.HealthPercentTotal);

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