using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Loki.Bot;
using Loki.Bot.Pathfinding;
using Loki.Common;
using Loki.Game;
using Loki.Game.GameData;
using Loki.Game.Objects;
using RoutineOfPower.Core.Settings;

namespace RoutineOfPower.Core
{
    public static class PoeHelpers
    {
        private static readonly ILog Log = Logger.GetLoggerInstanceForType();
        private static readonly Random random = new Random();

        public static void DisableAlwaysHiglight()
        {
            if (RoutineSettings.Instance.NeedsToDisableAlwaysHighlight &&
                LokiPoe.ConfigManager.IsAlwaysHighlightEnabled)
            {
                Log.InfoFormat("[DisableAlwaysHiglight] Now disabling Always Highlight to avoid skill use issues.");
                LokiPoe.Input.SimulateKeyEvent(LokiPoe.Input.Binding.highlight_toggle, true, false, false);
            }
        }

        public static int NumberOfHostileMonstersNear(NetworkObject target, float distance, Rarity rarity)
        {
            var targetPosition = target.Position;
            return LokiPoe.ObjectManager.Objects.OfType<Monster>().Count(monster =>
                monster.Id != target.Id 
                && !monster.IsDead
                && monster.Reaction == Reaction.Enemy
                && monster.Rarity == rarity
                && targetPosition.Distance(monster.Position) < distance);
        }

        public static bool EnableAlwaysHiglight()
        {
            if (!LokiPoe.ConfigManager.IsAlwaysHighlightEnabled)
            {
                Log.InfoFormat("[EnableAlwaysHiglight] Now enabling Always Highlight.");
                LokiPoe.Input.SimulateKeyEvent(LokiPoe.Input.Binding.highlight_toggle, true, false, false);
                return true;
            }

            return false;
        }

        public static Skill GetSkillFromSlot(int slot)
        {
            if (slot == -1)
                return null;
            var skill = LokiPoe.InGameState.SkillBarHud.Slot(slot);
            return skill;
        }

        public static IEnumerable<Skill> GetSkillbarSkills(Func<Skill, bool> predicate)
        {
            return LokiPoe.InGameState.SkillBarHud.SkillBarSkills.Where(skill => skill != null && predicate(skill));
        }

        public static IEnumerable<Skill> GetSkillbarSkills()
        {
            return LokiPoe.InGameState.SkillBarHud.SkillBarSkills.Where(skill => skill != null);
        }

        public static MoveResult MoveInRange(Monster monster, float minRange)
        {
            var cachedPosition = monster.Position;

            var hasProximity = false;
            var monsterId = -1;

            try
            {
                hasProximity = monster.HasProximityShield;
                monsterId = monster.Id;
            }
            catch (Exception)
            {
                // ignored
            }


            var distance = LokiPoe.MyPosition.Distance(ref cachedPosition);

            var skipPathing = monster.Rarity == Rarity.Unique &&
                              (monster.Metadata.Contains("KitavaBoss/Kitava") ||
                               monster.Metadata.Contains("VaalSpiderGod/Arakaali"));

            var canSee = ExilePather.CanObjectSee(LokiPoe.Me, cachedPosition, !RoutineSettings.Instance.LeaveFrame);
            var blockedByDoor = ClosedDoorBetween(LokiPoe.Me, monster);
            var pathDistance = ExilePather.PathDistance(LokiPoe.MyPosition, cachedPosition, dontLeaveFrame: !RoutineSettings.Instance.LeaveFrame);
            

            if (pathDistance.CompareTo(float.MaxValue) == 0 && !skipPathing)
            {
                Log.ErrorFormat("[Logic] Could not determine the path distance to the best target. Now blacklisting it.");
                Blacklist.Add(monsterId, TimeSpan.FromMinutes(1), "Unable to pathfind to.");
                return MoveResult.MoveFailed;
            }

            if (pathDistance > 2 * RoutineSettings.Instance.CombatRange && !skipPathing)
            {
                EnableAlwaysHiglight();
                //Blacklist.Add(monsterId, TimeSpan.FromSeconds(10), "Too far");
                return MoveResult.TargetTooFar;
            }


            if ((!canSee && !skipPathing) || blockedByDoor)
            {
                if (!PlayerMover.MoveTowards(cachedPosition))
                    Log.ErrorFormat("[Logic] MoveTowards failed for {0}.", cachedPosition);
                return MoveResult.MoveFailed;
            }


            if (distance > minRange || distance > 10 && hasProximity)
            {
                var range = hasProximity ? 10 : minRange;
                var rangedLocation = LokiPoe.MyPosition.GetPointAtDistanceBeforeEnd(cachedPosition, range);
                if (!PlayerMover.MoveTowards(rangedLocation))
                    Log.ErrorFormat("[Logic] MoveTowards failed for {0}.", rangedLocation);
                return MoveResult.MoveFailed;
            }

            return MoveResult.MoveSuccseed;
        }

        public static bool HasDangerousNeighbours(Vector2i position, IEnumerable<Monster> monsters)
        {
            var normalMonstersCount = 0;
            var magicMonsterCount = 0;
            var strongMonsterCount = 0;

            foreach (var monster in monsters)
            {
                var distance = position.DistanceF(monster.Position);
                var attackingMe = monster.HasCurrentAction && monster.CurrentAction.Target == LokiPoe.Me;
                if ((distance <= 25 || attackingMe) && monster.Rarity == Rarity.Normal)
                    normalMonstersCount++;
                else if ((distance <= 30 || attackingMe) && monster.Rarity == Rarity.Magic)
                    magicMonsterCount++;
                else if ((distance <= 30 || attackingMe) && monster.Rarity == Rarity.Rare)
                    strongMonsterCount++;
            }

            var manyNormalMonsters = normalMonstersCount >= 8;
            var manyMagicMonsters = magicMonsterCount >= 5;
            var manyRareMonsters = strongMonsterCount >= 2;

            return manyRareMonsters || manyNormalMonsters || manyMagicMonsters;
        }

        public static Vector2i GetRandomVector(int offset)
        {
            return new Vector2i(random.Next(-offset, offset), random.Next(-offset, offset));
        }

        private static bool ClosedDoorBetween(NetworkObject start, NetworkObject end, int distanceFromPoint = 10, int stride = 10)
        {
            return ClosedDoorBetween(start.Position, end.Position, distanceFromPoint, stride);
        }

        private static bool ClosedDoorBetween(Vector2i start, Vector2i end, int distanceFromPoint = 10, int stride = 10)
        {
            var doors = LokiPoe.ObjectManager.AnyDoors.Where(d => !d.IsOpened).ToList();
            if (!doors.Any())
                return false;

            var path = ExilePather.GetPointsOnSegment(start, end, RoutineSettings.Instance.LeaveFrame);

            for (var i = 0; i < path.Count; i += stride)
            {
                foreach (var door in doors)
                {
                    if (door.Position.Distance(path[i]) <= distanceFromPoint)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    

    public enum MoveResult
    {
        MoveFailed,
        TargetTooFar,
        MoveSuccseed
    }
}