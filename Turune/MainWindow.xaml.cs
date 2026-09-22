using System.Windows;
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
            _navigation.NavigateTo(new MainMenuView(_navigation, "Select an engine revision", MainMenuOptions.All, true)); //Setting up the initial view

            var odn = new Turune.Models.Formats.Containers.ODN();
            odn.ParsePacked(System.IO.File.ReadAllBytes("Z:\\VN\\Valkyria\\Masou no Kuni no Alice - Alice in Immoral-Land\\data07.odn"));
            //System.IO.File.WriteAllLines("C:\\Users\\User\\Downloads\\system_export.ini", dat.Export());

            //dat.ParseUnpacked(System.IO.File.ReadAllLines("C:\\Users\\User\\Downloads\\system_export.ini"));
            //System.IO.File.WriteAllBytes("C:\\Users\\User\\Downloads\\system_export.bin", dat.Generate());

            
        }
    }
}