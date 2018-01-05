using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using log4net;
using Loki.Common;
using Loki.Game;
using Loki.Game.Objects;
using RoutineOfPower.Core.SkillHandlers.Decorators;

namespace RoutineOfPower.Core.SkillHandlers
{
    public abstract class SkillHandler
    {
        protected static readonly ILog Log = Logger.GetLoggerInstanceForType();

        private static readonly Dictionary<string, SkillHandler> Handlers = new Dictionary<string, SkillHandler>
        {
            ["default"] = new ChannelingSkill(),
            ["bf"] = new ChannelingSkill(() => LokiPoe.Me.BladeFlurryCharges >= 6),
            ["short"] = new SingleCastHandler()
        };

        protected bool InternalUse(Func<int, bool, Vector2i, LokiPoe.InGameState.UseResult> useFunc, int slot,
            bool inPlace, Vector2i position)
        {
            var error = useFunc(slot, inPlace, position);
            if (error != LokiPoe.InGameState.UseResult.None)
            {
                Log.Error($"[Rotutine] Failed to cast skill at {slot}: {error}");
                return false;
            }
            return true;
        }

        public SkillHandler AddDecorator(Decorator decorator)
        {
            decorator.SetHandler(this);
            return decorator;
        }

        protected static Skill ValidateSlot(int slot, bool inSkillBar = true)
        {
            if (slot == -1)
                return null;
            var skill = LokiPoe.InGameState.SkillBarHud.Slot(slot);

            if (skill == null)
                return null;

            if (!skill.CanUseEx(out var error, inSkillBar))
            {
                //Log.Error($"Cant use {skill.Name}: {error}");
                return null;
            }

            return skill;
        }

        public async Task<bool> HandleSkillAt(int slot, Vector2i position, bool inPlace)
        {
            if (ShouldUse(slot))
                return await UseAt(slot, position, inPlace);
            return false;
        }

        public virtual bool ShouldUse(int slot)
        {
            var skill = ValidateSlot(slot);
            return skill != null;
        }

        public abstract Task<bool> UseAt(int slot, Vector2i position, bool inPlace);


        public static SkillHandler GetSkillHandler(int slot)
        {
            var skill = LokiPoe.InGameState.SkillBarHud.Slot(slot);
            return GetSkillHandler(skill);
        }

        public static SkillHandler GetSkillHandler(Skill skill)
        {
            if (skill == null)
                return Handlers["default"];
            switch (skill.Name)
            {
                case "Blade Flurry":
                    return Handlers["bf"];
                default:
                    return Handlers["default"];
            }
        }

        public static SkillHandler GetSkillHandler(string handlerName)
        {
            if (Handlers.TryGetValue(handlerName, out var handler))
                return handler;
            return Handlers["default"];
        }
    }
}