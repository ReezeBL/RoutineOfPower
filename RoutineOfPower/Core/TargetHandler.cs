using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Loki.Bot;
using Loki.Common;
using Loki.Game;
using Loki.Game.GameData;
using Loki.Game.Objects;
using RoutineOfPower.Core.Settings;

namespace RoutineOfPower.Core
{
    internal class TargetHandler
    {
        private static readonly ILog Log = Logger.GetLoggerInstanceForType();

        private static readonly string[] IgnoredMonsterAuras =
        {
            "shrine_godmode", // Ignore any mob near Divine Shrine
            "bloodlines_invulnerable", // Ignore Phylacteral Link
            "god_mode", // Ignore Animated Guardian
            "bloodlines_necrovigil"
        };

        private readonly Targeting combatTargeting;

        public TargetHandler()
        {
            combatTargeting = new Targeting();

            combatTargeting.WeightCalculation += TargetingWeightCalcuations;
            combatTargeting.InclusionCalcuation += TargetingInclusionCalculations;
        }


        public IEnumerable<T> Targets<T>() where T : class
        {
            return combatTargeting.Targets<T>();
        }

        public IEnumerable<NetworkObject> Targets()
        {
            return combatTargeting.Targets();
        }

        public void Update()
        {
            combatTargeting.Update();
        }

        public void Reset()
        {
            combatTargeting.ResetInclusionCalcuation();
            combatTargeting.ResetWeightCalculation();

            combatTargeting.WeightCalculation += TargetingWeightCalcuations;
            combatTargeting.InclusionCalcuation += TargetingInclusionCalculations;
        }

        public Targeting Targeting => combatTargeting;

        private static void TargetingWeightCalcuations(NetworkObject entity, ref float weight)
        {
            weight -= entity.Distance;

            var m = entity as Monster;
            if (m == null)
                return;

            // If the monster is the source of Allies Cannot Die, we really want to kill it fast.
            if (m.HasAura("monster_aura_cannot_die"))
                weight += 100;

            // Necros
            if (m.ExplicitAffixes.Any(a => a.InternalName.Contains("RaisesUndead")) ||
                m.ImplicitAffixes.Any(a => a.InternalName.Contains("RaisesUndead")))
                weight += 45;

            switch (m.Rarity)
            {
                case Rarity.Normal when m.Type.Contains("/Totems/"):
                    weight += 10;
                    break;
                case Rarity.Magic:
                    weight += 5;
                    break;
                case Rarity.Rare:
                    weight += 10;
                    break;
                case Rarity.Unique:
                    weight += 15;
                    break;
            }

            /*if (m.IsTargetingMe)
			{
				weight += 20;
			}*/

            // Minions that get in the way.
            switch (m.Name)
            {
                case "Summoned Skeleton":
                    weight -= 15;
                    break;
                case "Raised Zombie":
                    weight -= 15;
                    break;
                case "Lightless Grub":
                    weight -= 30;
                    break;
            }

            

            // Ignore these mostly, as they just respawn.
            if (m.Type.Contains("TaniwhaTail"))
                weight -= 30;

            // Ignore mobs that expire and die
            if (m.Components.DiesAfterTimeComponent != null)
                weight -= 15;

            // Make sure hearts are targeted with highest priority.
            if (m.Type.Contains("/BeastHeart"))
                weight += 75;

            if (m.Metadata == "Metadata/Monsters/Tukohama/TukohamaShieldTotem")
                weight += 75;

            if (m.IsStrongboxMinion || m.IsHarbingerMinion)
                weight += 30;
        }

        private static bool TargetingInclusionCalculations(NetworkObject entity)
        {
            try
            {
                var m = entity as Monster;
                if (m == null)
                    return false;

                if (Blacklist.Contains(m))
                    return false;

                // Do not consider inactive/dead mobs.
                if (!m.IsActive)
                    return false;

                // Ignore any mob that cannot die.
                if (m.CannotDie)
                    return false;

                // Ignore mobs that are too far to care about.
                if (m.Distance > RoutineSettings.Instance.CombatRange)
                    return false;

                // Ignore mobs with special aura/buffs
                if (m.HasAura(IgnoredMonsterAuras))
                    return false;

                // Ignore Voidspawn of Abaxoth: thanks ExVault!
                if (m.ExplicitAffixes.Any(a => a.DisplayName == "Voidspawn of Abaxoth"))
                    return false;


                switch (m.Name)
                {
                    // Ignore these mobs when trying to transition in the dom fight.
                    // Flag1 has been seen at 5 or 6 at times, so need to work out something more reliable.
                    case "Miscreation":
                        var dom = LokiPoe.ObjectManager.GetObjectByName<Monster>("Dominus, High Templar");
                        if (dom != null && !dom.IsDead &&
                            (dom.Components.TransitionableComponent.Flag1 == 6 ||
                             dom.Components.TransitionableComponent.Flag1 == 5))
                        {
                            Blacklist.Add(m.Id, TimeSpan.FromHours(1), "Miscreation");
                            return false;
                        }
                        break;
                    // Ignore Piety's portals.
                    case "Chilling Portal":
                    case "Burning Portal":
                        Blacklist.Add(m.Id, TimeSpan.FromHours(1), "Piety portal");
                        return false;
                }
            }
            catch (Exception ex)
            {
                Log.Error("[CombatOnInclusionCalcuation]", ex);
                return false;
            }
            return true;
        }
    }
}