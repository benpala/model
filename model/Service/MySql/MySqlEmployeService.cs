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
            IList<Employe> result = new List<Employe>();
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
                    result.Add(ConstructEmploye(employe));
                    lstEmploye.Add(ConstructEmploye(employe));
                }
            }
            catch (MySqlException)
            {
                throw;
            }
            return result;
            return lstEmploye;
        }
        public DataTable getProjet_onEmploye(string id){
            /*
                SELECT liaisonprojetemployes.idEmploye, CONCAT(Employes.prenom," ",Employes.nom),  Projets.nom FROM Projets
                LEFT JOIN liaisonprojetemployes  ON liaisonprojetemployes.idProjet = Projets.idProjet 
                LEFT JOIN Employes ON Employes.idEmploye = liaisonprojetemployes.idEmploye WHERE Employes.idEmploye = 2
 
            */
            try
            {
                connexion = new MySqlConnexion();
                StringBuilder buildReq = new StringBuilder();
                buildReq.Append("SELECT liaisonprojetemployes.idEmploye, CONCAT(Employes.prenom,' ',Employes.nom), Projets.nom FROM Projets");
                buildReq.Append(" LEFT JOIN liaisonprojetemployes  ON liaisonprojetemployes.idProjet = Projets.idProjet");
                buildReq.Append(" LEFT JOIN Employes ON Employes.idEmploye = liaisonprojetemployes.idEmploye WHERE Employes.idEmploye =");
                buildReq.Append(id);
                DataSet dataset = connexion.Query(buildReq.ToString());
                DataTable tableEM_on_PROJECT = dataset.Tables[0];
                
                return tableEM_on_PROJECT;
            }
            catch (MySqlException)
            {
                throw;
            }

            
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
