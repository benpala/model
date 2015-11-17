using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using model.Models;
using model.Service.Helpers;
using MySql.Data.MySqlClient;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

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
                buildReq.Append(" ORDER BY horsFonction, idEmploye ASC ");
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
                if(EmpID != null)
                { 
                    buildReq.Append("SELECT liaisonprojetemployes.idEmploye, Projets.nom ,liaisonprojetemployes.idProjet FROM Projets ");
                    buildReq.Append(" LEFT JOIN liaisonprojetemployes  ON liaisonprojetemployes.idProjet = Projets.idProjet ");
                    buildReq.Append(" LEFT JOIN Employes ON Employes.idEmploye = liaisonprojetemployes.idEmploye ");
                    buildReq.Append(" WHERE liaisonprojetemployes.idEmploye = '");
                    buildReq.Append(EmpID);
                    buildReq.Append("'");
                    DataSet dataset = connexion.Query(buildReq.ToString());
                    DataTable table = dataset.Tables[0];

                    if (table.Rows.Count != 0)
                        foreach (DataRow liaison in table.Rows)
                        {
                            result.Add(ConstructLiaison(liaison, EmpID));
                        }
                }
                else
                {
                    buildReq.Append("SELECT nom FROM Projets");
                    DataSet dataset = connexion.Query(buildReq.ToString());
                    DataTable table = dataset.Tables[0];
                    if (table.Rows.Count != 0)
                        foreach (DataRow liaison in table.Rows)
                        {
                            result.Add(ConstructLiaison(liaison, EmpID));
                        }
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
                    LiaisonID = null,
                    ProjNom = (string)row[0].ToString(),
                    Occupe = false
                };
        }

        //Ajouter un employé
        public void AjoutUnEmploye(string Nom, string Prenom, string Poste, string Salaire,bool horsFonction,string date, ObservableCollection<LiaisonProjetEmploye> Liaison)
        {
            try
            {
                connexion = new MySqlConnexion();
                StringBuilder buildReq = new StringBuilder();
                buildReq.Append("INSERT INTO Employes (nom,prenom,horsFonction) VALUES ('");
                buildReq.Append(Nom);
                buildReq.Append("' , '");
                buildReq.Append(Prenom);
                buildReq.Append("' , ");
                buildReq.Append(horsFonction);
                buildReq.Append(")");
                connexion.Query(buildReq.ToString());

                buildReq = new StringBuilder();
                buildReq.Append("SELECT idEmploye FROM Employes WHERE nom = '");
                buildReq.Append(Nom);
                buildReq.Append("' AND prenom = '");
                buildReq.Append(Prenom);
                buildReq.Append("'");
                DataSet IDdataSet = connexion.Query(buildReq.ToString());
                string ID = IDdataSet.Tables[0].Rows[0].ItemArray[0].ToString(); 
                /*
                buildReq = new StringBuilder();
                buildReq.Append("INSERT INTO detailfinancies (titreEmploi,tauxHoraireNormal,tauxHoraireOver,idEmploye,dateEmbauche) VALUES ('");
                buildReq.Append(Poste);
                buildReq.Append("' , '");
                Salaire = Salaire.Replace(',', '.');
                buildReq.Append(Salaire);
                buildReq.Append("' , '");
                Salaire = Salaire.Replace('.', ',');
                Double salaireOver = Convert.ToSingle(Salaire) * 3 / 2;
                salaireOver = Math.Round( salaireOver,2);
                string s = salaireOver.ToString().Replace(',', '.');
                buildReq.Append(s);
                buildReq.Append("' , ");
                buildReq.Append(ID);
                buildReq.Append(", '");
                buildReq.Append(Convert.ToDateTime(date));
                buildReq.Append("')");
                connexion.Query(buildReq.ToString());
                */
                UpdateProjetEmploye(Liaison, ID);
            }
            catch (MySqlException)
            {
                throw;
            }
        }
        //Verification d'ajout
        public bool ExisteEmploye(string Nom, string Prenom, string ID)
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
                {
                    if (ID == dataset.Tables[0].Rows[0][0].ToString())
                        return false;
                    else
                        return true;
                }
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
                    buildReq.Append("', tauxHoraireOver = '");
                    double s = emp.Salaire * 3 / 2;
                    string sOver = s.ToString().Replace(',','.');
                    buildReq.Append(sOver);
                    buildReq.Append("', tauxHoraireNormal = '");
                    string salaire = emp.Salaire.ToString().Replace(',','.');
                    buildReq.Append(salaire);
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
        public void UpdateProjetEmploye(ObservableCollection<LiaisonProjetEmploye> liaisonPE,string ID)
        {
        try
             {
                connexion = new MySqlConnexion();
                StringBuilder buildReq;
                //supprimer tout concerné à cet employé
                buildReq = new StringBuilder();
                buildReq.Append("DELETE FROM liaisonprojetemployes WHERE idEmploye = '");
                buildReq.Append(ID);
                buildReq.Append("'");
                connexion.Query(buildReq.ToString());
                foreach (LiaisonProjetEmploye l in liaisonPE)
                {
                    //Trouve ID de projet
                    buildReq = new StringBuilder();
                    buildReq.Append("SELECT idProjet FROM Projets WHERE nom = '");
                    buildReq.Append(l.ProjNom);
                    buildReq.Append("'");
                    DataSet dataset = connexion.Query(buildReq.ToString());
                    string idProjet = dataset.Tables[0].Rows[0][0].ToString();

                    //Insérer dans le BD si un projet est occupé
                    if (l.Occupe == true)
                    {
                        //Insérer de nouveau
                        buildReq = new StringBuilder();
                        buildReq.Append("INSERT INTO liaisonprojetemployes (idEmploye, idProjet) VALUES ('");
                        buildReq.Append(ID);
                        buildReq.Append("' , '");
                        buildReq.Append(idProjet);
                        buildReq.Append("') ");
                        connexion.Query(buildReq.ToString());
                    }
                    //supprimer si un projet n'est pas occupé
                    else
                    {
                        buildReq = new StringBuilder();
                        buildReq.Append("DELETE FROM liaisonprojetemployes WHERE idEmploye = '");
                        buildReq.Append(ID);
                        buildReq.Append("' AND idProjet = '");
                        buildReq.Append(idProjet);
                        buildReq.Append("' ");
                        connexion.Query(buildReq.ToString());
                    }
                }
             }
            catch (MySqlException)
            {
                throw;
            } 
        }
        public void SupprimerPhoto(string ID)
        {
            connexion = new MySqlConnexion();
            StringBuilder buildReq = new StringBuilder();
            buildReq.Append("SELECT idPhoto FROM Employes WHERE idEmploye = ");
            buildReq.Append(ID);
            DataSet dataset = connexion.Query(buildReq.ToString());
            string idPhoto = dataset.Tables[0].Rows[0][0].ToString();
            if (idPhoto !="")
            { 
                buildReq = new StringBuilder();
                buildReq.Append("UPDATE Employes SET idPhoto = null WHERE idEmploye = ");
                buildReq.Append(ID);
                connexion.Query(buildReq.ToString());

                buildReq = new StringBuilder();
                buildReq.Append("DELETE FROM Photos WHERE idPhoto = ");
                buildReq.Append(idPhoto);
                connexion.Query(buildReq.ToString());
            }
        }
        public void AjouterPhoto(Byte[] ImageData,string nom,string prenom,string format)
        {
            try
            {
                connexion = new MySqlConnexion();
                connexion.AjouterPhoto(ImageData, nom + prenom , format);

                StringBuilder buildReq = new StringBuilder();
                buildReq.Append("SELECT idEmploye FROM Employes WHERE nom = '");
                buildReq.Append(nom);
                buildReq.Append("' AND prenom = '");
                buildReq.Append(prenom);
                buildReq.Append("'");
                DataSet dataset = connexion.Query(buildReq.ToString());
                string IDEmploye = dataset.Tables[0].Rows[0][0].ToString();

                buildReq = new StringBuilder();
                buildReq.Append("SELECT idPhoto FROM Photos WHERE nom LIKE '%");
                buildReq.Append(nom+prenom);
                buildReq.Append("%'");
                dataset = connexion.Query(buildReq.ToString());
                string IDPhoto = dataset.Tables[0].Rows[0][0].ToString();

                buildReq = new StringBuilder();
                buildReq.Append("UPDATE Employes SET idPhoto = ");
                buildReq.Append(IDPhoto);
                buildReq.Append(" WHERE idEmploye = ");
                buildReq.Append(IDEmploye);
                connexion.Query(buildReq.ToString());
            }

            catch (MySqlException)
            {
                throw;
            }
        }
        public Byte[] GetPhoto(string ID)
        {
            
            try
            {
                if(ID.Length>0)
                { 
                    connexion = new MySqlConnexion();
                    StringBuilder buildReq = new StringBuilder();
                    buildReq.Append("SELECT idPhoto FROM Employes WHERE idEmploye = ");
                    buildReq.Append(ID);
                    DataSet dataset = connexion.Query(buildReq.ToString());
                    string idPhoto = dataset.Tables[0].Rows[0][0].ToString();
                    if (idPhoto !="")
                        return connexion.GetCodePhoto(idPhoto);
                    else 
                        return null;
                }
                return null;
            }
            catch (MySqlException)
            {
                throw;
            }
        }
        public static Byte[] GetBytesFromDataSet(DataSet ds)
        {
            Byte[] data = null;
            using (MemoryStream stream = new MemoryStream())
            {
                IFormatter bf = new BinaryFormatter();
                ds.RemotingFormat = SerializationFormat.Binary;
                bf.Serialize(stream, ds);
                data = stream.ToArray();
            }
            return data;
        }

    }
}
