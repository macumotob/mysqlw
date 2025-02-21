using MySql.Data.MySqlClient;
using static System.Net.Mime.MediaTypeNames;

namespace mysqlw
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var host = args[0];
            var user = args[1];
            var password = args[2];
            var db = args[3];
            var output = args[4];

            Console.WriteLine($"{host} {user} {password} {db}");
            Console.WriteLine($"{output}");

            if (!Directory.Exists(output))
            {
                Directory.CreateDirectory(output);
            }

            using (var conn = GetConnection(host, 3306, db, user, password))
            {
                var views = GetViews(conn,db);

                // Використання StreamWriter для запису у файл
                using (StreamWriter writer = new StreamWriter(output + "_views.sql"))
                {
                    foreach (var view in views)
                    {
                        writer.WriteLine($"CREATE VIEW {view.name} AS ");
                        writer.WriteLine($"{view.definition};");
                        writer.WriteLine();
                    }
                }
                conn.Close();
            }

            Console.WriteLine(".................. DONE ...............");
        }
        static MySqlConnection GetConnection(string host, int port, string database, string username, string password)
        {
            String connString = "Server=" + host + ";Database=" + database
                      + ";port=" + port + ";User Id=" + username + ";password=" + password
                       //+ ";TLS Version=TLS 1.3";
                       + ";SslMode=none;"
                       ;
            //var connString = $"server={host};uid={username};pwd={password};database={database}";

            //Console.WriteLine(connString);

            var conn = new MySqlConnection(connString);
            conn.Open();

            return conn;
        }
        public static List<view> GetViews(MySqlConnection conn,string server)
        {
            // Utils.IsStage = false;
            var list = new List<view>();


            string query = $"SELECT * FROM INFORMATION_SCHEMA.VIEWS where table_schema='{server}'";

            using (MySqlCommand command = new MySqlCommand(query, conn))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        string name = reader.GetString("TABLE_NAME");
                        string definition = reader.GetString("VIEW_DEFINITION");
                        list.Add(new view
                        {
                            name = name,
                            definition = definition
                        });
                    }
                }
            }
            return list;
        }
    }
}
