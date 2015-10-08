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
<<<<<<< HEAD
            IList<Employe> result = new List<Employe>();
=======
            IList<Employe> lstEmploye = new List<Employe>();
>>>>>>> 50d8ed7ad07c7fb2cb06651ea58aca19448f1d63
            try
            {
                connexion = new MySqlConnexion();

<<<<<<< HEAD
                string requete = "SELECT Employes.idEmploye, detailFinancies.employeur, detailFinancies.titreEmploi, detailFinancies.tauxHoraireNormal, detailFinancies.tauxHoraireOver, nom, prenom, horsFonction FROM Employes INNER JOIN detailFinancies ON Employes.idEmploye = detailFinancies.idEmploye ";
=======
                string requete = "SELECT e.idEmploye, d.titreEmploi, d.tauxHoraireNormal, d.tauxHoraireOver, e.nom, e.prenom, e.horsFonction FROM Employes e "
                            //    + " INNER JOIN Employes e ON e.idEmploye = l.idEmploye" 
                            //    + " INNER JOIN Projets p ON p.idProjet = l.idProjet" 
                                + " INNER JOIN detailfinancies d ON d.idEmploye = e.idEmploye ";
>>>>>>> 50d8ed7ad07c7fb2cb06651ea58aca19448f1d63

                DataSet dataset = connexion.Query(requete);
                DataTable table = dataset.Tables[0];

                foreach (DataRow employe in table.Rows)
                {
<<<<<<< HEAD
                    result.Add(ConstructEmploye(employe));
=======
                    lstEmploye.Add(ConstructEmploye(employe));
>>>>>>> 50d8ed7ad07c7fb2cb06651ea58aca19448f1d63
                }
            }
            catch (MySqlException)
            {
                throw;
            }

<<<<<<< HEAD
            return result;
=======
            return lstEmploye;
>>>>>>> 50d8ed7ad07c7fb2cb06651ea58aca19448f1d63
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
<<<<<<< HEAD
                horsFonction = (bool)row["horsFonction"]
            };
        }
=======
                HorsFonction = (bool)row["horsFonction"]
            };
        }
        
>>>>>>> 50d8ed7ad07c7fb2cb06651ea58aca19448f1d63
    }

}
