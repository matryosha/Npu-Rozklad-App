using System;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Core.Servers;
using MySql.Data.MySqlClient;

namespace RozkladNpuBot.Persistence.Helpers
{
    public static class CheckDatabaseConnections
    {
        public static void CheckDbConnections(string mysqlConnectionString, 
            string mongoConnectionString)
        {
            CheckMySqlConnection(mysqlConnectionString);
            CheckMongoConnection(mongoConnectionString);
        }
        
        private static void CheckMySqlConnection(string connectionString)
        {
            var attempts = 3;
            if (!IsAbleToConnectToMysql(connectionString))
            {
                do
                {
                    Console.WriteLine("Connecting to mysql db...");
                    Task.Delay(TimeSpan.FromSeconds(5)).Wait();
                    if (!IsAbleToConnectToMysql(connectionString))
                        attempts--;
                    else
                        return;
                } while (attempts > 0);
            }
            if (attempts == 0)
                throw new Exception($"Cannot connect to mysql db. Connection string: {connectionString}");
        }
        
        private static void CheckMongoConnection(string connectionString)
        {
            var attempts = 3;
            if (!IsAbleToConnectToMongo(connectionString))
            {
                do
                {
                    Console.WriteLine("Connecting to mongo db...");
                    Task.Delay(TimeSpan.FromSeconds(5)).Wait();
                    if (!IsAbleToConnectToMongo(connectionString))
                        attempts--;
                    else
                        return;
                } while (attempts > 0);
            }

            if (attempts == 0)
                throw new Exception($"Cannot connect to mongo db. Connection string: {connectionString}");
        }

        private static bool IsAbleToConnectToMongo(string connectionString)
        {
            var mongoConnection = new MongoClient(connectionString);
            return mongoConnection.Cluster.Description.Servers.Single().State == ServerState.Connected;
        }

        private static bool IsAbleToConnectToMysql(string connectionString)
        {
            var mysqlConnection = new MySqlConnection(connectionString);
            try
            {
                mysqlConnection.Open();
            }
            catch (MySqlException e)
            {
                return false;
            }
            mysqlConnection.Close();
            return true;
        }
    }
}