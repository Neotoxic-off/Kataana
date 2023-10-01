using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Kataana.Models;

using Newtonsoft.Json;

namespace Kataana.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public SettingsModel SettingsModel { get; set; }
        private AssemblyViewModel assemblyViewModel { get; set; }
        public DelegateCommand SaveSettingsCommand { get; set; }

        public SettingsViewModel()
        {
            assemblyViewModel = new AssemblyViewModel();

            SettingsModel = new SettingsModel()
            {
                Assembly = assemblyViewModel.Assembly,
                SettingsFile = "settings.json",
                JSONSettings = null
            };

            SaveSettingsCommand = new DelegateCommand(SaveSettings);

            LoadSettings();
        }

        private void LoadSettings()
        {
            if (File.Exists(SettingsModel.SettingsFile) == true)
            {
                SettingsModel.JSONSettings = JsonConvert.DeserializeObject<Models.JSON.JSONSettingsModel>(
                    File.ReadAllText(
                        SettingsModel.SettingsFile,
                        Encoding.UTF8
                    )
                );
            }
        }

        private void SaveSettings(object data)
        {
            if (File.Exists(SettingsModel.SettingsFile) == true)
            {
                File.Delete(SettingsModel.SettingsFile);
            }

            File.WriteAllText(
                SettingsModel.SettingsFile,
                JsonConvert.SerializeObject(
                    SettingsModel.JSONSettings
                ),
                Encoding.UTF8
            );
        }
    }
}
