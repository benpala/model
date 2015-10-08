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
    public class MySqlProjetService : IProjetService
    {
        private MySqlConnexion connexion;
        public IList<Projet> retrieveAll()
        {
            IList<Projet> result = new List<Projet>();
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
            catch(MySqlException)
            {
                throw;
            }

            return result;
        }

        private Projet ConstructProjet(DataRow row)
        {
            return new Projet()
            {
                ID = (string)row["idProjet"],
                nom = (string)row["nom"],
                dateun = (string)row["dateDebut"],
                datedeux = (string)row["dateFin"],
                prixSimulation = (float)row["PrixSimulation"],
                abandonne = (bool)row["estAbandonne"]
            };
        } 
    }

}
