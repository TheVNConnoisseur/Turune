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
            IReadOnlyList<MenuOption> options,
            bool isRootMenu = false)
        {
            InitializeComponent();
            _navigation = navigation;
            TitleMenu.Text = title;
            OptionList.ItemsSource = options;
            BackButton.Visibility =                 //Hide or show the back button depending on the menu stack status
                isRootMenu
                    ? Visibility.Collapsed
                    : Visibility.Visible;

            if (options.Count > 0)
                OptionList.SelectedIndex = 0;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
            => _navigation.GoBack();

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (OptionList.SelectedItem is not MenuOption selected) return;

            if (selected.Execute != null)
            {
                selected.Execute.Invoke();
                return;
            }

            if (selected.HasChildren)
            {
                _navigation.NavigateTo(new MainMenuView(
                    _navigation,
                    "Select a file format",
                    options: selected.Children,
                    false));
            }
        }

        private void OptionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OptionList.SelectedItem is not MenuOption option) return;
            Next.IsEnabled = true;  //Enable the next view button once an option is selected
            DescriptionRenderer.Render(Option_Descriptor.Inlines, option.Description);
        }
    }
}
