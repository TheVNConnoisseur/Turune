
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Turune.Services;
using Turune.Views;

namespace Turune
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var nav = new Services.NavigationService(); //Putting Services to avoid ambiguity
            DataContext = nav;
            nav.NavigateTo(new HomeView(nav)); //Setting up the initial view
        }
    }
}