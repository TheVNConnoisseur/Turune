
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
using Turune.Services.MenuOptions;
using Turune.Views;

namespace Turune
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly NavigationService _navigation;
        public MainWindow()
        {
            InitializeComponent();
            _navigation = new NavigationService();
            DataContext = _navigation;
            _navigation.NavigateTo(new MainMenuView(_navigation, "Select an engine revision", MainMenuOptions.All)); //Setting up the initial view
        }
    }
}