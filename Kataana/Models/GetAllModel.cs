using Kataana.Models.JSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kataana.Models
{
    public class GetAllModel: BaseModel
    {
        private JSONGetAllModel _JSONgetAllModel;
        public JSONGetAllModel JSONGetAllModel
        {
            get { return _JSONgetAllModel; }
            set { SetProperty(ref _JSONgetAllModel, value); }
        }
    }
}
