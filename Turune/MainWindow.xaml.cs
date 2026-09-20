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
            _navigation.NavigateTo(new MainMenuView(_navigation, "Select an engine revision", MainMenuOptions.All)); //Setting up the initial view

            var dat = new Turune.Models.Formats.Initialization.DAT();
            dat.ParsePacked(System.IO.File.ReadAllBytes("Z:\\VN\\Valkyria\\Nihiki no Marionette ~Kouki Naru Shimai no Songen Houkai~\\system.dat"));
            //ini.ParseUnpacked(System.IO.File.ReadAllLines("C:\\Users\\User\\Downloads\\system_export.ini"));
            //System.IO.File.WriteAllBytes("C:\\Users\\User\\Downloads\\system_export.bin", ini.Generate());

            //System.IO.File.WriteAllLines("C:\\Users\\User\\Downloads\\system_export.ini", ini.Export());
        }
    }
}