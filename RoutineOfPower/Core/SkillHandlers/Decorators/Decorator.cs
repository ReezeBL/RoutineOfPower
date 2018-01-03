namespace RoutineOfPower.Core.SkillHandlers.Decorators
{
    public abstract class Decorator : SkillHandler
    {
        protected SkillHandler handler;

        public void SetHandler(SkillHandler handler)
        {
            this.handler = handler;
        }
    }
}