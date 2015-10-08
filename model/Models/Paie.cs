using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
        public static float supp = 50;

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
        public virtual float MontantIndemnite { get; set; }
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

            MontantIndemnite = 0;
            MontantAllocations = 0;
            MontantCommission = 0;
            MontantPourboire = 0;
            
        }
        // Fin de la classe de test.
        public void GenererPaies()
        {
            MySqlPaieService _service = new MySqlPaieService();
            DataTable Periode;
            
            float temps;
            try
            {
                // Aller chercher tous les employés 
                MySqlEmployeService _emService =  new MySqlEmployeService();
                IList<Employe> emp = _emService.RetrieveAll();
                Periode = _service.PeriodeTemps();
                float HeureSupp = 0; 
                
                // Pour chaque employé aller chercher leur temps.
                foreach (Employe em in emp)
                {
                    {
                        DateTime start = (DateTime)Periode.Rows[0][0], end =(DateTime)Periode.Rows[0][1];
                        temps = _service.RetrieveCompteurs(em.ID, start, end);
                        if (temps > supp)
                        {
                            HeureSupp = temps - supp;
                            temps = temps -supp;
                        }
                        //MyNumber = float.Parse("123.5", CultureInfo.InvariantCulture);
                        float Brute = ((float)em.Salaire*temps)+((float)em.SalaireOver*HeureSupp);
                        float Net = Brute * (1 - taux);

                        //Block de conversion ou on enleve les , pour des .
                      
                       
                        
                        Paie tmpPaie = new Paie(){MontantBrute = Brute, MontantNet=Net, NombreHeure=temps, NombreHeureSupp=HeureSupp};
                        // Appel  de la fonction de génération de paie.
                        if (!_service.insertPaie(tmpPaie, (DateTime)Periode.Rows[0][0], (DateTime)Periode.Rows[0][1], em.ID))
                        {
                            throw new Exception("Impossible de générer la paie de " + em.Prenom + " , " + em.Nom + " correctements vérifier les dates de vos périodes de paies.");
                        }
                    }
                }
                throw new Exception("Réussite de la génération des paies pour la période de :" + String.Format("{0:d/M/yyyy HH:mm:ss}",Periode.Rows[0][0]) + " aux " + String.Format("{0:d/M/yyyy HH:mm:ss}",Periode.Rows[0][1])); 
            }catch(Exception e)
            {
                throw e;
            }
        }
    }
}
