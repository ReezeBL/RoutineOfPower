using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using RoutineOfPower.Core.Settings;
using RoutineOfPower.Core.Settings.Misc;

namespace RoutineOfPower.GUI
{
    /// <summary>
    /// Логика взаимодействия для TotemLogicGui.xaml
    /// </summary>
    public partial class TotemLogicGui
    {
        private readonly TotemLogicSettings settings;
        public TotemLogicGui(TotemLogicSettings settings)
        {
            InitializeComponent();
            DataContext = this.settings = settings;
        }

        private void SaveSettings(object sender, RoutedEventArgs e)
        {
            settings.Save();
        }

        private void RefreshSlots(object sender, RoutedEventArgs e)
        {
            settings.CreateSettingsForSlots();
        }
    }

    public class TotemSettingsVisibilityConverter : IValueConverter
    {
        public static readonly TotemSettingsVisibilityConverter Instance = new TotemSettingsVisibilityConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TotemCastType totemCastType) || !(parameter is string stringParameter))
                return Visibility.Collapsed;
            switch (totemCastType)
            {
                case TotemCastType.Support when stringParameter == "Rarity":
                    return Visibility.Visible;
                case TotemCastType.Offensive when stringParameter == "TotemCount" || stringParameter == "Range":
                    return Visibility.Visible;
                default:
                    return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
