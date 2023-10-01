using System;

namespace Kataana.Models
{
    public class AccountModel: BaseModel
    {
        private JSON.JSONGetAllModel _JSONgetAllModel;
        public JSON.JSONGetAllModel JSONGetAllModel
        {
            get { return _JSONgetAllModel; }
            set { SetProperty(ref _JSONgetAllModel, value); }
        }
    }
}
