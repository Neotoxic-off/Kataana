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
    public class GetAllViewModel : BaseViewModel
    {
        public GetAllModel GetAllModel { get; set; }
        private SettingsViewModel SettingsViewModel { get; set; }
        private Tools.Requests Client { get; set; }

        public GetAllViewModel()
        {
            SettingsViewModel = new SettingsViewModel();
            GetAllModel = new GetAllModel();

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
            LoadMarket(File.ReadAllText(SettingsViewModel.SettingsModel.JSONSettings.GetAllFile));
            if (await IsUpdated() == false)
            {
                WriteMarket(latest);
            }
        }

        private void WriteMarket(string content)
        {
            if (File.Exists(SettingsViewModel.SettingsModel.JSONSettings.GetAllFile) == true)
            {
                File.Delete(SettingsViewModel.SettingsModel.JSONSettings.GetAllFile);
            }

            File.WriteAllText(
                SettingsViewModel.SettingsModel.JSONSettings.GetAllFile,
                content
            );
        }

        private void LoadMarket(string content)
        {
            GetAllModel.JSONGetAllModel = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.JSON.JSONGetAllModel>(content);
        }

        private async Task<bool> IsUpdated()
        {
            string latest = await DownloadVersion();
            Models.JSON.JSONGetAllModel current = GetAllModel.JSONGetAllModel;

            if (latest != null)
            {
                if (current != null)
                {
                    return (current.updated == latest.Replace("\n", string.Empty));

                }
            }

            return (true);
        }

        private bool IsPresent()
        {
            return (File.Exists(SettingsViewModel.SettingsModel.JSONSettings.GetAllFile));
        }

        private async Task<string> DownloadMarket()
        {
            return (await Client.Get(SettingsViewModel.SettingsModel.JSONSettings.GetAllFlux, null));
        }

        private async Task<string> DownloadVersion()
        {
            return (await Client.Get(SettingsViewModel.SettingsModel.JSONSettings.VersionFlux, null));
        }
    }
}
