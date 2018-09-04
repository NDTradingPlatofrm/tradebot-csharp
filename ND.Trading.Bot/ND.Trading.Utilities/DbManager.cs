using MongoDB.Driver;
using ND.Trading.Platform.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ND.Trading.Utilities
{
    public class DBManager
    {
        private static string connectionString = ConfigurationManager.AppSettings["MongoServer"].ToString();
        public static string DatabaseName { get { return ConfigurationManager.AppSettings["DBName"].ToString(); } }

        private static IMongoClient _client = null;
        private static IMongoDatabase _db = null;

        public IMongoClient Client
        {
            get
            {
                if (_client == null)
                {
                    _client = new MongoClient(connectionString);
                }

                return _client;
            }
        }

        public IMongoDatabase DB
        {
            get
            {
                if (_db == null)
                    _db = Client.GetDatabase(DatabaseName);
                return _db;
            }
        }

        public IMongoCollection<T> GetCollection<T>() where T : Entity
        {
            return DB.GetCollection<T>(typeof(T).Name);
        }

        public IEnumerable<T> GetEntityList<T>(FilterDefinition<T> query) where T : Entity
        {
            var collection = DB.GetCollection<T>(typeof(T).Name);
            return collection.Find<T>(query).ToList();
        }
        public IEnumerable<T> GetLastEntityList<T>(FilterDefinition<T> query, SortDefinition<T> sortDef, int limit) where T : Entity
        {
            var collection = DB.GetCollection<T>(typeof(T).Name);
            return collection.Find<T>(query).Sort(sortDef).Limit(limit).ToList();
        }

        public T GetEntity<T>(FilterDefinition<T> query) where T : Entity
        {
            var collection = DB.GetCollection<T>(typeof(T).Name);
            return collection.Find<T>(query).SingleOrDefault<T>();
        }

        public bool InsertEntity<T>(T entity) where T : Entity
        {
            GetCollection<T>().InsertOne(entity);
            return true;
        }
        public bool UpdateEntity<T>(UpdateDefinition<T> updateInfo, FilterDefinition<T> query) where T : Entity
        {
            GetCollection<T>().UpdateOne(query, updateInfo);
            return true;
        }

        public bool ReplaceEntity<T>(FilterDefinition<T> query, T entity) where T : Entity
        {
            GetCollection<T>().ReplaceOne(query, entity);
            return true;
        }
        public bool DeleteEntity<T>(FilterDefinition<T> query) where T : Entity
        {
            GetCollection<T>().DeleteOne(query);
            return true;
        }
    }
}
