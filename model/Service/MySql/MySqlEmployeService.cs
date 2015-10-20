using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using model.Models;
using model.Service.Helpers;
using MySql.Data.MySqlClient;

namespace model.Service.MySql
{
    public class MySqlEmployeService : IEmployeService
    {
        private MySqlConnexion connexion;
        //Construction liste des employés
        public IList<Employe> RetrieveAll()
        {
            IList<Employe> lstEmploye = new List<Employe>();
            try
            {
                connexion = new MySqlConnexion();
                StringBuilder buildReq = new StringBuilder();
                buildReq.Append("SELECT Employes.idEmploye,detailfinancies.titreEmploi, detailfinancies.tauxHoraireNormal , detailfinancies.tauxHoraireOver, Employes.nom, Employes.prenom, Employes.horsFonction FROM Employes ");
                buildReq.Append(" INNER JOIN detailfinancies ON detailfinancies.idEmploye = Employes.idEmploye ");
                DataSet dataset = connexion.Query(buildReq.ToString());
                DataTable table = dataset.Tables[0];

                if (table.Rows.Count != 0)
                    foreach (DataRow employe in table.Rows)
                        lstEmploye.Add(ConstructEmploye(employe));
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
                Salaire = Convert.ToSingle(row["tauxHoraireNormal"]),
                SalaireOver = Convert.ToSingle(row["tauxHoraireOver"]),
                HorsFonction = (bool)row["horsFonction"]
            };
        }
        //Get liaison des projets pour un employé
        public IList<LiaisonProjetEmploye> GetLiaison(string EmpID)
        {
            IList<LiaisonProjetEmploye> result = new List<LiaisonProjetEmploye>();

            try
            {
                connexion = new MySqlConnexion();
                StringBuilder buildReq = new StringBuilder();
                buildReq.Append("SELECT liaisonprojetemployes.idEmploye, Projets.nom FROM Projets ");
                buildReq.Append(" LEFT JOIN liaisonprojetemployes  ON liaisonprojetemployes.idProjet = Projets.idProjet ");
                buildReq.Append(" LEFT JOIN Employes ON Employes.idEmploye = liaisonprojetemployes.idEmploye ");
                
                DataSet dataset = connexion.Query(buildReq.ToString());
                DataTable table = dataset.Tables[0];

                if (table.Rows.Count != 0)
                    foreach (DataRow liaison in table.Rows)
                    {
                        result.Add(ConstructLiaison(liaison, EmpID));
                    }
            }
            catch (MySqlException)
            {
                throw;
            }
            return result;
        }
        public LiaisonProjetEmploye ConstructLiaison(DataRow row, string EmpID)
        {
            if ((string)row[0].ToString() == EmpID)
            {
                return new LiaisonProjetEmploye()
                {
                    LiaisonID = (string)row[0].ToString(),
                    ProjNom = (string)row[1].ToString(),
                    Occupe = true

                };
            }
            else
                return new LiaisonProjetEmploye()
                {
                    LiaisonID = (string)row[0].ToString(),
                    ProjNom = (string)row[1].ToString(),
                    Occupe = false
                };
        }

       /* public List<bool> GetIDExist(string EmpID)
        {
            List<bool> ExisteID = new List<bool>();

            foreach (string ID in liaisonID)
                if (ID == EmpID)
                    ExisteID.Add(true);
                else 
                    ExisteID.Add(false);
            return ExisteID;
        }*/

        //Ajouter un employé
        public void AjoutUnEmploye(string Nom, string Prenom, string Poste, string Salaire)
        {
            try
            {
                connexion = new MySqlConnexion();
                StringBuilder buildReq = new StringBuilder();
                buildReq.Append("INSERT INTO Employes (nom,prenom) VALUES ('");
                buildReq.Append(Nom);
                buildReq.Append("' , '");
                buildReq.Append(Prenom);
                buildReq.Append("')");
                connexion.Query(buildReq.ToString());

                buildReq = new StringBuilder();
                buildReq.Append("SELECT idEmploye FROM Employes WHERE nom = '");
                buildReq.Append(Nom);
                buildReq.Append("' AND prenom = '");
                buildReq.Append(Prenom);
                buildReq.Append("'");
                DataSet IDdataSet = connexion.Query(buildReq.ToString());
                string ID = IDdataSet.Tables[0].Rows[0].ItemArray[0].ToString(); 

                buildReq = new StringBuilder();
                buildReq.Append("INSERT INTO detailfinancies (titreEmploi,tauxHoraireNormal,idEmploye) VALUES ('");
                buildReq.Append(Poste);
                buildReq.Append("' , '");
                buildReq.Append(Salaire);
                buildReq.Append("' , ");
                buildReq.Append(ID);
                buildReq.Append(")");
                connexion.Query(buildReq.ToString());
            }
            catch (MySqlException)
            {
                throw;
            }
        }
        //Verification d'ajout
        public bool ExisteEmploye(string Nom, string Prenom)
        {
            try
            {
                connexion = new MySqlConnexion();
                StringBuilder buildReq = new StringBuilder();
                buildReq.Append("SELECT idEmploye FROM Employes WHERE nom = '");
                buildReq.Append(Nom);
                buildReq.Append("' AND prenom = '");
                buildReq.Append(Prenom);
                buildReq.Append("'");
                DataSet dataset = connexion.Query(buildReq.ToString());
                DataTable table = dataset.Tables[0];
                if (table.Rows.Count == 0)
                {
                    return false;
                }
                else
                    return true;
            }
            catch (MySqlException)
            {
                throw;
            }
        }

        //Modifier Informations d'un employé
        public void UpdateInfoEmploye(Employe emp, bool hF)
         {
             try
             {
                // nom, prenom
                connexion = new MySqlConnexion();
                StringBuilder buildReq = new StringBuilder();
                buildReq.Append("SELECT Employes.idEmploye,detailfinancies.titreEmploi, detailfinancies.tauxHoraireNormal, detailfinancies.tauxHoraireOver, Employes.nom, Employes.prenom, Employes.horsFonction FROM Employes ");
                buildReq.Append(" INNER JOIN detailfinancies ON detailfinancies.idEmploye = ");             
                buildReq.Append(emp.ID);
                DataSet result = connexion.Query(buildReq.ToString());
                DataTable table = result.Tables[0];

                if (emp.Nom != table.Rows[0]["nom"].ToString() || emp.Prenom != table.Rows[0]["prenom"].ToString() || hF != (bool)table.Rows[0]["horsFonction"])
                {
                    buildReq = new StringBuilder();
                    buildReq.Append("UPDATE Employes SET nom = '");
                    buildReq.Append(emp.Nom);
                    buildReq.Append("', prenom = '");
                    buildReq.Append(emp.Prenom);
                    buildReq.Append("', horsFonction = ");
                    buildReq.Append(hF);
                    buildReq.Append(" WHERE idEmploye = ");
                    buildReq.Append(emp.ID);
                }
                connexion.Query(buildReq.ToString());

               // string s = table.Rows[0]["tauxHoraireNormal"].ToString().Replace(',', '.');
                //float salaire = Convert.ToSingle(s);
                string poste =  table.Rows[0]["titreEmploi"].ToString();
                //Poste + salaire
                if (emp.Poste != poste || emp.Salaire != Convert.ToSingle(table.Rows[0]["tauxHoraireNormal"]))
                {
                    buildReq = new StringBuilder();
                    buildReq.Append("UPDATE detailfinancies SET titreEmploi = '");
                    buildReq.Append(emp.Poste);
                    buildReq.Append("', tauxHoraireNormal = '");
                    buildReq.Append(emp.Salaire);
                    buildReq.Append("' WHERE idEmploye = '");
                    buildReq.Append(emp.ID);
                    buildReq.Append("'");
                }
                connexion.Query(buildReq.ToString());
            }
             catch (MySqlException)
             {
                 throw;
             }            
         }
        //Les projets associés à un employé
        public DataTable getProjetsEmploye(Employe emp)
        {
            try
            {
                connexion = new MySqlConnexion();
                StringBuilder buildReq = new StringBuilder();
                buildReq.Append("SELECT liaisonprojetemployes.idEmploye, Employes.prenom,Employes.nom, Projets.nom FROM Projets ");
                buildReq.Append(" LEFT JOIN liaisonprojetemployes  ON liaisonprojetemployes.idProjet = Projets.idProjet ");
                buildReq.Append(" LEFT JOIN Employes ON Employes.idEmploye = liaisonprojetemployes.idEmploye ");
              //  buildReq.Append(" WHERE Employes.idEmploye =");
             //   buildReq.Append(emp.ID);
                DataSet dataset = connexion.Query(buildReq.ToString());
                DataTable tableEM_on_PROJECT = dataset.Tables[0];

                return tableEM_on_PROJECT;
            }
            catch (MySqlException)
            {
                throw;
            }
        }

    }
}
