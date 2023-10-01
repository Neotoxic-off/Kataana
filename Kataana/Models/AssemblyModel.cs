using System;

namespace Kataana.Models
{
    public class AssemblyModel: BaseModel
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private Version _version;
        public Version Version
        {
            get { return _version; }
            set { SetProperty(ref _version, value); }
        }
    }
}
