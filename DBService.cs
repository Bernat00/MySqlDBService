using System;
using MySql.Data.MySqlClient;

namespace MySqlDBService
{
    public class DBService
    {
        static MySqlConnectionStringBuilder csb;
        MySqlConnection conn;
        MySqlCommand cmd;
        MySqlDataReader reader;

        public MySqlDataReader Reader
        {
            get
            {
                if (reader == null || reader.IsClosed)
                {
                    reader = cmd.ExecuteReader();
                }

                return reader;
            }
        }


        public DBService()
        {
            CreateConnection();
        }

        public DBService(string query)
        {
            CreateConnection();
            Query(query);
        }

        void CreateConnection()
        {
            if (csb == null)
            {
                throw new Exception("database connection not configured");
            }

            conn = new MySqlConnection(csb.ToString());
            conn.Open();
        }

        public static void SetDBConfig(string server, string userId, string db, string password = "", string charSet = "utf8")
        {
            if (csb == null)
            {
                csb = new MySqlConnectionStringBuilder();
                csb.Server = server;
                csb.UserID = userId;
                csb.Password = password;
                csb.Database = db;
                csb.CharacterSet = charSet;
            }
        }

        public void Query(string query)
        {
            reader?.Close();
            cmd = new MySqlCommand(query, conn);
        }


        public void AddInputParameter(string name, object value)
        {
            if (cmd == null)
            {
                throw new Exception("No procedure created yet.");
            }

            cmd.Parameters.AddWithValue(name, value);
            cmd.Parameters[name].Direction = System.Data.ParameterDirection.Input;
        }

        ~DBService()
        {
            conn?.Close(); // ez meno (ha conn == null akk null-t ad vissza es nem kell if-ezni)
        }
    }
}
