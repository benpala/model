using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model.Models.Args
{
    public class RetrieveProjetArgs
    {
        public string ID { get; set; }
        public string nom { get; set; }
        public string dateun { get; set; }
        public string datedeux { get; set; }
        public int nbHeures { get; set; }
    }
}
