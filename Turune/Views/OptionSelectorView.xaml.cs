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
using Turune.Models;
using Turune.Services;

namespace Turune.Views
{
    /// <summary>
    /// Interaction logic for OptionSelectorView.xaml
    /// </summary>
    public partial class OptionSelectorView : UserControl
    {
        private readonly NavigationService _navigation;
        private readonly List<MenuOption> _options;

        private Button _selectedGroup1Button;
        private Button _selectedGroup2Button;
        private MenuOption _selectedOption;
        //private SubOption _selectedSubOption;

        public OptionSelectorView(NavigationService navigation, IReadOnlyList<MenuOption> options)
        {
            InitializeComponent();
            _navigation = navigation;

            OptionsList.ItemsSource = options;
        }
        private void Back_Click(object sender, RoutedEventArgs e)
            => _navigation.GoBack();
        private void Next_Click(object sender, RoutedEventArgs e)
        {
            //if (_selectedSubOption?.NextViewFactory == null) return;
            //_navigation.NavigateTo(_selectedSubOption.NextViewFactory(_navigation));
        }
    }
}
