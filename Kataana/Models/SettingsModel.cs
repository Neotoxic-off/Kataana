using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kataana.Models
{
    public class SettingsModel: BaseModel
    {
        private AssemblyModel _assembly;
        public AssemblyModel Assembly
        {
            get { return _assembly; }
            set { SetProperty(ref _assembly, value); }
        }

        private string _settingsFile;
        public string SettingsFile
        {
            get { return _settingsFile; }
            set { SetProperty(ref _settingsFile, value); }
        }

        private JSON.JSONSettingsModel _JSONsettings;
        public JSON.JSONSettingsModel JSONSettings
        {
            get { return _JSONsettings; }
            set { SetProperty(ref _JSONsettings, value); }
        }
    }
}
