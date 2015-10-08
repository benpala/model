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

        public IList<Projet> retrieveAll(List<string> args,List<string> donnees)
        {
            IList<Projet> result = new List<Projet>();
            try
            {
                connexion = new MySqlConnexion();

                string requete = "SELECT * FROM Projets WHERE ";
                
                for(int i = 0; i < args.Count; i++)
                {
                    if(args[i] != "nbHeures")
                    { 
                        requete += args[i];
                        requete += " LIKE '%";
                        requete += donnees[i];
                        requete += "%' AND ";
                    }
                }
                    requete = requete.Substring(0,requete.Count()-5);

                DataSet dataset = connexion.Query(requete);
                DataTable table = dataset.Tables[0];

                if (table.Rows.Count != 0)
                {
                    foreach (DataRow projet in table.Rows)
                    {
                        result.Add(ConstructProjet(projet));
                    }
                }
            }
            catch (MySqlException)
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
                        string requeteNbHeure = "SELECT TIMESTAMPDIFF(HOUR,dateDebut,dateFin) FROM projets WHERE idProjet = " + projet.ItemArray[0];
                        DataSet result = connexion.Query(requeteNbHeure);
                        DataTable tableHeure = result.Tables[0];

                        foreach (DataRow Heures in tableHeure.Rows)
                        {
                            NbHeures = int.Parse(Heures.ItemArray[0].ToString());
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
                datedeux = row["dateFin"].ToString(),
                prixSimulation = prixSimule,
                prixReel = 0,
                nbHeuresSimule = CalculerNbHeuresSimule((Int32)row["idProjet"]),
                nbHeuresReel = /*CalculerNbHeuresSimule((Int32)row["idProjet"])*/ 0,
                etat = (string)row["etat"]
            };
        }
        
        public void create(Projet pNouveau)
        {
            try
            {
                connexion = new MySqlConnexion();

                string requete = "INSERT INTO Projets VALUES(" + pNouveau.nom + "," + pNouveau.dateun + "," + pNouveau.datedeux  + "," + pNouveau.prixSimulation  + "," + pNouveau.etat + ")";

                connexion.Query(requete);
            }
            catch (MySqlException)
            {
                throw;
            }
        } 
    }

}
