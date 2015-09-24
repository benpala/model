using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model.Models
{
    public class Employe
    {
        public string ID { get; set; }
        public string Prenom { get; set; }
        public string Nom { get; set; }
        public string Poste { get; set; }
        public string Salaire { get; set; }
        public string Photo { get; set; }

        public Employe()
        {
            ID = String.Empty;
            Prenom = String.Empty;
            Nom = String.Empty;
            Poste = String.Empty;
            Salaire = String.Empty;
            Photo = String.Empty;
        }
        public Employe(string c_nom, string p_prenom):this()
        {
            Nom = c_nom;
            Prenom = p_prenom;
        }
    }
}
