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

                string requete = "SELECT * FROM Projets ORDER BY Etat";

                DataSet dataset = connexion.Query(requete);
                DataTable table = dataset.Tables[0];

                if(table.Rows.Count != 0)
                { 
                    foreach (DataRow projet in table.Rows)
                    {
                        string requete2 = "SELECT DISTINCT COUNT(e.idEmploye) FROM Employes e INNER JOIN liaisonProjetEmployes l ON l.idEmploye = e.idEmploye WHERE idProjet = " + projet.ItemArray[0];
                        DataSet dataset2 = connexion.Query(requete2);
                        DataTable table2 = dataset2.Tables[0];
                        DataRow row2 = table2.Rows[0];
                        result.Add(ConstructProjet(projet,row2));
                    }
                }
            }
            catch(MySqlException)
            {
                throw;
            }

            return result;
        }

        public int CalculerNbHeuresSimule(int ID)
        {
            int NbHeures = 0;

            try
            {
                connexion = new MySqlConnexion();

                string requete = "SELECT * FROM Projets WHERE idProjet = " + ID;

                DataSet dataset = connexion.Query(requete);
                DataTable table = dataset.Tables[0];

                if (table.Rows.Count != 0)
                {
                    foreach (DataRow projet in table.Rows)
                    {
                        string requeteNbHeure = "SELECT TIMESTAMPDIFF(DAY,dateDebut,dateFin) FROM projets WHERE idProjet = " + projet.ItemArray[0];
                        DataSet result = connexion.Query(requeteNbHeure);
                        DataTable tableHeure = result.Tables[0];

                        foreach (DataRow Heures in tableHeure.Rows)
                        {
                            NbHeures = int.Parse(Heures.ItemArray[0].ToString());
                            NbHeures = NbHeures * 7;
                            if(NbHeures < 0)
                            {
                                NbHeures = 0;
                            }
                        }
                    }
                }
            }
            catch(MySqlException)
            {
                throw;
            }

            return NbHeures;
        }

        public int CalculerNbHeuresReel(int ID)
        {
            int NbHeures = 0;

            try
            {
                connexion = new MySqlConnexion();

                string requete = "SELECT * FROM Projets WHERE idProjet = " + ID;

                DataSet dataset = connexion.Query(requete);
                DataTable table = dataset.Tables[0];

                if (table.Rows.Count != 0)
                {
                    foreach (DataRow projet in table.Rows)
                    {
                        string requeteNbHeure = "SELECT SUM(TIMESTAMPDIFF(minute,dateTimerStart,dateTimerEnd)/60) FROM compteursTemps WHERE idProjet = " + ID;
                        DataSet result = connexion.Query(requeteNbHeure);
                        DataTable tableHeure = result.Tables[0];

                        foreach (DataRow Heures in tableHeure.Rows)
                        {
                            if (Heures.ItemArray[0].ToString() != "")
                            {
                                NbHeures = Convert.ToInt32(Heures.ItemArray[0]);
                            }
                            if (NbHeures < 0)
                            {
                                NbHeures = 0;
                            }
                        }
                    }
                }
            }
            catch (MySqlException)
            {
                throw;
            }

            return NbHeures;
        }

        private Projet ConstructProjet(DataRow row,DataRow row2)
        {
            float prixSimule = 0;

            if (row.ItemArray[4].GetType().ToString() == "System.Single")
            {
                prixSimule = (float)row["prixSimulation"];
            }

            return new Projet()
            {
                ID = row["idProjet"].ToString(),
                nom = (string)row["nom"],
                dateun = (row["dateDebut"].ToString() == "0001-01-01 00:00:00"?"Indéfini":row["dateDebut"].ToString().Substring(0,10)),
                datedeux = (row["dateFin"].ToString() == "0001-01-01 00:00:00" ? "Indéfini" : row["dateFin"].ToString().Substring(0,10)),
                prixSimulation = prixSimule,
                prixReel = CalculerPrixReel(Convert.ToInt32(row["idProjet"])),
                nbHeuresSimule = /*CalculerNbHeuresSimule(*/Convert.ToInt32(row["nbHeuresSimule"])/*)*/,
                nbHeuresReel = CalculerNbHeuresReel(Convert.ToInt32(row["idProjet"])),
                dateTerminer = (row["dateTerminer"].ToString() != ""?row["dateTerminer"].ToString():""),
                dateAbandon = (row["dateAbandon"].ToString() != "" ? row["dateAbandon"].ToString(): ""),
                nbEmploye = int.Parse(row2.ItemArray[0].ToString()),
                joursOuvrable = (row["jourOuvrable"].ToString() == "0"?true:false),
                nbHeureTravail = int.Parse(row["nbHeureJour"].ToString()),
                nbQuart = int.Parse(row["nbQuart"].ToString()),
                nbRessourcesEstime = int.Parse(row["nbRessources"].ToString()),
                etat = ((string)row["etat"] != "END"?(string)row["etat"]: "TER")
            };
        }

        public float CalculerPrixReel(int ID)
        {
           IList<ProjetEmploye> iList = getEmployeProjet(ID.ToString());
           float Cout = 0;

           for (int i = 0; i < iList.Count(); i++)
           {
               Cout += Convert.ToSingle(Math.Floor(Convert.ToDouble(iList[i].Cout)));
           }

           return Cout;
        }

        public string dernierId()
        {
            string dernier = "";


            try
            {
                connexion = new MySqlConnexion();

                string requete = "SELECT `AUTO_INCREMENT`" +
                                    "FROM  INFORMATION_SCHEMA.TABLES " +
                                    "WHERE TABLE_SCHEMA = '" + connexion.getBD() + "' " + 
                                    "AND   TABLE_NAME   = 'Projets';";

                DataSet dataset = connexion.Query(requete);
                DataTable table = dataset.Tables[0];
                if (table.Rows.Count != 0)
                {
                    foreach (DataRow rowId in table.Rows)
                    {
                        dernier = rowId.ItemArray[0].ToString();
                    }
                }

            }
            catch (MySqlException)
            {
                throw;
            }

            return dernier;
        } 
        
        public void create(Projet pNouveau)
        {
            try
            {
                connexion = new MySqlConnexion();

                string requete = "INSERT INTO Projets (nom,dateDebut,dateFin,PrixSimulation,etat,nbHeuresSimule,nbRessources,jourOuvrable,nbHeureJour,nbQuart) VALUES('" + pNouveau.nom + "'," + pNouveau.dateun + "," + pNouveau.datedeux  + "," + pNouveau.prixSimulation  + ",'" + pNouveau.etat + "'," + pNouveau.nbHeuresSimule + "," + pNouveau.nbRessourcesEstime + "," + pNouveau.joursOuvrable + ","+ pNouveau.nbHeureTravail + "," + pNouveau.nbQuart + ")";

                connexion.Query(requete);

            }
            catch (MySqlException)
            {
                throw;
            }
        } 

        public bool Existe(string nomProjet)
        {
            try
            {
                connexion = new MySqlConnexion();

                string requete = "SELECT * FROM Projets WHERE nom = '" + nomProjet + "'";

                DataSet dataset = connexion.Query(requete);
                DataTable table = dataset.Tables[0];
                if (table.Rows.Count != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (MySqlException)
            {
                throw;
            }
        }

        public void modifier(Projet pNouveau, string date)
        {
            try
            {
                connexion = new MySqlConnexion();

                if(pNouveau.etat == "TER")
                    pNouveau.etat = "END";

                string requete = "UPDATE Projets SET nom='" + pNouveau.nom + "',dateDebut=" + pNouveau.dateun + ",dateFin="
                 + pNouveau.datedeux + ",PrixSimulation=" + pNouveau.prixSimulation + ",etat='"
                 + pNouveau.etat +"',nbHeuresSimule=" + pNouveau.nbHeuresSimule + (pNouveau.etat == "END"?",dateTerminer= '" + date + "'":pNouveau.etat == "ABD"?",dateAbandon= '" + date + "'":"") + ", nbRessources= " + pNouveau.nbRessourcesEstime + ",jourOuvrable= " + (pNouveau.joursOuvrable?"'0'":"'1'") + ", nbHeureJour=" + pNouveau.nbHeureTravail + ", nbQuart=" + pNouveau.nbQuart + " WHERE idProjet=" + pNouveau.ID;

                connexion.Query(requete);

            }
            catch (MySqlException)
            {
                throw;
            }
        }

        public IList<ProjetEmploye> getEmployeProjet(string idProj)
        {
            IList<ProjetEmploye> result = new List<ProjetEmploye>();
            try
            {
                connexion = new MySqlConnexion();

                string requete = "SELECT DISTINCT e.idEmploye, CONCAT(prenom,' ',nom), titreEmploi, tauxHoraireNormal FROM Employes e INNER JOIN liaisonProjetEmployes l ON l.idEmploye = e.idEmploye INNER JOIN detailFinancies d ON d.idEmploye = l.idEmploye WHERE idProjet = " + idProj;

                DataSet dataset = connexion.Query(requete);
                DataTable table = dataset.Tables[0];
                if (table.Rows.Count != 0)
                {
                    foreach (DataRow rowId in table.Rows)
                    {
                        string requete2 = "SELECT SUM(TIMESTAMPDIFF(minute,dateTimerStart,dateTimerEnd)/60) FROM compteursTemps WHERE idEmploye = " + rowId[0] +" AND idProjet = " + idProj;
                        DataSet dataset2 = connexion.Query(requete2);
                        DataTable table2 = dataset2.Tables[0];
                        DataRow rowId2 = table2.Rows[0];
                        float d = 0;
                        if(rowId2[0].ToString() != "")
                        {
                            d = Convert.ToSingle(rowId2[0]);
                        }

                        if (table2.Rows.Count != 0)
                        {
                           result.Add(new ProjetEmploye()
                            {
                                ID = rowId[0].ToString(),
                                Nom = rowId[1].ToString(),
                                Poste = rowId[2].ToString(),
                                Heure = Math.Round(d,2).ToString(),
                                Cout = Math.Round((Convert.ToSingle(rowId[3]) * d),2).ToString()
                            });
                        }
                    }
                }
            }
            catch (MySqlException)
            {
                throw;
            }

            return result;
        }
    }

}
