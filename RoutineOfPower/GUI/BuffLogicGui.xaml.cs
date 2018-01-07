using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using RoutineOfPower.Core.Settings;
using RoutineOfPower.Core.Settings.Misc;

namespace RoutineOfPower.GUI
{
    /// <summary>
    /// Логика взаимодействия для BuffLogicGui.xaml
    /// </summary>
    public partial class BuffLogicGui
    {
        private readonly BuffLogicSettings settings;

        public BuffLogicGui(BuffLogicSettings settings)
        {
            InitializeComponent();
            DataContext = this.settings = settings;
        }

        private void RemoveAuraTrigger(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var trigger = (TriggerSettings)button.DataContext;
            var aura = (VaalAuraInfo)button.Tag;
            aura.Triggers.Remove(trigger);
        }

        private void AddAuraTrigger(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var areas = (ObservableCollection<TriggerSettings>)button.Tag;
            areas.Add(new TriggerSettings());
        }

        private void SaveSettings(object sender, RoutedEventArgs e)
        {
            settings.Save();
        }
    }

    public class DescriptionConverter : IValueConverter
    {
        public static readonly DescriptionConverter Instance = new DescriptionConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "null";

            var field = value.GetType().GetField(value.ToString());
            foreach (var attrib in field.GetCustomAttributes(false))
            {
                if (attrib is DescriptionAttribute desc) return desc.Description;
            }
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class VisibilityConverter : IValueConverter
    {
        public static readonly VisibilityConverter Instance = new VisibilityConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var trigger = (TriggerType)value;
            var param = (string)parameter;

            if (trigger == TriggerType.Hp)
                return param == "Hp" ? Visibility.Visible : Visibility.Collapsed;

            if (trigger == TriggerType.Es)
                return param == "Es" ? Visibility.Visible : Visibility.Collapsed;

            if (trigger == TriggerType.Monsters)
                return param == "Mobs" || param == "MobsOrAttack" ? Visibility.Visible : Visibility.Collapsed;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
