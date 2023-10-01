using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Kataana.Models;

namespace Kataana.ViewModels
{
    public class BloodwebViewModel : BaseViewModel
    {
        public BloodwebModel BloodwebModel { get; set; }
        private SettingsViewModel SettingsViewModel { get; set; }
        private Tools.Requests Client { get; set; }

        public BloodwebViewModel()
        {
            SettingsViewModel = new SettingsViewModel();
            BloodwebModel = new BloodwebModel();

            Client = new Tools.Requests();

            Load();
        }

        private async void Load()
        {
            string latest = await DownloadMarket();

            if (IsPresent() == false)
            {
                WriteMarket(latest);
            }
            LoadMarket(File.ReadAllText(SettingsViewModel.SettingsModel.JSONSettings.BloodwebFile));
            if (await IsUpdated() == false)
            {
                WriteMarket(latest);
            }
        }

        private void WriteMarket(string content)
        {
            if (File.Exists(SettingsViewModel.SettingsModel.JSONSettings.BloodwebFile) == true)
            {
                File.Delete(SettingsViewModel.SettingsModel.JSONSettings.BloodwebFile);
            }

            File.WriteAllText(
                SettingsViewModel.SettingsModel.JSONSettings.BloodwebFile,
                content
            );
        }

        private void LoadMarket(string content)
        {
            BloodwebModel.JSONBloodwebModel = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.JSON.JSONBloodwebModel>(content);
        }

        private async Task<bool> IsUpdated()
        {
            string latest = await DownloadVersion();
            Models.JSON.JSONBloodwebModel current = BloodwebModel.JSONBloodwebModel;

            if (latest != null)
            {
                return (current.updated == latest.Replace("\n", string.Empty));
            }

            return (true);
        }

        private bool IsPresent()
        {
            return (File.Exists(SettingsViewModel.SettingsModel.JSONSettings.BloodwebFile));
        }

        private async Task<string> DownloadMarket()
        {
            return (await Client.Get(SettingsViewModel.SettingsModel.JSONSettings.BloodwebFlux, null));
        }

        private async Task<string> DownloadVersion()
        {
            return (await Client.Get(SettingsViewModel.SettingsModel.JSONSettings.VersionFlux, null));
        }
    }
}
