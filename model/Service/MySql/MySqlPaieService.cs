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
                + "CONCAT(DATE(periodepaies.dateDebut),' aux ', DATE(periodepaies.dateFin)) AS periodeP, dateGenerationRapport, montantDueBrute,"
                + "montantDueNet, nombreHeure, nombreHeureSupp, montantPrime, montantIndemnites,montantAllocations,"
                + "montantCommissions,montantPourboire,paies.idPeriode, paies.idEmploye,detailfinancies.tauxHoraireNormal, paies.updatedetail "
                + "FROM Paies "
                + "INNER JOIN Employes ON Employes.idEmploye=Paies.idEmploye "
                + "INNER JOIN detailfinancies ON Employes.idEmploye=detailfinancies.idEmploye "
                + "INNER JOIN periodepaies ON periodepaies.idPeriode=paies.idPeriode ORDER BY name ASC, dateGenerationRapport DESC";

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
        public bool periodeGenere(DateTime start, DateTime end)
        {
            try
            {
                connexion = new MySqlConnexion();
                connexion.Query("UPDATE periodepaies SET aEteGenere=1 WHERE dateDebut='" + start + "' AND dateFin='" + end +"'");
                return true;
            }
            catch (MySqlException)
            {
                return false;
            }
        }
        // Ici nous commençons la génération des différentes paie en fonction des temps.
        public float RetrieveCompteurs(String id, DateTime periodeDebut, DateTime periodeFin)
        {
            try
            {
                float S_temps = 0;
                connexion = new MySqlConnexion();
                string requete = "SELECT SUM(TIMESTAMPDIFF( MINUTE, dateTimerStart, dateTimerEnd)/60) as temps FROM compteurstemps WHERE idEmploye = '" + id + "'"
                                + " AND dateTimerStart >= '" + periodeDebut + "' AND "
                                + " dateTimerEnd <= '" + periodeFin + "'";

                DataSet dataset = connexion.Query(requete);
                //dataSet.Tables[tableIndex].Rows[rowIndex][colIndex]
                DataTable table = dataset.Tables[0];
             
                string y = table.Rows[0][0].ToString();
                if(y != "")
                {
                    S_temps = Convert.ToSingle(table.Rows[0][0].ToString());
                }
                // float temps = (float)S_temps;
                return S_temps;
            }
            catch (MySqlException)
            {
                throw;
            }
        }
        public bool updatePay(Paie updatePaie)
        {
            try
            {
                connexion = new MySqlConnexion();
                /*
                 *  montantDueBrute, montantDueNet, nombreHeure, 
                 *  nombreHeureSupp, montantPrime, montantIndemnites,
                 *  montantAllocations, montantCommissions, montantPourboire
                 */
                StringBuilder req = new StringBuilder();
                req.Append("UPDATE Paies SET ");
                req.Append(" montantDueBrute='"); req.Append(updatePaie.MontantBrute.ToString().Replace(",","."));
                req.Append("', montantDueNet='"); req.Append(updatePaie.MontantNet.ToString().Replace(",", "."));
                req.Append("', nombreHeure='"); req.Append(updatePaie.NombreHeure.ToString().Replace(",", "."));
                req.Append("', nombreHeureSupp='"); req.Append(updatePaie.NombreHeureSupp.ToString().Replace(",", "."));
                req.Append("', montantPrime='"); req.Append(updatePaie.MontantPrime.ToString().Replace(",", "."));
                req.Append("', montantIndemnites='"); req.Append(updatePaie.MontantIndemnite.ToString().Replace(",", "."));
                req.Append("', montantAllocations='"); req.Append(updatePaie.MontantAllocations.ToString().Replace(",", "."));
                req.Append("', montantCommissions='"); req.Append(updatePaie.MontantCommission.ToString().Replace(",", "."));
                req.Append("', montantPourboire='"); req.Append(updatePaie.MontantPourboire.ToString().Replace(",", "."));
                req.Append("', updatedetail=NOW() "); 
                req.Append(" WHERE idPaies='");
                req.Append(updatePaie.ID.ToString());
                req.Append("' ");

                connexion.Query(req.ToString());
                return true;
            }
            catch (MySqlException)
            {
                return false;
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
        public DataTable anciennePeriode(string id) 
        {
            try
            {
                connexion = new MySqlConnexion();

                string requete = "SELECT dateDebut,dateFin FROM periodepaies WHERE idPeriode="+id;

                DataSet dataset = connexion.Query(requete);
                DataTable table = dataset.Tables[0];
                return table;
            }
            catch (MySqlException)
            {
                throw;
            }
        }
        public string tauxHorraire(string id)
        {
            try
            {
                connexion = new MySqlConnexion();

                string requete = "SELECT tauxHoraireNormal FROM detailfinancies WHERE idEmploye=" + id;

                DataSet dataset = connexion.Query(requete);
                DataTable table = dataset.Tables[0];
                return table.Rows[0][0].ToString();
            }
            catch (MySqlException)
            {
                throw;
            }
        }
        public bool insertPaie(Paie insertPaie, DateTime start, DateTime end, string idEmploye)
        {
            try
            {
                connexion = new MySqlConnexion();
                /*{
                          System.Globalization.NumberFormatInfo nf = new System.Globalization.NumberFormatInfo()
                          {
                              NumberGroupSeparator = "."
                          };
                          Brute = float.Parse(Brute, nf);
                          Net = float.Parse(Net.ToString(), nf);
                          HeureSupp = float.Parse(HeureSupp.ToString(), nf);
                          temps = float.Parse(temps.ToString(), nf);
                 }*/
                StringBuilder sb = new StringBuilder();

                sb.Append("INSERT INTO Paies (idEmploye, idPeriode, montantDueBrute, montantDueNet, nombreHeure,nombreHeureSupp) VALUES(");
                sb.Append("(SELECT idEmploye FROM Employes WHERE idEmploye ='");
                sb.Append(idEmploye);
                sb.Append("'),(SELECT idPeriode FROM periodepaies WHERE dateDebut ='");
                sb.Append(start);
                sb.Append("' AND dateFin = '");
                sb.Append(end);
                sb.Append("'),");
                sb.Append(insertPaie.MontantBrute.ToString().Replace(",", "."));
                sb.Append(",");
                sb.Append(insertPaie.MontantNet.ToString().Replace(",", "."));
                sb.Append(",");
                sb.Append(insertPaie.NombreHeure.ToString().Replace(",", "."));
                sb.Append(",");
                sb.Append(insertPaie.NombreHeureSupp.ToString().Replace(",", "."));
                sb.Append(")");
                /* string requete = "INSERT INTO Paies (idEmploye, idPeriode, montantDueBrute, montantDueNet, nombreHeure,nombreHeureSupp) VALUES("
                                  +"(SELECT idEmploye FROM Employes WHERE idEmploye ='" + idEmploye + "'),"
                                  +"(SELECT idPeriode FROM periodepaies WHERE dateDebut = '"+start+"' AND dateFin = '"+end+"'),"

                                  +""+insertPaie.MontantBrute+","+insertPaie.MontantNet+","+insertPaie.NombreHeure+","+insertPaie.NombreHeureSupp+""
                                  +")";*/
                connexion.Query(sb.ToString());
                return true;
            }
            catch (MySqlException)
            {
                return false;
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
                MontantIndemnite = (float)row["montantIndemnites"],
                MontantAllocations = (float)row["montantAllocations"],
                MontantCommission = (float)row["montantCommissions"],
                MontantPourboire = (float)row["montantPourboire"],
                idPeriode = row["idPeriode"].ToString(),
                idEmploye = row["idEmploye"].ToString(),
                salaire = row["tauxHoraireNormal"].ToString(),
                updatedetail = row["updatedetail"].ToString()
            };
        }
    }
}
