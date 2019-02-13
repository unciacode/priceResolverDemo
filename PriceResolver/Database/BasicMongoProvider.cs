using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using PriceResolver.Models.Oderable;

namespace PriceResolver.Database {
    class BasicMongoProvider : IOrderableProvider {

        public ManualResetEvent isReady { get; }
        public string SessionId { get; set; }

        public BasicMongoProvider() {
            isReady = new ManualResetEvent(true);
        }
        public BasicMongoProvider(string sessionId) {
            isReady = new ManualResetEvent(true);
            SessionId = sessionId;
        }

        public bool AddOrderable(BaseOrderable toAdd) {
            
            throw new NotImplementedException();
        }

        public bool AddOrderable(IEnumerable<BaseOrderable> toAdd) {
            throw new NotImplementedException();
        }

        public IEnumerable<BaseOrderable> GetOderableList(string id) {
            throw new NotImplementedException();
        }

        public bool RemoveOrderableData(string id) {
            throw new NotImplementedException();
        }

        public void SetSessionId(string id)=> SessionId = id;
    }
}
