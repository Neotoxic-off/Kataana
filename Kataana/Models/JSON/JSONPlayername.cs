using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kataana.Models.JSON
{
    public class JSONPlayername : BaseModel
    {
        public Providerplayernames providerPlayerNames { get; set; }
        public string userId { get; set; }
        public string playerName { get; set; }
        public bool unchanged { get; set; }

        public class Providerplayernames
        {
            public string egs { get; set; }
        }
    }
}
