using System.Windows;
using RoutineOfPower.Core.Settings;

namespace RoutineOfPower.GUI
{
    /// <summary>
    /// Логика взаимодействия для SingleTargetGui.xaml
    /// </summary>
    public partial class SingleTargetGui
    {
        private readonly OffenseLogicSettings settings;

        public SingleTargetGui(OffenseLogicSettings settings)
        {
            InitializeComponent();

            this.settings = settings;
            DataContext = settings;
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            settings.Save();
        }
    }
}
