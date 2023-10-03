using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Titanium.Web.Proxy;
using System.Collections.ObjectModel;

namespace Kataana.Models
{
    public class ProxyModel: BaseModel
    {
        private ProxyServer _proxyServer;
        public ProxyServer ProxyServer
        {
            get { return _proxyServer; }
            set { SetProperty(ref _proxyServer, value); }
        }

        private bool _running;
        public bool Running
        {
            get { return _running; }
            set { SetProperty(ref _running, value); }
        }

        private string _label;
        public string Label
        {
            get { return _label; }
            set { SetProperty(ref _label, value); }
        }

        private X509Certificate2 _certificate;
        public X509Certificate2 Certificate
        {
            get { return _certificate; }
            set { SetProperty(ref _certificate, value); }
        }

        private string _certificatePath;
        public string CertificatePath
        {
            get { return _certificatePath; }
            set { SetProperty(ref _certificatePath, value); }
        }

        private string _certificatePassword;
        public string CertificatePassword
        {
            get { return _certificatePassword; }
            set { SetProperty(ref _certificatePassword, value); }
        }

        private ObservableCollection<string> _logs;
        public ObservableCollection<string> Logs
        {
            get { return _logs; }
            set { SetProperty(ref _logs, value); }
        }

        private string _bhvrSession;
        public string BHVRSession
        {
            get { return _bhvrSession; }
            set { SetProperty(ref _bhvrSession, value); }
        }
    }
}
