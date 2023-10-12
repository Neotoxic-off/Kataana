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

        private bool _spoofPlayername;
        public bool SpoofPlayername
        {
            get { return _spoofPlayername; }
            set { SetProperty(ref _spoofPlayername, value); }
        }

        private bool _bypassAnalytics;
        public bool BypassAnalytics
        {
            get { return _bypassAnalytics; }
            set { SetProperty(ref _bypassAnalytics, value); }
        }

        private bool _bypassLogin;
        public bool BypassLogin
        {
            get { return _bypassLogin; }
            set { SetProperty(ref _bypassLogin, value); }
        }

        private bool _fakeLocation;
        public bool FakeLocation
        {
            get { return _fakeLocation; }
            set { SetProperty(ref _fakeLocation, value); }
        }

        private bool _forceClientValidation;
        public bool ForceClientValidation
        {
            get { return _forceClientValidation; }
            set { SetProperty(ref _forceClientValidation, value); }
        }

        private bool _forceIncentives;
        public bool ForceIncentives
        {
            get { return _forceIncentives; }
            set { SetProperty(ref _forceIncentives, value); }
        }
    }
}
