using PriceResolver.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceResolver {
    public interface IOrderable {

        string ID { set; get; }

        List<PriceBreak> PriceBreakList { set; get; }

        long QtyStock { set; get; }
        long QtyRequested { set; get; }   //this isn't really needed here? just enforcing comformity for future needs
        long QtyMinimum { set; get; }
        long QtyInterval { set; get; }

        double GetUnitPriceForQty(long? qty);

        long GetMaxOrderableQty(long? qty);
    }
}
