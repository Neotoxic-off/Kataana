using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kataana.Models.JSON
{
    public class JSONMarketModel
    {
        public int code { get; set; }
        public string author { get; set; }
        public string updated { get; set; }
        public Data data { get; set; }

        public class Data
        {
            public Inventory[] inventory { get; set; }
        }

        public class Inventory
        {
            public int lastUpdateAt { get; set; }
            public string objectId { get; set; }
            public int quantity { get; set; }
        }
    }
}
