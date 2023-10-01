using System;

namespace Kataana.Models
{
    public class OptionsModel: BaseModel
    {
        private bool _temporaryUnlock;
        public bool TemporaryUnlock
        {
            get { return _temporaryUnlock; }
            set { SetProperty(ref _temporaryUnlock, value); }
        }

        private bool _autoRun;
        public bool AutoRun
        {
            get { return _autoRun; }
            set { SetProperty(ref _autoRun, value); }
        }

        private bool _customMarket;
        public bool CustomMarket
        {
            get { return _customMarket; }
            set { SetProperty(ref _customMarket, value); }
        }
    }
}
