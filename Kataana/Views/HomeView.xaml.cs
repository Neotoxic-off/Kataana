using Kataana.Models;
using Kataana.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Kataana.Views
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        public HomeViewModel HomeViewModel { get; set; }

        public HomeView(UserControl accountView, UserControl marketView, UserControl settingsView)
        {
            InitializeComponent();

            DataContext = new HomeViewModel(accountView, marketView, settingsView);
        }
    }
}
