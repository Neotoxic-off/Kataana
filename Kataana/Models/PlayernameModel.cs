using Kataana.Models.JSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kataana.Models
{
    public class PlayernameModel : BaseModel
    {
        private JSONPlayername _JSONplayername;
        public JSONPlayername JSONPlayername
        {
            get { return _JSONplayername; }
            set { SetProperty(ref _JSONplayername, value); }
        }
    }
}
