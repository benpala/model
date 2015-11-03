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
    public class MySqlPeriodeService : IPeriodeService
    {
        private MySqlConnexion connexion;

        public IList<PeriodePaie> RetrieveAll()
        {
            IList<PeriodePaie> result = new List<PeriodePaie>();
            try
            {
                connexion = new MySqlConnexion();

                string requete = "SELECT dateDebut, dateFin FROM periodepaies";

                DataSet dataset = connexion.Query(requete);
                DataTable table = dataset.Tables[0];

                foreach (DataRow periode in table.Rows)
                {
                    PeriodePaie P = ConstructPeriodePaie(periode);
                    result.Add(P);
                }
            }
            catch (MySqlException)
            {
                throw;
            }

            return result;
        }
        private PeriodePaie ConstructPeriodePaie(DataRow row)
        {
            return new PeriodePaie(Convert.ToDateTime(row["dateDebut"]), Convert.ToDateTime(row["dateFin"])){};
        }
    }
}
