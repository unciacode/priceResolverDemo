using PriceResolver.Models.Oderable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceResolver.Models {
    public class ResolverMatrix {
        public string ID { set; get; }

        public long MaxOrderable { set; get; }
        public long MinAdjustment { set; get; }

        public bool isMOQ1 { set; get; }

        public double UnitCost { set; get; }
        public double TotalCost { set; get; }

        public ResolverMatrix(BaseOrderable part, long qty) {
            ID = part.ID;

            MaxOrderable = part.GetMaxOrderableQty(qty);
            MinAdjustment = part.GetMinimumAmountToFulfillInterval(qty);

            UnitCost = part.GetUnitPriceForQty(MaxOrderable);
            TotalCost = UnitCost * MaxOrderable;

            isMOQ1 = part.QtyInterval == 1;
        }
    }
}
