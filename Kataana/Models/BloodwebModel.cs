using Kataana.Models.JSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kataana.Models
{
    public class BloodwebModel: BaseModel
    {
        private JSONBloodwebModel _JSONbloodwebModel;
        public JSONBloodwebModel JSONBloodwebModel
        {
            get { return _JSONbloodwebModel; }
            set { SetProperty(ref _JSONbloodwebModel, value); }
        }
    }
}
