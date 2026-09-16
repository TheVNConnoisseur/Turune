using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Turune.Services;

namespace Turune.Views
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        private readonly NavigationService _nav;

        public HomeView(NavigationService nav)
        {
            InitializeComponent();
            _nav = nav;
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            var selected = (OptionPicker.SelectedItem as ComboBoxItem)?.Content?.ToString();
            UserControl next = selected switch
            {
                "Option A" => new TemplateView(_nav),
                "Option B" => new TemplateView(_nav)
            };
            if (next != null) _nav.NavigateTo(next);
        }

        private void OptionPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Next.IsEnabled = true;

            Option_Descriptor.Text = (OptionPicker.SelectedItem as ComboBoxItem)?.Content?.ToString() switch
            {
                "Option A" => "Description for Option A",
                "Option B" => "Description for Option B"
            };
        }
    }
}
