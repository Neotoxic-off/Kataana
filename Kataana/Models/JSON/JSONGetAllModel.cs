using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kataana.Models.JSON
{
    public class JSONGetAllModel
    {
        public string author { get; set; }
        public string updated { get; set; }
        public List[] list { get; set; }

        public class List
        {
            public string characterName { get; set; }
            public int legacyPrestigeLevel { get; set; }
            public Characteritem[] characterItems { get; set; }
            public int bloodWebLevel { get; set; }
            public Bloodwebdata bloodWebData { get; set; }
            public int prestigeLevel { get; set; }
        }

        public class Bloodwebdata
        {
            public string[] paths { get; set; }
            public Ringdata[] ringData { get; set; }
        }

        public class Ringdata
        {
            public Nodedata[] nodeData { get; set; }
        }

        public class Nodedata
        {
            public object contentId { get; set; }
            public string nodeId { get; set; }
            public string state { get; set; }
        }

        public class Characteritem
        {
            public string itemId { get; set; }
            public int quantity { get; set; }
        }
    }
}
