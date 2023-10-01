using Kataana.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Http;
using Titanium.Web.Proxy.Models;
using System.IO;
using System.Runtime.ConstrainedExecution;
using Kataana.Properties;
using System.Reflection;
using System.Windows.Media;
using System.Net.Http;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

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
        public BloodwebViewModel BloodwebViewModel { get; set; }
        public GetAllViewModel GetAllViewModel { get; set; }
        public OptionsViewModel OptionsViewModel { get; set; }
        private Dictionary<bool, Action<object>> States { get; set; }
        private Dictionary<bool, string> Labels { get; set; }
        private Dictionary<string, Func<SessionEventArgs, SessionEventArgs>> Corrupters { get; set; }
        public DelegateCommand ChangeStateProxyCommand { get; set; }

        public ProxyViewModel(AccountViewModel accountViewModel, MarketViewModel marketViewModel)
        {
            ProxyModel = new ProxyModel()
            {
                ProxyServer = new ProxyServer(true, true, true),
                CertificatePath = "certificate.pfx",
                CertificatePassword = "Kataana"
            };
            AccountViewModel = accountViewModel;
            MarketViewModel = marketViewModel;
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
            ProxyModel.Running = false;
            ChangeStateProxyCommand = new DelegateCommand(ChangeState);
            ProxyModel.Label = Labels[ProxyModel.Running];

            ProxyModel.ProxyServer.CertificateManager.CertificateEngine = Titanium.Web.Proxy.Network.CertificateEngine.DefaultWindows;
            ProxyModel.ProxyServer.CertificateManager.EnsureRootCertificate();
            ProxyModel.ProxyServer.CertificateManager.TrustRootCertificate(true);

            TransparentProxyEndPoint transparentEndPoint = new TransparentProxyEndPoint(IPAddress.Any, 8001, true)
            {
                GenericCertificateName = "google.com"
            };

            ProxyModel.ProxyServer.AddEndPoint(transparentEndPoint);
        }

        private void ChangeState(object data)
        {
            States[ProxyModel.Running](data);
            ProxyModel.Running = !ProxyModel.Running;
            ProxyModel.Label = Labels[ProxyModel.Running];
        }

        private void StartProxy(object data)
        {
            ProxyModel.ProxyServer.BeforeResponse += Manipulate;

            ProxyModel.ProxyServer.Start();
            ExplicitProxyEndPoint explicitEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, 8000, true)
            {
                
            };
            ProxyModel.ProxyServer.AddEndPoint(
                explicitEndPoint
            );
            ProxyModel.ProxyServer.SetAsSystemHttpProxy(explicitEndPoint);
            ProxyModel.ProxyServer.SetAsSystemHttpsProxy(explicitEndPoint);
        }

        private void StopProxy(object data)
        {
            ProxyModel.ProxyServer.BeforeResponse -= Manipulate;
            ProxyModel.ProxyServer.Stop();
        }

        private async Task Manipulate(object sender, SessionEventArgs e)
        {
            string[] methods = { "GET", "POST", "PUT" };

            if (methods.Contains(e.HttpClient.Request.Method.ToUpper()) == true)
            {
                if (e.HttpClient.Request.Url.Contains("bhvrdbd") == true)
                {
                    if (IsCorruptable(e.HttpClient.Request.RequestUri.LocalPath) == true)
                    {
                        Corrupters[e.HttpClient.Request.RequestUri.LocalPath](e);
                    }
                }
            }
            
            //if (sess.fullUrl.Contains("v1/dbd-character-data/bloodweb") == true && variables.manager.get_switch_button(switch_savefile) == true)
            //{
            //    string tmp = Encoding.UTF8.GetString(Properties.Resources.Bloodweb);
            //    models.bloodweb.Rootobject bb = JsonConvert.DeserializeObject<models.bloodweb.Rootobject>(tmp);
            //    bb.prestigeLevel = (int)variables.manager.get_numericupdown(numericUpDown1);
            //    string data = JsonConvert.SerializeObject(bb);
            //    sess.utilDecodeResponse();
            //    sess.utilSetResponseBody(data);
            //}
            //if (sess.fullUrl.Contains("v1/dbd-character-data/get-all") == true && variables.manager.get_switch_button(switch_savefile) == true)
            //{
            //    string tmp = Encoding.UTF8.GetString(Properties.Resources.GetAll);
            //    string class_data = null;
            //    models.getall.Rootobject bb = JsonConvert.DeserializeObject<models.getall.Rootobject>(tmp);
            //    foreach (models.getall.List data in bb.list)
            //    {
            //        data.prestigeLevel = (int)variables.manager.get_numericupdown(numericUpDown1);
            //    }
            //    class_data = JsonConvert.SerializeObject(bb);
            //    sess.utilDecodeResponse();
            //    sess.utilSetResponseBody(class_data);
            //}
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
                modifiedResponseBody = JsonConvert.SerializeObject(
                    BloodwebViewModel.BloodwebModel.JSONBloodwebModel
                );
                session.SetResponseBodyString(modifiedResponseBody);
                session.HttpClient.Response.ContentLength = modifiedResponseBody.Length;
            }

            return (session);
        }

        private SessionEventArgs ManipulateGetAll(SessionEventArgs session)
        {
            string modifiedResponseBody = null;

            if (OptionsViewModel.OptionsModel.TemporaryUnlock == true)
            {
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
