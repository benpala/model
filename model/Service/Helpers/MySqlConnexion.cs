using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace model.Service.Helpers
{
    public class MySqlConnexion
    {

        private static readonly string CONNECTION_STRING;
        private MySqlConnection connection;
        private MySqlTransaction transaction;


        static MySqlConnexion()
        {
            CONNECTION_STRING = ConfigurationManager.ConnectionStrings["MySqlConnexion"].ConnectionString;
        }

        public MySqlConnexion()
        {
            try
            {
                connection = new MySqlConnection(CONNECTION_STRING);
            }
            catch (MySqlException)
            {
                throw;
            }
        }


        private bool Open()
        {
            try
            {
                if (connection.State == ConnectionState.Open)
                    return true;

                connection.Open();
                return true;
            }
            catch (MySqlException)
            {
                throw;
            }

        }

        public void OpenWithTransaction()
        {
            try
            {
                if (Open())
                {
                    transaction = connection.BeginTransaction();
                }
            }
            catch (MySqlException)
            {
                throw;
            }
        }

        private bool Close()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException)
            {
                throw;
            }
        }



        public void Commit()
        {
            try
            {
                transaction.Commit();
                transaction = null;
                connection.Close();
            }
            catch (MySqlException)
            {
                throw;
            }
        }

        public void Rollback()
        {
            try
            {
                transaction.Rollback();
                transaction = null;
                connection.Close();
            }
            catch (MySqlException)
            {
                throw;
            }
        }

        public int NonQuery(string query)
        {
            int nbResultat = 0;
            try
            {
                if (Open() || connection.State == ConnectionState.Open)
                {
                    MySqlCommand command = new MySqlCommand(query, connection);
                    nbResultat = command.ExecuteNonQuery();
                }

                return nbResultat;
            }
            catch (MySqlException)
            {
                throw;
            }
            finally
            {
                if (transaction == null)
                    Close();
            }
        }


        public DataSet Query(string query)
        {

            DataSet dataset = new DataSet();

            try
            {
                if (Open() || transaction != null)
                {
                    MySqlDataAdapter adapter = new MySqlDataAdapter();
                    adapter.SelectCommand = new MySqlCommand(query, connection);
                    adapter.Fill(dataset);


                }
                return dataset;

            }
            catch (MySqlException)
            {
                throw;
            }
            finally
            {
                Close();
            }

        }


        public DataSet StoredProcedure(string query, IList<MySqlParameter> parameters = null)
        {
            DataSet dataset = new DataSet();

            try
            {
                if (Open() || transaction != null)
                {

                    MySqlCommand commande = new MySqlCommand(query, connection);
                    commande.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                    {
                        commande.Parameters.AddRange(parameters.ToArray());
                    }
                    MySqlDataAdapter adapter = new MySqlDataAdapter();
                    adapter.SelectCommand = commande;
                    adapter.Fill(dataset);


                }
                return dataset;

            }
            catch (MySqlException)
            {
                throw;
            }
            finally
            {
                if (transaction == null)
                    Close();
            }
        }

        public void AjouterPhoto(Byte[] ImageData,string nom,string format)
        {
            MySqlCommand cmd;
            string SQLcmd = "INSERT INTO Photos (nom,typePhoto,codePhoto) VALUES (@nom ,@type , @image)";
            cmd = new MySqlCommand(SQLcmd, connection);
            cmd.Parameters.Add("@nom", nom);
            cmd.Parameters.Add("@type", format);
            cmd.Parameters.Add("@image", MySqlDbType.Blob).Value = ImageData;
            Open();
            cmd.ExecuteNonQuery();
        }
        public Byte[] GetCodePhoto(string ID)
        {
            Byte[] blob = null;
            string SQLcmd = "SELECT codePhoto FROM Photos WHERE idPhoto = " + ID;
            MySqlDataAdapter adapt = new MySqlDataAdapter(SQLcmd,connection);
            DataTable dt = new DataTable();
            adapt.Fill(dt);
            foreach(DataRow row in dt.Rows)
            {
                blob = (Byte[])row["codePhoto"];
            }
            return blob;
        }

        public string getBD()
        {
            CONNECTION_STRING.IndexOf("database=");
            return CONNECTION_STRING.Substring(CONNECTION_STRING.IndexOf("database=") + 9);
        }
    }
}
