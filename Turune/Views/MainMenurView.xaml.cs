using System.Windows;
using System.Windows.Controls;
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
            if (OptionList.SelectedItem is not MenuOption selected) return;
            if (!selected.HasChildren) return;

            _navigation.NavigateTo(new OptionSelectorView(
                _navigation,
                options: selected.Children));
        }

        private void OptionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OptionList.SelectedItem is not MenuOption option) return;
            Next.IsEnabled = true;  //Enable the next view button once an option is selected
            DescriptionRenderer.Render(Option_Descriptor.Inlines, option.Description);
        }
    }
}
