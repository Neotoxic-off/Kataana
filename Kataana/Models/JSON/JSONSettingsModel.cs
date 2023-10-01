using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kataana.Models.JSON
{
    public class JSONSettingsModel: BaseModel
    {
        private string _marketFlux;
        public string MarketFlux
        {
            get { return _marketFlux; }
            set { SetProperty(ref _marketFlux, value); }
        }

        private string _versionFlux;
        public string VersionFlux
        {
            get { return _versionFlux; }
            set { SetProperty(ref _versionFlux, value); }
        }

        private string _marketFile;
        public string MarketFile
        {
            get { return _marketFile; }
            set { SetProperty(ref _marketFile, value); }
        }

        private string _bloodwebFile;
        public string BloodwebFile
        {
            get { return _bloodwebFile; }
            set { SetProperty(ref _bloodwebFile, value); }
        }

        private string _bloodwebFlux;
        public string BloodwebFlux
        {
            get { return _bloodwebFlux; }
            set { SetProperty(ref _bloodwebFlux, value); }
        }

        private string _getallFile;
        public string GetAllFile
        {
            get { return _getallFile; }
            set { SetProperty(ref _getallFile, value); }
        }

        private string _getallFlux;
        public string GetAllFlux
        {
            get { return _getallFlux; }
            set { SetProperty(ref _getallFlux, value); }
        }
    }
}
