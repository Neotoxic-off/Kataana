using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Kataana.Models;
using Kataana.Views;
using Newtonsoft.Json;

namespace Kataana.ViewModels
{
    public class AccountViewModel : BaseViewModel
    {
        public AccountModel AccountModel { get; set; }
        public SettingsViewModel SettingsViewModel { get; set; }
        public DelegateCommand SaveAccountCommand { get; set; }

        public AccountViewModel(UserControl settingsView)
        {
            SettingsViewModel = (SettingsViewModel)settingsView.DataContext;

            SaveAccountCommand = new DelegateCommand(SaveAccount);
        }

        private void SaveAccount(object data)
        {
            SettingsViewModel.SaveSettingsCommand.Execute(null);
        }
    }
}
