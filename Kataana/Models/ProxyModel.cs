using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;

namespace Kataana.Models
{
    public class ProxyModel: BaseModel
    {
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
