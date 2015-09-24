using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model.Models.Args
{
    public class RetrieveEmployeArgs
    {
        public string ID { get; set; }
        public string Prenom { get; set; }
        public string Nom { get; set; }
        public string Poste { get; set; }
        public string Salaire { get; set; }
        public string Photo { get; set; }
    }
}
