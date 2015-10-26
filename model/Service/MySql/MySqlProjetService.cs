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
                           result.Add(ConstructProjet(projet));
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

        private Projet ConstructProjet(DataRow row)
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
                dateun = row["dateDebut"].ToString(),
                datedeux = (row["dateFin"].ToString() == "0001-01-01 00:00:00"?"Indéfini":row["dateFin"].ToString()),
                prixSimulation = prixSimule,
                prixReel = 0,
                nbHeuresSimule = CalculerNbHeuresSimule((Int32)row["idProjet"]),
                nbHeuresReel = /*CalculerNbHeuresReel((Int32)row["idProjet"])*/ 0,
                etat = (string)row["etat"]
            };
        }

        public string dernierId()
        {
            string dernier = "";


            try
            {
                connexion = new MySqlConnexion();

                string requete = "SELECT `AUTO_INCREMENT`" +
                                    "FROM  INFORMATION_SCHEMA.TABLES " +
                                    "WHERE TABLE_SCHEMA = '420-5a5-a15_gemc' " + 
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

                string requete = "INSERT INTO Projets (nom,dateDebut,dateFin,PrixSimulation,etat,nbHeuresSimule) VALUES('" + pNouveau.nom + "'," + pNouveau.dateun + "," + pNouveau.datedeux  + "," + pNouveau.prixSimulation  + ",'" + pNouveau.etat + "'," + pNouveau.nbHeuresSimule + " )";

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

        internal void modifier(Projet pNouveau)
        {
            try
            {
                connexion = new MySqlConnexion();

                string requete = "UPDATE Projets SET nom='" + pNouveau.nom + "',dateDebut=" + pNouveau.dateun + ",dateFin=" + pNouveau.datedeux + ",PrixSimulation=" + pNouveau.prixSimulation + ",etat='" + pNouveau.etat +"',nbHeuresSimule=" + pNouveau.nbHeuresSimule + " WHERE idProjet=" + pNouveau.ID;

                connexion.Query(requete);

            }
            catch (MySqlException)
            {
                throw;
            }
        }
    }

}
