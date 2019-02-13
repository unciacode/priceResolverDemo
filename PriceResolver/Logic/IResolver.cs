using MongoDB.Bson;
using PriceResolver.Database;
using PriceResolver.Models;
using PriceResolver.Models.Oderable;
using System.Collections.Generic;
using System.Linq;

namespace PriceResolver.Logic {
    public interface IResolver {
        IOrderableProvider dataProvider { set; get; }

        long TotalRequestedQuantity { set; get; }
        List<BaseOrderable> PartsListLibrary { set; get; }
        ResolverResult Result { set; get; }


        void LoadFromDatabase(string mongoIdentifier);
        void LoadFromDatabase(ObjectId mongoIdentifier);

        void LoadPartsList(List<BaseOrderable> toLoad);
        void LoadPartsList(List<OrderablePart> toLoad);


        ResolverResult RunLogic();

        void ProcessMain();
        void ProcessLeftovers();

    }
}