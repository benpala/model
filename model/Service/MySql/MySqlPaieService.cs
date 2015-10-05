using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using model.Service.Helpers;
using model.Models;
using System.Data;
using MySql.Data.MySqlClient;
namespace model.Service.MySql
{
    class MySqlPaieService : IPaiesService
    {
        private MySqlConnexion connexion;

        public IList<Paie> RetrieveAll()
        {
            IList<Paie> result = new List<Paie>();
            try
            {
                connexion = new MySqlConnexion();

                string requete = "SELECT paies.idPaies, CONCAT(Employes.prenom,' ',Employes.nom) AS name,"
                + "CONCAT(periodepaies.dateDebut,' aux ', periodepaies.dateFin) AS periodeP, dateGenerationRapport, montantDueBrute," 
                + "montantDueNet, nombreHeure, nombreHeureSupp, montantPrime, montantIndemnites,montantAllocations,"
                + "montantCommissions,montantPourboire  "
                + "FROM Paies "
                + "INNER JOIN Employes ON Employes.idEmploye=Paies.idEmploye "
                + "INNER JOIN periodepaies ON periodepaies.idPeriode=paies.idPeriode";

                DataSet dataset = connexion.Query(requete);
                DataTable table = dataset.Tables[0];

                foreach (DataRow paie in table.Rows)
                {
                    result.Add(ConstructPaie(paie));
                }
            }
            catch (MySqlException)
            {
                throw;
            }

            return result;
        }
        // Ici nous commençons la génération des différentes paie en fonction des temps.
        public float RetrieveCompteurs(String id, String periodeDebut, String periodeFin)
        {
            try
            {
                connexion = new MySqlConnexion();
                string requete = "SELECT SUM(TIMESTAMPDIFF( MINUTE, dateTimerStart, dateTimerEnd)/60) as temps FROM compteurstemps WHERE idEmploye = '" + id + "'" 
                                +" AND dateTimerStart >= '"+periodeDebut+"' AND "
                                + " dateTimerEnd <= '" + periodeFin + "'";

                DataSet dataset = connexion.Query(requete);
                //dataSet.Tables[tableIndex].Rows[rowIndex][colIndex]
                float table = (float)dataset.Tables[0].Rows[0][0];
                return table;
            }
            catch (MySqlException)
            {
                throw;
            }
        }
        public DataTable PeriodeTemps()
        {
            try
            {
                connexion = new MySqlConnexion();

                string requete = "SELECT dateDebut,dateFin FROM periodepaies WHERE aEteGenere IS FALSE ORDER BY dateFin LIMIT 1";

                DataSet dataset = connexion.Query(requete);
                DataTable table = dataset.Tables[0];
                return table;
            }
            catch (MySqlException)
            {
                throw;
            }
        }
        // fin de la génération.
        private Paie ConstructPaie(DataRow row)
        {
            return new Paie()
            {
                //ID = (string)row["idProjet"],
                ID = row["idPaies"].ToString(),
                Periode = row["periodeP"].ToString(), 
                DateGenerationRapport = row["DateGenerationRapport"].ToString(),
                Nom = row["name"].ToString(),
                MontantBrute = (float)row["montantDueBrute"],
                MontantNet = (float)row["montantDueNet"],
                NombreHeure = (float)row["nombreHeure"],
                NombreHeureSupp = (float)row["nombreHeureSupp"],
                MontantPrime = (float)row["montantPrime"],
                MontantRetenue = (float)row["montantIndemnites"],
                MontantAllocations = (float)row["montantAllocations"],
                MontantCommission = (float)row["montantCommissions"],
                MontantPourboire = (float)row["montantPourboire"]
            };
        } 
    }
}
