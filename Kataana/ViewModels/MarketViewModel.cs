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
    public class MarketViewModel : BaseViewModel
    {
        public MarketModel MarketModel { get; set; }
        private SettingsViewModel SettingsViewModel { get; set; }
        private Tools.Requests Client { get; set; }

        public MarketViewModel()
        {
            SettingsViewModel = new SettingsViewModel();
            MarketModel = new MarketModel();

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
            LoadMarket(File.ReadAllText(SettingsViewModel.SettingsModel.JSONSettings.MarketFile));
            if (await IsUpdated() == false)
            {
                WriteMarket(latest);
            }
        }

        private void WriteMarket(string content)
        {
            if (File.Exists(SettingsViewModel.SettingsModel.JSONSettings.MarketFile) == true)
            {
                File.Delete(SettingsViewModel.SettingsModel.JSONSettings.MarketFile);
            }

            File.WriteAllText(
                SettingsViewModel.SettingsModel.JSONSettings.MarketFile,
                content
            );
        }

        private void LoadMarket(string content)
        {
            MarketModel.JSONMarketModel = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.JSON.JSONMarketModel>(content);
        }

        private async Task<bool> IsUpdated()
        {
            #if DEBUG
                Console.WriteLine("Mode=Debug");
            #else
                string latest = await DownloadVersion();
                Models.JSON.JSONMarketModel current = MarketModel.JSONMarketModel;
                if (latest != null)
                {
                    return (current.updated == latest.Replace("\n", string.Empty));
                }
            #endif

            return (true);
        }

        private bool IsPresent()
        {
            return (File.Exists(SettingsViewModel.SettingsModel.JSONSettings.MarketFile));
        }

        private async Task<string> DownloadMarket()
        {
            return (await Client.Get(SettingsViewModel.SettingsModel.JSONSettings.MarketFlux, null));
        }

        private async Task<string> DownloadVersion()
        {
            return (await Client.Get(SettingsViewModel.SettingsModel.JSONSettings.VersionFlux, null));
        }
    }
}
