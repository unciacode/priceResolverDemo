using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceResolver.Database {
    public class SingleTonMongoConnection {

        private static readonly Lazy<SingleTonMongoConnection> _instance = new Lazy<SingleTonMongoConnection>(() => new SingleTonMongoConnection());

        public static SingleTonMongoConnection Instance { get { return _instance.Value; } }

        private MongoClient _mongoClient;

        public IMongoDatabase OrderableDB => _mongoClient.GetDatabase("Orderables_DB");
        public IMongoDatabase MetricsDB => _mongoClient.GetDatabase("Metrics_DB");

        private SingleTonMongoConnection() {
            MongoClientSettings connectionSettings = new MongoClientSettings {
                IPv6 = false,
                Server = new MongoServerAddress("localhost", 27017)
            };
            _mongoClient = new MongoClient(connectionSettings);
        }

    }
}
