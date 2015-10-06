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
    public class MySqlEmployeService : IEmployeService
    {
        private MySqlConnexion connexion;
        public IList<Employe> RetrieveAll()
        {
            IList<Employe> lstEmploye = new List<Employe>();
            try
            {
                connexion = new MySqlConnexion();

                string requete = "SELECT e.idEmploye, d.titreEmploi, d.tauxHoraireNormal, d.tauxHoraireOver, e.nom, e.prenom, e.horsFonction FROM Employes e "
                            //    + " INNER JOIN Employes e ON e.idEmploye = l.idEmploye" 
                            //    + " INNER JOIN Projets p ON p.idProjet = l.idProjet" 
                                + " INNER JOIN detailfinancies d ON d.idEmploye = e.idEmploye ";

                DataSet dataset = connexion.Query(requete);
                DataTable table = dataset.Tables[0];

                foreach (DataRow employe in table.Rows)
                {
                    lstEmploye.Add(ConstructEmploye(employe));
                }
            }
            catch (MySqlException)
            {
                throw;
            }

            return lstEmploye;
        }

        private Employe ConstructEmploye(DataRow row)
        {
            return new Employe()
            {
                ID = row["idEmploye"].ToString(),
                Nom = row["nom"].ToString(),
                Prenom = row["prenom"].ToString(),
                Poste = row["titreEmploi"].ToString(),
                Salaire = (double)row["tauxHoraireNormal"],
                SalaireOver = (double)row["tauxHoraireOver"],
                HorsFonction = (bool)row["horsFonction"]
            };
        }
        
    }

}
