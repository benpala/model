using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using model.Models;
using model.Service.Helpers;
using MySql.Data.MySqlClient;

namespace model.Service.MySql
{
    public class MysqlProjetEmploye : IProjetService
    {
        private MySqlConnexion connexion;
        public IList<Projet> retrieveProjEmp()
        {
            IList<Projet> result = new List<Projet>()
            try
            {
                connexion = new MySqlConnexion();

                string requete = "SELECT * FROM Projets";

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

        private Projet ConstructProjet(DataRow row)
        {
            float prixSimule = 0;

            if (row.ItemArray[4].Equals(null))
            {
                prixSimule = (float)row["prixSimulation"];
            }
            return new Projet()
            {
                ID = row["idProjet"].ToString(),
                nom = (string)row["nom"],
                dateun = row["dateDebut"].ToString(),
                datedeux = row["dateFin"].ToString(),
                prixSimulation = prixSimule,
                abandonne = (bool)row["estAbandonne"]
            };
        }
    }

}
