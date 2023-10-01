using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Kataana.Models;
using Kataana.Views;

namespace Kataana.ViewModels
{
    public class NavigationViewModel: BaseViewModel
    {
        public NavigationModel NavigationModel { get; set; }
        public UserControl AccountView { get; set; }
        public UserControl MarketView { get; set; }
        public UserControl SettingsView { get; set; }
        public UserControl HomeView { get; set; }

        public NavigationViewModel()
        {
            HomeView = new HomeView();
            AccountView = new AccountView();
            MarketView = new MarketView();
            SettingsView = new SettingsView();

            NavigationModel = new NavigationModel()
            {
                Bind = new Dictionary<string, (UserControl view, object param)>()
                {
                    { "Home", (HomeView, null) },
                    { "Account", (AccountView, null) },
                    { "Market", (MarketView, null) },
                    { "Settings", (SettingsView, null) }
                },
            };

            Navigate("Home");
        }

        public void Navigate(object param)
        {
            NavigationModel.ContentControl = NavigationModel.Bind[$"{param}"].View;
        }
    }
}
