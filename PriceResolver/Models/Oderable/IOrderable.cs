using PriceResolver.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceResolver {
    public interface IOrderable { 
        //Mainly here just to demo that Interfaces are a thing, usually if I place them on flat objects it's because there will be a few variants that require enforced conformity.
        //Usually I use this pattern on objects in a data layer that manage DB connection logic, so they could be impersonated and bypass the integration requirements during unit testing 
        //  ex. swapping out a mongodb connection for a Mongo2Go temp data base, or flat files with known-good json.

        string ID { set; get; }

        List<PriceBreak> PriceBreakList { set; get; }

        long QtyStock { set; get; }
        long QtyMinimum { set; get; }
        long QtyInterval { set; get; }

        double GetUnitPriceForQty(long? qty);
        long GetMaxOrderableQty(long? qty);
        long GetMinimumAmountToFulfillInterval(long? qty);
    }
}
