using System.Collections.ObjectModel;

namespace RoutineOfPower.Core.Settings.Misc
{
    public class VaalAuraInfo
    {
        public string Name { get; set; } = "";
        public ObservableCollection<TriggerSettings> Triggers { get; set; } = new ObservableCollection<TriggerSettings>();
    }
}