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
        /*
         41 495 $ ou moins	16 %
         Supérieur à 41 495 $, ne dépassant pas 82 985 $	20 %
         Supérieur à 82 985 $, ne dépassant pas 100 970 $	24 %
         Supérieur à 100 970 $	25,75 %
        */
        public static float salaireUn = 41495, salaireDeux = 82985, salaireTrois = 100970;
        public static float tauxUn = (float)0.16,
                            tauxDeux = (float)0.20,
                            tauxTrois = (float)0.24,
                            tauxQuatre = (float)0.2575;

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
                MySqlEmployeService _emService = new MySqlEmployeService();
                IList<Employe> emp = _emService.RetrieveAll();
                Periode = _service.PeriodeTemps();
                
                if (Periode.Rows.Count == 0)
                {
                    throw new Exception("Toutes les périodes de paies ont déjà été générer. Allez en rentré de nouvelle pour la nouvelle année.");
                }
                else
                {
                    DateTime start = (DateTime)Periode.Rows[0][0],
                    end = (DateTime)Periode.Rows[0][1];
                    float HeureSupp = 0;
                    int compteurTemps = 0;
                    int totalEmp = emp.Count;
                // Pour chaque employé aller chercher leur temps.
                    foreach (Employe em in emp)
                    {
                        {
                       
                            temps = _service.RetrieveCompteurs(em.ID, start, end);
                            // Vérification dans le cas ou personne n'aurais travailler dans cette période.
                            if(temps == 0)
                            {
                                compteurTemps++;
                                if(compteurTemps == totalEmp)
                                {
                                    StringBuilder mess = new StringBuilder();
                                    mess.Append("Aucune employé à travail durant la période. Cette période à été noté comme généré: ");
                                    mess.Append(start.ToString());
                                    mess.Append(end.ToString());
                                    if (!(_service.periodeGenere(start, end)))
                                    {
                                        throw new Exception("Problème avec l'état des périodes de paie, vérifier que votre période courrante est cohérente.");
                                    }
                                    throw new Exception(mess.ToString());
                                }  
                            }
                            else 
                            { 
                                if (temps > supp)
                                {
                                    HeureSupp = temps - supp;
                                    temps = temps - supp;
                                }
                        
                                float Brute = ((float)em.Salaire * temps) + ((float)em.SalaireOver * HeureSupp);
                                TimeSpan t = end - start;
                                Double days = t.TotalDays;
                                days = 365/(days+1);
                                double EstimatedAnnualSalary = days * Brute;
                                float Net = 0;

                                if (EstimatedAnnualSalary <= salaireUn)
                                {
                                    Net = Brute * (1 - tauxUn);
                                }
                                else if (EstimatedAnnualSalary > salaireUn && EstimatedAnnualSalary<=salaireDeux)
                                {
                                    Net = Brute * (1 - tauxDeux);
                                }
                                else if(EstimatedAnnualSalary > salaireDeux && EstimatedAnnualSalary <= salaireTrois)
                                {
                                    Net = Brute * (1 - tauxTrois);
                                }
                                else if (EstimatedAnnualSalary > salaireTrois)
                                {
                                    Net = Brute * (1 - tauxQuatre);
                                }
                        

                                Paie tmpPaie = new Paie() { MontantBrute = Brute, MontantNet = Net, NombreHeure = temps, NombreHeureSupp = HeureSupp };
                                if (!(_service.periodeGenere(start, end)))
                                {
                                    throw new Exception("Problème avec l'état des périodes de paie, vérifier que votre période courrante est cohérente.");
                                }
                                // Appel  de la fonction de génération de paie.
                                if (!(_service.insertPaie(tmpPaie, start, end, em.ID)))
                                {
                                    throw new Exception("Impossible de générer la paie de " + em.Prenom + " , " + em.Nom + " correctements vérifier les dates de vos périodes de paies.");
                                }
                           }
                        }
                    }
                    // change la période paye à terminé.
                    throw new Exception("Réussite de la génération des paies pour la période de :" + start.ToString() + " aux " + end.ToString());
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
