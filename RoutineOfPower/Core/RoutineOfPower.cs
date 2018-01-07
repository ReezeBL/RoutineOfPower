using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using log4net;
using Loki.Bot;
using Loki.Common;
using Loki.Game.GameData;
using Loki.Game.Objects;
using RoutineOfPower.Core.LogicProviders;
using RoutineOfPower.Core.Settings;
using RoutineOfPower.GUI;

namespace RoutineOfPower.Core
{
    public class RoutineOfPower : IRoutine
    {
        private readonly List<ILogicHandler> logicHandlers = new List<ILogicHandler>(15);
        private static readonly ILog Log = Logger.GetLoggerInstanceForType();

        private TargetHandler targetHandler;
        private UserControl guiWindow;

        public UserControl Control
        {
            get
            {
                if (guiWindow == null)
                {
                    guiWindow = new MainWindow();
                    foreach (var handler in logicHandlers)
                        handler.CreateInterfaceControl();
                }
                return guiWindow;
            }
        }

        public JsonSettings Settings => RoutineSettings.Instance;

        public MessageResult Message(Message message)
        {
            if (message.Id == "SetCombatRange")
            {
                var range = message.GetInput<int>();
                RoutineSettings.Instance.CombatRange = range;
                Log.Info($"[Routine] Combat range is set to {range}");
                return MessageResult.Processed;
            }

            if (message.Id == "GetCombatRange")
            {
                message.AddOutput(this, RoutineSettings.Instance.CombatRange);
                return MessageResult.Processed;
            }

            var wasProcessed = false;
            foreach (var handler in logicHandlers.Where(handler => handler.Enabled))
                if (handler.Message(message) == MessageResult.Processed)
                    wasProcessed = true;
            return wasProcessed ? MessageResult.Processed : MessageResult.Unprocessed;
        }

        public void Start()
        {
            foreach (var handler in logicHandlers.Where(handler => handler.Enabled))
                handler.Start();
        }

        public void Stop()
        {
            //TODO: Stop Function;
        }

        public void Tick()
        {
        }

        public string Name { get; } = "Routine of Power";
        public string Author { get; } = "Siamant";
        public string Description { get; } = "One routine to rule them all";
        public string Version { get; } = "0.0.1";

        public void Initialize()
        {
            targetHandler = new TargetHandler();

            var interfaceType = typeof(ILogicHandler);
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes())
                .Where(type => interfaceType.IsAssignableFrom(type) && !type.IsAbstract);

            foreach (var type in types)
            {
                var handler = (ILogicHandler) Activator.CreateInstance(type);
                RoutineSettings.Instance.GetSettingsForHandler(handler);
                logicHandlers.Add(handler);
            }

            RoutineSettings.Instance.HandlersList = new ObservableCollection<ILogicHandler>(logicHandlers.OrderByDescending(handler => handler.Priority));
        }

        public void Deinitialize()
        {
            foreach (var handler in logicHandlers)
            {
                RoutineSettings.Instance.SetSettingsForHandler(handler);
            }
        }

        public async Task<LogicResult> Logic(Logic logic)
        {
            if (logic.Id == "hook_combat")
            {
                using (new PerformanceTimer("Updating Priority Target", 1))
                {
                    targetHandler.Update();
                }

                var targets = targetHandler.Targets<Monster>().ToList();
                var strongEnemy = targets.Any(target => target.Rarity >= Rarity.Magic);

                if (targets.Count >= RoutineSettings.Instance.MinMobsToTriggerCombat || strongEnemy)
                {
                    foreach (var handler in logicHandlers.Where(handler => handler.Enabled)
                        .OrderByDescending(handler => handler.Priority))
                        if (await handler.CombatHandling(targets) == LogicResult.Provided)
                            return LogicResult.Provided;
                    Log.Error("Combat was triggered, but no logic provided!");
                }
                else
                {
                    foreach (var handler in logicHandlers.Where(handler => handler.Enabled)
                        .OrderByDescending(handler => handler.Priority))
                        if (await handler.OutCombatHandling() == LogicResult.Provided)
                            return LogicResult.Provided;
                }
            }

            return LogicResult.Unprovided;
        }

        public override string ToString()
        {
            return Name + ": " + Description;
        }
    }
}