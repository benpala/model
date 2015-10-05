using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using model.Service.MySql;

namespace model.Models
{
    public class Paie
    {
        // ceci est une simple classe de test pour les vues. Elle sera détruitre lors de la programmation.
        public static float taux = (float)0.20;

        public virtual string ID { get; set; }
        public virtual string Periode { get; set; }
        public virtual string DateGenerationRapport { get; set; }
        // Comme l'employé à une liste d'heure nous savons ici c'est quoi les heures pour l'employé.
        public virtual string Nom { get; set; }

        public virtual float MontantBrute { get; set; }
        public virtual float MontantNet { get; set; }
        public virtual float NombreHeure { get; set; }
        public virtual float NombreHeureSupp { get; set; }
        public virtual float MontantPrime { get; set; }
        public virtual float MontantRetenue { get; set; }
        public virtual float MontantAllocations { get; set; }
        public virtual float MontantCommission { get; set; }
        public virtual float MontantPourboire { get; set; }
        
        public Paie()
        {
            ID = String.Empty;
            Periode = String.Empty;
            DateGenerationRapport = String.Empty;

            Nom = String.Empty;
            MontantBrute = 0;

            MontantNet = 0;
            NombreHeure = 0;

            NombreHeureSupp = 0;
            MontantPrime = 0;

            MontantRetenue = 0;
            MontantAllocations = 0;
            MontantCommission = 0;
            MontantPourboire = 0;
            
        }
        // Fin de la classe de test.
        public void GenererPaies()
        {
            MySqlPaieService _service = new MySqlPaieService();
            DataTable idEm, Periode;
            
            float temps;
            /*foreach (DataRow paie in table.Rows)
               {
                   result.Add(ConstructPaie(paie));
               }*/
            try
            {
                // Aller chercher tous les employés
                MySqlEmployeService _emService =  new MySqlEmployeService();
                IList<Employe> emp = _emService.RetrieveAll();
                Periode = _service.PeriodeTemps();
                // Pour chaque employé aller chercher leur temps.
                foreach (Employe em in emp)
                {
                    {
                        temps = _service.RetrieveCompteurs(em.ID, (string)Periode.Rows[0][0], (string)Periode.Rows[0][1]);
                        float salaire = (float)em.Salaire;
                        MontantBrute = (salaire*temps);
                        MontantNet = MontantBrute*(1-taux);
                        // Todo: suite.
                    }
                   
                } 
            }catch(Exception e)
            {
                throw e;
            }
        }
    }
}
