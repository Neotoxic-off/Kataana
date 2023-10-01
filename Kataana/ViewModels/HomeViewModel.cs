using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Kataana.Models;

namespace Kataana.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public ProxyViewModel ProxyViewModel { get; set; }

        private AccountViewModel _accountViewModel;
        public AccountViewModel AccountViewModel
        {
            get { return _accountViewModel; }
            set { SetProperty(ref _accountViewModel, value); }
        }

        private MarketViewModel _marketViewModel;
        public MarketViewModel MarketViewModel
        {
            get { return _marketViewModel; }
            set { SetProperty(ref _marketViewModel, value); }
        }

        private SettingsViewModel _settingsViewModel;
        public SettingsViewModel SettingsViewModel
        {
            get { return _settingsViewModel; }
            set { SetProperty(ref _settingsViewModel, value); }
        }

        public HomeViewModel(UserControl accountViewModel, UserControl marketViewModel, UserControl settingsViewModel)
        {
            AccountViewModel = (AccountViewModel)accountViewModel.DataContext;
            MarketViewModel = (MarketViewModel)marketViewModel.DataContext;
            SettingsViewModel = (SettingsViewModel)settingsViewModel.DataContext;

            ProxyViewModel = new ProxyViewModel(AccountViewModel, MarketViewModel, SettingsViewModel);
        }
    }
}
