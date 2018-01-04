using System.Collections.Generic;
using System.Collections.ObjectModel;
using Loki;
using Loki.Common;
using Newtonsoft.Json;
using RoutineOfPower.Core.LogicProviders;

namespace RoutineOfPower.Core.Settings
{
    public class RoutineSettings : JsonSettings
    {
        private static RoutineSettings instance;

        private float combatRange = 75;
        [JsonIgnore] private ObservableCollection<ILogicHandler> handlersList;
        private bool leaveFrame;

        [JsonRequired]
        private readonly Dictionary<string, LogicSettingsWrapper> logicSettings =
            new Dictionary<string, LogicSettingsWrapper>();

        private int minMobsToTriggerCombat = 3;
        private bool needsToDisableAlwaysHighlight = true;

        private RoutineSettings() : base(GetSettingsFilePath(Configuration.Instance.Name, "RoutineOfPower",
            $"{nameof(RoutineOfPower)}.json"))
        {
        }

        public static RoutineSettings Instance => instance ?? (instance = new RoutineSettings());

        public float CombatRange
        {
            get => combatRange;
            set
            {
                if (value.Equals(combatRange)) return;
                combatRange = value;
                NotifyPropertyChanged(() => CombatRange);
            }
        }

        public bool NeedsToDisableAlwaysHighlight
        {
            get => needsToDisableAlwaysHighlight;
            set
            {
                if (value == needsToDisableAlwaysHighlight) return;
                needsToDisableAlwaysHighlight = value;
                NotifyPropertyChanged(() => NeedsToDisableAlwaysHighlight);
            }
        }

        public bool LeaveFrame
        {
            get => leaveFrame;
            set
            {
                if (value == leaveFrame) return;
                leaveFrame = value;
                NotifyPropertyChanged(() => LeaveFrame);
            }
        }

        public int MinMobsToTriggerCombat
        {
            get => minMobsToTriggerCombat;
            set
            {
                if (value == minMobsToTriggerCombat) return;
                minMobsToTriggerCombat = value;
                NotifyPropertyChanged(() => MinMobsToTriggerCombat);
            }
        }

        [JsonIgnore]
        public ObservableCollection<ILogicHandler> HandlersList
        {
            get => handlersList;
            set
            {
                if (Equals(value, handlersList)) return;
                handlersList = value;
                NotifyPropertyChanged(() => HandlersList);
            }
        }

        public void GetSettingsForHandler(ILogicHandler handler)
        {
            if (logicSettings.TryGetValue(handler.Name, out var settignsWrapper))
                settignsWrapper.SetToHandler(handler);
        }

        public void SetSettingsForHandler(ILogicHandler handler)
        {
            if (!logicSettings.TryGetValue(handler.Name, out var settignsWrapper))
                logicSettings[handler.Name] = settignsWrapper = new LogicSettingsWrapper();
            settignsWrapper.SetFromHandler(handler);
        }
    }
}