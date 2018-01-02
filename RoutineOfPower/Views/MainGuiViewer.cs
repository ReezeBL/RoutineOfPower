using System.Collections.ObjectModel;
using RoutineOfPower.Core.LogicProviders;
using RoutineOfPower.Core.Settings;

namespace RoutineOfPower.Views
{
    public class MainGuiViewer : AbstractViewer
    {
        private ILogicHandler selectedHandler;

        public ObservableCollection<ILogicHandler> LogicHandlers => RoutineSettings.Instance.HandlersList;

        public RoutineSettings Settings { get; } = RoutineSettings.Instance;

        public ILogicHandler SelectedHandler
        {
            get => selectedHandler;
            set
            {
                if (Equals(value, selectedHandler)) return;
                selectedHandler = value;
                OnPropertyChanged();
            }
        }
    }
}