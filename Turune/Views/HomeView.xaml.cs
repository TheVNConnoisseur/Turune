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
                "Version 1" => new TemplateView(_nav),
                "Version 2" => new TemplateView(_nav)
            };
            if (next != null) _nav.NavigateTo(next);
        }

        private void OptionPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Next.IsEnabled = true;  //Enable the next view button once an option is selected

            Option_Descriptor.Inlines.Clear();  //Clear the previous option description

            var selected = (OptionPicker.SelectedItem as ComboBoxItem)?.Content?.ToString();
            
            switch (selected)
            {
                case "Version 1":
                    var v2354 = new Hyperlink(new Run("Imouto Ninshin"))
                    {
                        NavigateUri = new Uri("https://vndb.org/v2354")
                    };
                    v2354.RequestNavigate += (s, e) =>
                    {
                        Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
                        e.Handled = true;
                    };
                    var v19908 = new Hyperlink(new Run("Netoraren'ai ~Ai Suru Kanojo ga Musunda Midara na Keiyaku~"))
                    {
                        NavigateUri = new Uri("https://vndb.org/v2354")
                    };
                    v19908.RequestNavigate += (s, e) =>
                    {
                        Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
                        e.Handled = true;
                    };
                    Option_Descriptor.Inlines.Add(new Run("For the games released in between "));
                    Option_Descriptor.Inlines.Add(v2354);
                    Option_Descriptor.Inlines.Add(" and ");
                    Option_Descriptor.Inlines.Add(v19908);
                    Option_Descriptor.Inlines.Add("(both included).");
                    Option_Descriptor.Inlines.Add(new LineBreak());
                    Option_Descriptor.Inlines.Add(new Run("Compatibility for the earlier games is not guaranteed, " +
                        "since the engine went under a lot of changes during the initial days of Valkyria."));
                    break;

                case "Version 2":
                    var v20364 = new Hyperlink(new Run("Kangokujou ~Ingyaku no Shihai~"))
                    {
                        NavigateUri = new Uri("https://vndb.org/v20364")
                    };
                    v20364.RequestNavigate += (s, e) =>
                    {
                        Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
                        e.Handled = true;
                    };
                    Option_Descriptor.Inlines.Add(new Run("Starting from "));
                    Option_Descriptor.Inlines.Add(v20364);
                    Option_Descriptor.Inlines.Add(", this is the latest main version of the MICo engine. " +
                        "It is important to understand that most of the files themselves have gone under several iterations " +
                        "in this same version.");
                    break;
            }
        }
    }
}
