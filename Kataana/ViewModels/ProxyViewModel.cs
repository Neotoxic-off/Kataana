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
        public PlayernameViewModel PlayernameViewModel { get; set; }
        public GetAllViewModel GetAllViewModel { get; set; }
        public OptionsViewModel OptionsViewModel { get; set; }
        private Dictionary<bool, Action<object>> States { get; set; }
        private Dictionary<bool, string> Labels { get; set; }
        private Dictionary<string, Func<Session, Session>> Corrupters { get; set; }
        private Dictionary<string, Func<Session, Session>> BeforeCorrupters { get; set; }
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
            PlayernameViewModel = new PlayernameViewModel();
            
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
                { "/api/v1/playername", ManipulatePlayername },
                { "/api/v1/dbd-character-data/bloodweb", ManipulateBloodweb },
                { "/api/v1/dbd-character-data/get-all", ManipulateGetAll },
                { "/api/v1/auth/provider/egs/login", ManipulateLogin },
                { "/api/v1/me/location", ManipulateLocation },
                { "/api/v1/clientVersion/check", ManipulateClientVersion },
                { "/api/v1/matchIncentives", ManipulateIncentives }
            };
            BeforeCorrupters = new Dictionary<string, Func<Session, Session>>()
            {
                { "/api/v1/gameLogs/batch", ManipulateAnalytics },
                { "/api/v1/gameDataAnalytics/v2/batch", ManipulateAnalytics }
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
            string key = null;

            if (e.oRequest.host != null)
            {
                if (e.hostname.Contains(".bhvrdbd.com") == true)
                {
                    e.bBufferResponse = true;
                    key = IsBeforeCorruptable(e.PathAndQuery);
                    if (key != null)
                    {
                        e.bBufferResponse = true;
                        BeforeCorrupters[key](e);
                    }
                }
            }
        }

        private void Manipulate(Session e)
        {
            string key = null;

            if (e.oRequest.host != null)
            {
                if (e.hostname.Contains(".bhvrdbd.com") == true)
                {
                    Cookie(e);
                    key = IsCorruptable(e.PathAndQuery);
                    if (key != null)
                    {
                        e.bBufferResponse = true;
                        Corrupters[key](e);
                    }
                }
            }
        }

        private string IsBeforeCorruptable(string Url)
        {
            string key = null;

            foreach (KeyValuePair<string, Func<Session, Session>> data in BeforeCorrupters)
            {
                if (Url.Contains(data.Key) == true)
                {
                    return (data.Key);
                }
            }

            return (null);
        }

        private string IsCorruptable(string Url)
        {
            string key = null;

            foreach (KeyValuePair<string, Func<Session, Session>> data in Corrupters)
            {
                if (Url.Contains(data.Key) == true)
                {
                    return (data.Key);
                }
            }

            return (null);
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

        private Session ManipulateLogin(Session session)
        {
            session.bBufferResponse = true;
            session.utilDecodeResponse();

            if (OptionsViewModel.OptionsModel.BypassLogin == true)
            {
                session.utilSetResponseBody("{\"preferredLanguage\":\"en\",\"friendsFirstSync\":{\"egs\":true},\"creationDate\":1656237604,\"fixedMyFriendsUserPlatformId\":{\"egs\":true},\"id\":\"7b3752e0-869b-497f-93e0-3810e2ae7895\",\"provider\":{\"providerId\":\"0244ad2c01de4e7bb311813ec4651ee0\",\"providerName\":\"egs\",\"userId\":\"7b3752e0-869b-497f-93e0-3810e2ae7895\"},\"providers\":[{\"providerName\":\"egs\",\"providerId\":\"0244ad2c01de4e7bb311813ec4651ee0\"}],\"friends\":[],\"triggerResults\":{\"success\":[],\"error\":[]},\"tokenId\":\"8e8a5ece-b2ba-4c66-946c-9a4ce1f4ad7b\",\"generated\":1697129977,\"expire\":1697216377,\"userId\":\"7b3752e0-869b-497f-93e0-3810e2ae7895\",\"endpointName\":\"egs\",\"providerName\":\"egs\",\"token\":\"8e8a5ece-b2ba-4c66-946c-9a4ce1f4ad7b\"}");
            }

            return (session);
        }

        private Session ManipulatePlayername(Session session)
        {
            string modifiedResponseBody = null;

            if (OptionsViewModel.OptionsModel.SpoofPlayername == true)
            {
                session.bBufferResponse = true;
                session.utilDecodeResponse();
                if (session.PathAndQuery.EndsWith("/api/v1/playername") == false)
                {
                    session.PathAndQuery = $"/api/v1/playername/egs/neo";
                }
                this.PlayernameViewModel.PlayernameModel.JSONPlayername = JsonConvert.DeserializeObject<Models.JSON.JSONPlayername>(
                    session.GetResponseBodyAsString()
                );
                this.PlayernameViewModel.PlayernameModel.JSONPlayername.playerName = "neo#3773";
                this.PlayernameViewModel.PlayernameModel.JSONPlayername.providerPlayerNames.egs = "neo";
                this.PlayernameViewModel.PlayernameModel.JSONPlayername.userId = "8822b6e1-15ec-478c-80c4-bb1745c76670";
                modifiedResponseBody = JsonConvert.SerializeObject(
                    this.PlayernameViewModel.PlayernameModel.JSONPlayername
                );
                session.utilSetResponseBody(modifiedResponseBody);
            }

            return (session);
        }

        private Session ManipulateAnalytics(Session session)
        {
            if (OptionsViewModel.OptionsModel.BypassAnalytics == true)
            {
                session.bBufferResponse = true;
                session.utilDecodeResponse();

                session.utilSetRequestBody("{}");
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

        private Session ManipulateLocation(Session session)
        {
            if (OptionsViewModel.OptionsModel.FakeLocation == true)
            {
                session.bBufferResponse = true;
                session.utilDecodeResponse();

                session.utilSetResponseBody("{\"continent\":\"EU\",\"country\":\"FR\",\"city\":\"Paris\"}");
            }

            return (session);
        }

        private Session ManipulateClientVersion(Session session)
        {
            if (OptionsViewModel.OptionsModel.ForceClientValidation == true)
            {
                session.bBufferResponse = true;
                session.utilDecodeResponse();

                session.utilSetResponseBody("{\"isValid\":true}");
            }

            return (session);
        }

        private Session ManipulateIncentives(Session session)
        {
            if (OptionsViewModel.OptionsModel.ForceIncentives == true)
            {
                session.bBufferResponse = true;
                session.utilDecodeResponse();

                session.utilSetResponseBody("{\"killerPercentageIncentive\":100,\"survivorPercentageIncentive\":100,\"refreshTime\":1800}");
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
    }
}
