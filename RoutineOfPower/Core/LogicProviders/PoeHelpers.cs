using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Loki.Bot;
using Loki.Common;
using Loki.Game;
using Loki.Game.Objects;
using RoutineOfPower.Core.Settings;

namespace RoutineOfPower.Core.LogicProviders
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

        public static IEnumerable<Skill> GetSkillBarSkill(Func<Skill, bool> predicate)
        {
            return LokiPoe.InGameState.SkillBarHud.SkillBarSkills.Where(skill => skill != null && predicate(skill));
        }
    }
}