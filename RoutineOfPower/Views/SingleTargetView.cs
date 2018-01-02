using System.Collections.Generic;
using RoutineOfPower.Core.Settings;

namespace RoutineOfPower.Views
{
    public class SingleTargetView : AbstractViewer
    {
        private OffenseLogicSettings settings;
        public List<int> PossibleSlots { get; } = new List<int>{-1, 1, 2, 3, 4, 5, 6, 7, 8};

        public OffenseLogicSettings Settings
        {
            get => settings;
            set
            {
                if (Equals(value, settings)) return;
                settings = value;
                OnPropertyChanged();
            }
        }
    }
}