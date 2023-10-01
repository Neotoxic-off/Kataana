using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Kataana.Views;

namespace Kataana.Models
{
    public class NavigationModel: BaseModel
    {
        private HomeView _homeView; 
        public HomeView HomeView
        {
            get { return (_homeView); }
            set { SetProperty(ref _homeView, value); }
        }

        private SettingsView _settingsView;
        public SettingsView SettingsView
        {
            get { return (_settingsView); }
            set { SetProperty(ref _settingsView, value); }
        }

        private UserControl _contentControl;
        public UserControl ContentControl
        {
            get { return (_contentControl); }
            set { SetProperty(ref _contentControl, value); }
        }

        private Dictionary<string, (UserControl View, object Param)> _bind;
        public Dictionary<string, (UserControl View, object Param)> Bind
        {
            get { return (_bind); }
            set { SetProperty(ref _bind, value); }
        }
    }
}
