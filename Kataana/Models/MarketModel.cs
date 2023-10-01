using Kataana.Models.JSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kataana.Models
{
    public class MarketModel: BaseModel
    {
        private JSONMarketModel _JSONmarketModel;
        public JSONMarketModel JSONMarketModel
        {
            get { return _JSONmarketModel; }
            set { SetProperty(ref _JSONmarketModel, value); }
        }
    }
}
