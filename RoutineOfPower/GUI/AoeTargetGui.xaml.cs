using System.Windows;
using System.Windows.Controls;
using RoutineOfPower.Core.Settings;

namespace RoutineOfPower.GUI
{
    /// <summary>
    /// Логика взаимодействия для AoeTargetGui.xaml
    /// </summary>
    public partial class AoeTargetGui : UserControl
    {
        private readonly AoeTargetLogicSettings settings;

        public AoeTargetGui(AoeTargetLogicSettings settings)
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
