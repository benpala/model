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
        public double Salaire { get; set; }
        public string Photo { get; set; }
        public bool HorsFonction { get; set; }
        public double SalaireOver { get; set; }

        public Employe()
        {
            ID = String.Empty;
            Prenom = String.Empty;
            Nom = String.Empty;
            Poste = String.Empty;
            Salaire = 0;
            SalaireOver = 0;
            Photo = String.Empty;
            HorsFonction = false;
        }
        public Employe(string nom)
        {
            ID = String.Empty;
            Prenom = String.Empty;
            Nom = nom;
            Poste = String.Empty;
            Salaire = 0;
            SalaireOver = 0;
            Photo = String.Empty;
            HorsFonction = false;
        }

    }
}
