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

        private bool _unlockMarket;
        public bool UnlockMarket
        {
            get { return _unlockMarket; }
            set { SetProperty(ref _unlockMarket, value); }
        }
    }
}
