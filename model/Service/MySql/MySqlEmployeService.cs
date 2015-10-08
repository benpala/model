﻿using System;
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
            try
            {
                connexion = new MySqlConnexion();

                string requete = "SELECT Employes.idEmploye, detailFinancies.employeur, detailFinancies.titreEmploi, detailFinancies.tauxHoraireNormal, detailFinancies.tauxHoraireOver, nom, prenom, horsFonction FROM Employes INNER JOIN detailFinancies ON Employes.idEmploye = detailFinancies.idEmploye ";

                DataSet dataset = connexion.Query(requete);
                DataTable table = dataset.Tables[0];

                foreach (DataRow employe in table.Rows)
                {
                    result.Add(ConstructEmploye(employe));
                }
            }
            catch (MySqlException)
            {
                throw;
            }

            return result;
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
                horsFonction = (bool)row["horsFonction"]
            };
        }
    }

}
