using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using log4net;
using Loki.Bot;
using Loki.Bot.Pathfinding;
using Loki.Common;
using Loki.Game;
using Loki.Game.GameData;
using Loki.Game.Objects;
using RoutineOfPower.Core.Settings;
using RoutineOfPower.Core.Settings.Misc;
using RoutineOfPower.Core.SkillHandlers;
using RoutineOfPower.Core.SkillHandlers.Decorators;
using RoutineOfPower.GUI;

namespace RoutineOfPower.Core.LogicProviders
{
    public class TotemLogic : ILogicHandler
    {
        private static readonly ILog Log = Logger.GetLoggerInstanceForType();
        private readonly TotemLogicSettings settings = new TotemLogicSettings();

        private readonly List<SkillWrapper> supportTotems = new List<SkillWrapper>(8);
        private readonly List<SkillWrapper> offensiveTotems = new List<SkillWrapper>(8);

        public MessageResult Message(Message message)
        {
            return MessageResult.Unprocessed;
        }

        public bool Enabled { get; set; } = true;
        public string Name { get; set; } = "Totems Placement";
        public int Priority { get; set; } = 1;

        public UserControl InterfaceControl { get; private set; }

        public void CreateInterfaceControl()
        {
            InterfaceControl = new TotemLogicGui(settings);
        }

        public void Start()
        {
            //totemSlot = null;
            //var totemSkill = PoeHelpers.GetSkillbarSkills(skill => skill.IsTotem && !skill.IsTrap && !skill.IsMine)
            //    .FirstOrDefault();
            //if (totemSkill != null)
            //    totemSlot = new SkillWrapper(totemSkill.Slot,
            //        new SingleCastHandler()
            //            .AddDecorator(new TimeoutDecorator(4000))
            //            .AddDecorator(new ConditionalDecorator(ShouldPlaceTotem)));

            supportTotems.Clear();
            offensiveTotems.Clear();

            foreach (var slotSettings in settings.TotemSettings)
            {
                var skill = PoeHelpers.GetSkillFromSlot(slotSettings.Slot);
                if (!(skill != null && skill.IsTotem && !skill.IsTrap && !skill.IsMine))
                {
                    Log.ErrorFormat("Skill in slot {0} was set as totem, but its not!", slotSettings.Slot);
                    continue;
                }

                switch (slotSettings.Type)
                {
                    case TotemCastType.Support:
                    {
                        var skillHandler = new SingleCastHandler()
                            .AddDecorator(new TimeoutDecorator(4000))
                            .AddDecorator(new DeployedObjectsDecorator(1, networkObject => networkObject is Monster totem && !totem.IsDead && totem.Distance < 60));

                        var totemSlot = new SkillWrapper(slotSettings.Slot, skillHandler);
                        totemSlot.AddParameter("Rarity", slotSettings.MinMonsterRarity);
                        supportTotems.Add(totemSlot);
                        break;
                    }
                    case TotemCastType.Offensive:
                    {
                        var skillHandler = new SingleCastHandler()
                            .AddDecorator(new TimeoutDecorator(4000));

                        var totemSlot = new SkillWrapper(slotSettings.Slot, skillHandler);
                            totemSlot.AddParameter("Range", slotSettings.Range);
                        totemSlot.AddParameter("TotemCount", slotSettings.MaxTotemCount);
                        offensiveTotems.Add(totemSlot);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            }
        }

        public async Task<LogicResult> OutCombatHandling()
        {
            return await Task.FromResult(LogicResult.Unprovided);
        }

        public async Task<LogicResult> CombatHandling(IList<Monster> targets)
        {
            var bestTarget = targets.FirstOrDefault();

            foreach (var supportTotem in supportTotems)
            {
                if (await HandleSupportTotem(supportTotem, bestTarget))
                    return LogicResult.Provided;
            }

            foreach (var offensiveTotem in offensiveTotems)
            {
                if (await HandleOffensiveTotem(offensiveTotem, bestTarget))
                    return LogicResult.Provided;
            }

            return LogicResult.Unprovided;
        }

        private static async Task<bool> HandleSupportTotem(SkillWrapper totem, Monster target)
        {
            if (!totem.CanUse() || target == null)
                return false;

            var supportedRarity = totem.GetParameter<Rarity>("Rarity");
            if (target.Rarity < supportedRarity)
                return false;

            var castPosition = LokiPoe.MyPosition + PoeHelpers.GetRandomVector(3);
            var distance = target.Distance;

            if (ExilePather.CanObjectSee(LokiPoe.Me, target))
                castPosition = LokiPoe.MyPosition.GetPointAtDistanceBeforeEnd(target.Position, distance / 2f);

            return await totem.UseAt(castPosition, true);
        }

        private async Task<bool> HandleOffensiveTotem(SkillWrapper totem, Monster target)
        {
            if (!totem.CanUse() || target == null)
                return false;

            var cachedPosition = target.Position;
            var moveResult = PoeHelpers.MoveInRange(target, settings.PlaceRange);

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

            var totemRange = totem.GetParameter<float>("Range");
            var maxTotemCount = totem.GetParameter<int>("TotemCount");
            var skill = totem.Skill;

            if (skill.DeployedObjects.OfType<Monster>().Count(deployedTotem =>
                    !deployedTotem.IsDead && cachedPosition.Distance(deployedTotem.Position) < totemRange) >= maxTotemCount)
                return false;
            var placePosition = LokiPoe.MyPosition.GetPointAtDistanceBeforeEnd(cachedPosition, totemRange / 2f);

            return await totem.UseAt(placePosition, true);
        }
    }
}