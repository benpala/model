using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model.Models
{
    public class Paie
    {
        // ceci est une simple classe de test pour les vues. Elle sera détruitre lors de la programmation.
       
        public virtual string ID { get; set; }
        public virtual string DateGenerationRapport { get; set; }
        // Comme l'employé à une liste d'heure nous savons ici c'est quoi les heures pour l'employé.
        public Employe UnEmploye { get; set; }

        public virtual float MontantBrute { get; set; }
        public virtual float MontantNet { get; set; }
        public virtual int NombreHeure { get; set; }
        public virtual int NombreHeureSupp { get; set; }
        public virtual float MontantPrime { get; set; }
        public virtual float MontantIndemites { get; set; }
        public virtual float MontantAllocations { get; set; }
        public virtual float MontantPourboire { get; set; }
        
        public Paie()
        {
            ID = String.Empty;
            DateGenerationRapport = String.Empty;

            UnEmploye = new Employe();
            MontantBrute = 0;

            MontantNet = 0;
            NombreHeure = 0;

            NombreHeureSupp = 0;
            MontantPrime = 0;

            MontantIndemites = 0;
            MontantAllocations = 0;

            MontantPourboire = 0;
            
        }
        // Fin de la classe de test.
    }
}
