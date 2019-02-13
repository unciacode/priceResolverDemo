using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceResolver.Helpers {
    public static class MongoIds {
        public static ObjectId ToObjectId(this string mongoIdString) {
            return ObjectId.Parse(mongoIdString); //we want the natural exceptions so we're not gonna do a safe converstion
        }
    }
}
