using MySqlConnector;
using System;

namespace MySqlDBService
{
    public class DBService
    {
        static MySqlConnectionStringBuilder csb;
        MySqlConnection conn;
        MySqlCommand cmd;
        MySqlDataReader reader;


        public delegate void ActionThatWorksWithParams(params object[] args);


        public MySqlDataReader Reader
        {
            get
            {
                if (cmd == null)
                {
                    throw new Exception("Nothing to read, cmd not yet created.");
                }

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

        public static void SetDBConfig(string server, string userId, string db, uint port = 3306, string password = "", string charSet = "utf8")
        {
            if (csb == null)
            {
                csb = new MySqlConnectionStringBuilder
                {
                    Server = server,
                    Port = port,
                    UserID = userId,
                    Password = password,
                    Database = db,
                    CharacterSet = charSet
                };
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


        public ActionThatWorksWithParams this[string procedureName]
        {
            get
            {
                void CreateProcedureCall(params object[] args)
                {
                    string vars = "";

                    for (int i = 0; i < args.Length; i++)
                    {
                        string param = $"@{i}, ";
                        vars += param;
                    }

                    vars = vars.TrimEnd(',', ' ');

                    Query($"CALL {procedureName}({vars})");

                    for (int i = 0; i < args.Length; i++)
                    {
                        AddInputParameter($"@{i}", args[i]);
                    }
                }

                return CreateProcedureCall;
            }
        }

        ~DBService()
        {
            conn?.Close(); // ez meno (ha conn == null akk null-t ad vissza es nem kell if-ezni)
        }

    }
}
