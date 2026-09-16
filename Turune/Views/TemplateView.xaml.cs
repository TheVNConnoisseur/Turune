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
    /// Interaction logic for TemplateView.xaml
    /// </summary>
    public partial class TemplateView : UserControl
    {
        private readonly NavigationService _navigation;
        public TemplateView(NavigationService navigation)
        {
            InitializeComponent();
            _navigation = navigation;
        }

        private void Next_Click(object sender, RoutedEventArgs e)
            => _navigation.NavigateTo(new TemplateView(_navigation));

        private void Back_Click(object sender, RoutedEventArgs e)
            => _navigation.GoBack();
    }
}
