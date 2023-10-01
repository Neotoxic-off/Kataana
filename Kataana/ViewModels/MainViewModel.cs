using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Kataana.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public DelegateCommand ExitCommand { get; set; }
        public DelegateCommand MinimizeCommand { get; set; }
        public DelegateCommand NavigateCommand { get; set; }

        public AssemblyViewModel AssemblyViewModel { get; set; }
        public NavigationViewModel NavigationViewModel { get; set; }
        
        public MainViewModel()
        {
            LoadViewModels();
            LoadCommands();
        }

        private void LoadViewModels()
        {
            AssemblyViewModel = new AssemblyViewModel();
            NavigationViewModel = new NavigationViewModel();
        }

        private void LoadCommands()
        {
            ExitCommand = new DelegateCommand(Exit);
            MinimizeCommand = new DelegateCommand(Minimize);
            NavigateCommand = new DelegateCommand(NavigationViewModel.Navigate);
        }

        private void Minimize(object data)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void Exit(object data)
        {
            Application.Current.Shutdown();
        }
    }
}
