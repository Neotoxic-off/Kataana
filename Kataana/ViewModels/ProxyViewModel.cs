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
using System.Diagnostics;

using Fiddler;

namespace Kataana.ViewModels
{
    public class ProxyViewModel : BaseViewModel
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
        private Dictionary<string, Func<Session, Session>> Corrupters { get; set; }
        public DelegateCommand ChangeStateProxyCommand { get; set; }

        public ProxyViewModel(AccountViewModel accountViewModel, MarketViewModel marketViewModel, SettingsViewModel settingsViewModel)
        {
            ProxyModel = new ProxyModel();
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
            Corrupters = new Dictionary<string, Func<Session, Session>>()
            {
                { "/api/v1/inventories", ManipulateInventories },
                { "/api/v1/dbd-character-data/bloodweb", ManipulateBloodweb },
                { "/api/v1/dbd-character-data/get-all", ManipulateGetAll }
            };
            ProxyModel.Logs = new ObservableCollection<string>();
            ChangeStateProxyCommand = new DelegateCommand(ChangeState);
            ProxyModel.Label = Labels[ProxyModel.Running];
        }

        private void ChangeState(object data)
        {
            States[FiddlerApplication.IsStarted()](data);
            ProxyModel.Label = Labels[FiddlerApplication.IsStarted()];
        }

        private void StartProxy(object data)
        {
            FiddlerCoreStartupSettings startupSettings = new FiddlerCoreStartupSettingsBuilder()
                .RegisterAsSystemProxy()
                .DecryptSSL()
                .Build();

            if (FiddlerApplication.IsStarted() == false)
            {
                InstallCertificate();
                FiddlerApplication.Startup(startupSettings);
            }

            FiddlerApplication.BeforeRequest += BeforeRequest;
            FiddlerApplication.BeforeResponse += Manipulate;
        }

        private void StopProxy(object data)
        {
            FiddlerApplication.BeforeRequest -= BeforeRequest;
            FiddlerApplication.BeforeResponse -= Manipulate;

            if (FiddlerApplication.IsStarted() == true)
            {
                FiddlerApplication.Shutdown();
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

        private void BeforeRequest(Session e)
        {
            e.bBufferResponse = true;
        }

        private void Manipulate(Session e)
        {
            if (e.oRequest.host != null)
            {
                if (e.fullUrl.Contains(".bhvrdbd.com") == true)
                {
                    Cookie(e);
                    if (IsCorruptable(e.PathAndQuery) == true)
                    {
                        e.bBufferResponse = true;
                        Corrupters[e.PathAndQuery](e);
                    }
                }
            }
        }

        private bool IsCorruptable(string Url)
        {
            foreach (KeyValuePair<string, Func<Session, Session>> data in Corrupters)
            {
                if (Url == data.Key)
                {
                    return (true);
                }
            }

            return (false);
        }

        private Session ManipulateInventories(Session session)
        {
            string modifiedResponseBody = null;

            if (OptionsViewModel.OptionsModel.UnlockMarket == true)
            {
                session.bBufferResponse = true;
                session.utilDecodeResponse();
                modifiedResponseBody = JsonConvert.SerializeObject(
                    MarketViewModel.MarketModel.JSONMarketModel
                );
                session.utilSetResponseBody(modifiedResponseBody);
            }

            return (session);
        }

        private Session ManipulateBloodweb(Session session)
        {
            string modifiedResponseBody = null;

            if (OptionsViewModel.OptionsModel.TemporaryUnlock == true)
            {
                session.bBufferResponse = true;
                session.utilDecodeResponse();

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
                session.utilSetResponseBody(modifiedResponseBody);
            }

            return (session);
        }

        private void Cookie(Session session)
        {
            string token = "bhvrSession=";

            if (session.RequestHeaders.ToString().Contains(token) == true)
            {
                ProxyModel.BHVRSession = session.RequestHeaders["Cookie"].ToString().Replace(token, String.Empty);
            }
        }

        private Session ManipulateGetAll(Session session)
        {
            string modifiedResponseBody = null;

            if (OptionsViewModel.OptionsModel.TemporaryUnlock == true)
            {
                session.bBufferResponse = true;
                session.utilDecodeResponse();
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
                session.utilSetResponseBody(modifiedResponseBody);
            }

            return (session);
        }
    }
}
