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

                foreach (DataRow projet in table.Rows)
                {
                    result.Add(ConstructProjet(projet));
                }
            }
            catch (MySqlException)
            {
                throw;
            }

            return result;
        }

        private Paie ConstructProjet(DataRow row)
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
                MontantIndemites = (float)row["montantIndemnites"],
                MontantAllocations = (float)row["montantAllocations"],
                MontantCommission = (float)row["montantCommissions"],
                MontantPourboire = (float)row["montantPourboire"]
            };
        } 
    }
}
