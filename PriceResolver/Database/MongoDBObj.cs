using MongoDB.Bson;
using MongoDB.Driver;
using PriceResolver.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceResolver.Database {
    class MongoDBObj {

        public class OrderableDB {
            public IMongoCollection<BsonDocument> Orderables => SingleTonMongoConnection.Instance.OrderableDB.GetCollection<BsonDocument>("Orderables");
            public IMongoCollection<BsonDocument> UserInfo => SingleTonMongoConnection.Instance.OrderableDB.GetCollection<BsonDocument>("UserInfo");
            public IMongoCollection<BsonDocument> Results => SingleTonMongoConnection.Instance.OrderableDB.GetCollection<BsonDocument>("Results");
        }

        public class MetricsDB {
            public IMongoCollection<BsonDocument> Basics => SingleTonMongoConnection.Instance.MetricsDB.GetCollection<BsonDocument>("Basics");

        }
    }
}
