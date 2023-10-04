using Kataana.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Runtime.ConstrainedExecution;
using Kataana.Properties;
using System.Reflection;
using System.Windows.Media;
using System.Net.Http;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.Xml.Linq;
using Titanium.Web.Proxy.Network;
using System.Diagnostics;

using Fiddler;

namespace Kataana.ViewModels
{
    public class ProxyViewModel: BaseViewModel
    {
        public ProxyModel ProxyModel { get; set; }

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
        public BloodwebViewModel BloodwebViewModel { get; set; }
        public GetAllViewModel GetAllViewModel { get; set; }
        public OptionsViewModel OptionsViewModel { get; set; }
        private Dictionary<bool, Action<object>> States { get; set; }
        private Dictionary<bool, string> Labels { get; set; }
        private Dictionary<string, Func<SessionEventArgs, SessionEventArgs>> Corrupters { get; set; }
        public DelegateCommand ChangeStateProxyCommand { get; set; }
        private ExplicitProxyEndPoint ExplicitProxyEndPoint { get; set; }

        public ProxyViewModel(AccountViewModel accountViewModel, MarketViewModel marketViewModel, SettingsViewModel settingsViewModel)
        {
            ProxyModel = new ProxyModel()
            {
                ProxyServer = new ProxyServer(true, false, false)
            };
            AccountViewModel = accountViewModel;
            MarketViewModel = marketViewModel;
            SettingsViewModel = settingsViewModel;
            BloodwebViewModel = new BloodwebViewModel();
            GetAllViewModel = new GetAllViewModel();
            OptionsViewModel = new OptionsViewModel();
            States = new Dictionary<bool, Action<object>>()
            {
                { false, StartProxy },
                { true, StopProxy }
            };
            Labels = new Dictionary<bool, string>()
            {
                { false, "Start" },
                { true, "Stop" }
            };
            Corrupters = new Dictionary<string, Func<SessionEventArgs, SessionEventArgs>>()
            {
                { "/v1/inventories", ManipulateInventories },
                { "/v1/dbd-character-data/bloodweb", ManipulateBloodweb },
                { "/v1/dbd-character-data/get-all", ManipulateGetAll }
            };
            ProxyModel.Logs = new ObservableCollection<string>();
            ChangeStateProxyCommand = new DelegateCommand(ChangeState);
            ProxyModel.Label = Labels[ProxyModel.Running];
        }

        private void ChangeState(object data)
        {
            States[ProxyModel.ProxyServer.ProxyRunning](data);
            ProxyModel.Label = Labels[ProxyModel.ProxyServer.ProxyRunning];
        }

        private void StartProxy(object data)
        {
            FiddlerCoreStartupSettings startupSettings = new FiddlerCoreStartupSettingsBuilder()
                .RegisterAsSystemProxy()
                .DecryptSSL()
                .Build();
                
            if (FiddlerApplication.IsStarted() == false)
            {
                ProxyModel.ProxyServer.Startup(startupSettings);
            }

            ProxyModel.ProxyServer.BeforeResponse += Manipulate;

        }

        private void StopProxy(object data)
        {
            ProxyModel.ProxyServer.BeforeResponse -= Manipulate;

            if (ProxyModel.ProxyServer.IsStarted())
                ProxyModel.ProxyServer.Shutdown();
                UninstallCertificate();
            }
        }

        private bool InstallCertificate()
        {
            if (CertMaker.rootCertExists() == false)
            {
                if (CertMaker.createRootCert() == false)
                {
                    return (false);
                }

                if (CertMaker.trustRootCert() == false)
                {
                    return (false);
                }
            }

            return (true);
        }

        private bool UninstallCertificate()
        {
            if (CertMaker.rootCertExists() == true)
            {
                if (CertMaker.removeFiddlerGeneratedCerts(true) == false)
                {
                    return (false);
                }
            }

            return (true);
        }

        private async Task Manipulate(object sender, SessionEventArgs e)
        {
            string[] methods = { "GET", "POST", "PUT" };
            string tmp_cookie = null;

            e.HttpClient.Request.KeepBody = true;
            
            if (e.HttpClient.Request.Host != null)
            {
                Console.WriteLine(e.HttpClient.Request.Url);
                if (methods.Contains(e.HttpClient.Request.Method.ToUpper()) == true)
                {
                    if (e.HttpClient.Request.Url.Contains(".bhvrdbd.com") == true)
                    {
                        tmp_cookie = GetCookie(e);
                        if (tmp_cookie != null && ProxyModel.BHVRSession != tmp_cookie)
                        {
                            ProxyModel.BHVRSession = tmp_cookie;
                        }
                        if (IsCorruptable(e.HttpClient.Request.RequestUri.LocalPath) == true)
                        {
                            Corrupters[e.HttpClient.Request.RequestUri.LocalPath](e);
                        }
                    }
                }
            }
        }

        private bool IsCorruptable(string Url)
        {
            foreach (KeyValuePair<string, Func<SessionEventArgs, SessionEventArgs>> data in Corrupters)
            {
                if (Url == data.Key)
                {
                    return (true);
                }
            }

            return (false);
        }

        private SessionEventArgs ManipulateInventories(SessionEventArgs session)
        {
            string modifiedResponseBody = null;

            if (OptionsViewModel.OptionsModel.UnlockMarket == true)
            {
                modifiedResponseBody = Newtonsoft.Json.JsonConvert.SerializeObject(MarketViewModel.MarketModel.JSONMarketModel);
                session.SetResponseBodyString(modifiedResponseBody);
                session.HttpClient.Response.ContentLength = modifiedResponseBody.Length;
            }

            return (session);
        }

        private SessionEventArgs ManipulateBloodweb(SessionEventArgs session)
        {
            string modifiedResponseBody = null;

            if (OptionsViewModel.OptionsModel.TemporaryUnlock == true)
            {
                BloodwebViewModel.BloodwebModel.JSONBloodwebModel.bloodWebLevel = SettingsViewModel.SettingsModel.JSONSettings.BloodWeb.BloodWebLevel;
                BloodwebViewModel.BloodwebModel.JSONBloodwebModel.legacyPrestigeLevel = SettingsViewModel.SettingsModel.JSONSettings.BloodWeb.LegacyPrestigeLevel;
                BloodwebViewModel.BloodwebModel.JSONBloodwebModel.prestigeLevel = SettingsViewModel.SettingsModel.JSONSettings.BloodWeb.PrestigeLevel;
                foreach (Models.JSON.JSONBloodwebModel.Characteritem character in BloodwebViewModel.BloodwebModel.JSONBloodwebModel.characterItems)
                {
                    character.quantity = SettingsViewModel.SettingsModel.JSONSettings.BloodWeb.ItemQuantity;
                }

                modifiedResponseBody = JsonConvert.SerializeObject(
                    BloodwebViewModel.BloodwebModel.JSONBloodwebModel
                );
                session.SetResponseBodyString(modifiedResponseBody);
                session.HttpClient.Response.ContentLength = modifiedResponseBody.Length;
            }

            return (session);
        }

        private string GetCookie(SessionEventArgs session)
        {
            string token = "bhvrSession";

            if (session.HttpClient.Response.Headers.Headers.Count() > 0)
            {
                if (session.HttpClient.Response.Headers.Headers.ContainsKey(token) == true)
                {
                    return (session.HttpClient.Response.Headers.Headers[token].Value);
                }
            }

            return (null);
        }

        private SessionEventArgs ManipulateGetAll(SessionEventArgs session)
        {
            string modifiedResponseBody = null;

            if (OptionsViewModel.OptionsModel.TemporaryUnlock == true)
            {
                foreach (Models.JSON.JSONGetAllModel.List element in GetAllViewModel.GetAllModel.JSONGetAllModel.list)
                {
                    element.bloodWebLevel = SettingsViewModel.SettingsModel.JSONSettings.BloodWeb.BloodWebLevel;
                    element.legacyPrestigeLevel = SettingsViewModel.SettingsModel.JSONSettings.BloodWeb.LegacyPrestigeLevel;
                    element.prestigeLevel = SettingsViewModel.SettingsModel.JSONSettings.BloodWeb.PrestigeLevel;
                    foreach (Models.JSON.JSONGetAllModel.Characteritem item in element.characterItems)
                    {
                        item.quantity = SettingsViewModel.SettingsModel.JSONSettings.BloodWeb.ItemQuantity;
                    }
                }
                
                modifiedResponseBody = JsonConvert.SerializeObject(
                    GetAllViewModel.GetAllModel.JSONGetAllModel
                );
                session.SetResponseBodyString(modifiedResponseBody);
                session.HttpClient.Response.ContentLength = modifiedResponseBody.Length;
            }

            return (session);
        }
    }
}
