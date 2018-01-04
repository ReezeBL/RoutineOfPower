using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
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

        public static void DisableAlwaysHiglight()
        {
            if (RoutineSettings.Instance.NeedsToDisableAlwaysHighlight &&
                LokiPoe.ConfigManager.IsAlwaysHighlightEnabled)
            {
                Log.InfoFormat("[DisableAlwaysHiglight] Now disabling Always Highlight to avoid skill use issues.");
                LokiPoe.Input.SimulateKeyEvent(LokiPoe.Input.Binding.highlight_toggle, true, false, false);
            }
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

        public static IEnumerable<Skill> GetSkillbarSkills(Func<Skill, bool> predicate)
        {
            return LokiPoe.InGameState.SkillBarHud.SkillBarSkills.Where(skill => skill != null && predicate(skill));
        }

        public static IEnumerable<Skill> GetSkillbarSkills()
        {
            return LokiPoe.InGameState.SkillBarHud.SkillBarSkills.Where(skill => skill != null);
        }

        public static bool HasDangerousNeighbours(Vector2i position, IEnumerable<Monster> monsters)
        {
            var normalMonstersCount = 0;
            var magicMonsterCount = 0;
            var strongMonsterCount = 0;

            foreach (var monster in monsters)
            {
                var distance = position.DistanceF(monster.Position);
                if (distance <= 20 && monster.Rarity == Rarity.Normal)
                    normalMonstersCount++;
                else if (distance <= 25 && monster.Rarity == Rarity.Magic)
                    magicMonsterCount++;
                else if (distance <= 25 && monster.Rarity == Rarity.Rare)
                    strongMonsterCount++;
            }

            return normalMonstersCount >= 10 || magicMonsterCount >= 5 || strongMonsterCount >= 2;
        }
    }
}