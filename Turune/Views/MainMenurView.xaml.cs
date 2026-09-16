using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Turune.Services;
using Turune.Models;

namespace Turune.Views
{
    /// <summary>
    /// Interaction logic for MainMenuView.xaml
    /// </summary>
    public partial class MainMenuView : UserControl
    {
        private readonly NavigationService _navigation;
        private readonly IReadOnlyList<MenuOption> _options;

        public MainMenuView(NavigationService navigation,
            string title,
            IReadOnlyList<MenuOption> options)
        {
            InitializeComponent();
            _navigation = navigation;
            TitleMenu.Text = title;
            OptionList.ItemsSource = options;

            if (options.Count > 0)
                OptionList.SelectedIndex = 0;
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            var selected = (OptionList.SelectedItem as ComboBoxItem)?.Content?.ToString();
            //UserControl next = OptionList switch
            //{
            //    "Version 1" => new OptionSelectorView(_navigation, new List<string> { "Option A", "Option B" }),
            //    "Version 2" => new OptionSelectorView(_navigation, new List<string> { "Option C", "Option D" })
            //};
            //if (next != null) _navigation.NavigateTo(next);
        }

        private void OptionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Next.IsEnabled = true;  //Enable the next view button once an option is selected

            if (OptionList.SelectedItem is not MenuOption option) return;
            DescriptionRenderer.Render(Option_Descriptor.Inlines, option.Description);
        }
    }
}
