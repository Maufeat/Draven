using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Draven
{
    public static class DatabaseManager
    {
        public static MySqlConnection connection { get; set; }
        public static MySqlDataReader rdr = null;
        public static object Locker = new object();

        public static bool InitConnection()
        {
            try
            {
                Console.WriteLine("[LOG] Connecting to database");
                connection = new MySqlConnection("Database=" + Program.database + ";Data Source=" + Program.host + ";User Id = " + Program.user + "; Password = " + Program.pass + "; SslMode=none");
                connection.Open();
                Console.WriteLine("[LOG] Connection established");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("[LOG] Couldn't connect to database.\n" + e.Message);
                return false;
            }
        }

        public static Dictionary<string, string> getAccountData(string user, string pass)
        {
            MySqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM accounts WHERE username='" + user + "' AND password='" + pass + "'";
            MySqlDataReader reader = cmd.ExecuteReader();
            DataTable dtCustomers = new DataTable();
            dtCustomers.Load(reader);
            var dataArray = new Dictionary<string, string>();
            foreach (DataRow row in dtCustomers.Rows)
            {
                dataArray["id"] = row["id"].ToString();
                dataArray["summonerId"] = row["summonerId"].ToString();
                dataArray["RP"] = row["RP"].ToString();
                dataArray["IP"] = row["IP"].ToString();
                dataArray["banned"] = row["isBanned"].ToString();
            }
            return dataArray;
        }

        public static Dictionary<string, string> getSummonerData(string sumId)
        {
            MySqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM summoner WHERE id='" + sumId + "'";
            MySqlDataReader reader = cmd.ExecuteReader();
            DataTable dtCustomers = new DataTable();
            dtCustomers.Load(reader);
            var dataArray = new Dictionary<string, string>();
            foreach (DataRow row in dtCustomers.Rows)
            {
                dataArray["id"] = row["id"].ToString();
                dataArray["summonerName"] = row["summonerName"].ToString();
                dataArray["icon"] = row["icon"].ToString();
            }
            return dataArray;
        }

        public static void updateSummonerIconById(int sumId, int iconId)
        {
            try {
                MySqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = "UPDATE summoner SET icon='" + iconId + "' WHERE id='" + sumId + "'";
                cmd.ExecuteNonQuery();
            } catch (MySqlException sex)
            {
                Console.WriteLine(sex.Message);
            }
        }

        public static bool checkAccount(string user, string pass)
        {
            try
            {
                MySqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT count(*) FROM accounts WHERE username='" + user + "' AND password='" + pass + "'";
                int userCount = Convert.ToInt32(cmd.ExecuteScalar());
                if (userCount > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
    }
}
