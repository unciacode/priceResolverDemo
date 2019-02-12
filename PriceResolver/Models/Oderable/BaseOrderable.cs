using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PriceResolver.Helpers;

namespace PriceResolver.Models.Oderable {
    public class BaseOrderable : IOrderable {

        private string _ID = null;
        public string ID {
            set => _ID = value;
            get {
                if (String.IsNullOrWhiteSpace(_ID))   //sometimes you just need a default!
                    _ID = Guid.NewGuid().ToString();

                return _ID;
            }
        }

        public List<PriceBreak> PriceBreakList { get; set; } = new List<PriceBreak>();

        public long QtyStock { get; set; } = 0L;
        public long QtyRequested { get; set; } = 0L;
        private long _QtyOrderable => GetMaxOrderableQty(QtyRequested);

        public long QtyMinimum { get; set; } = 1L;
        public long QtyInterval { get; set; } = 1L;


        public BaseOrderable() { }  //always include, even if unused

        public double GetUnitPriceForQty(long? qty = null) { 
            var tmpQty = qty ?? _QtyOrderable;
            PriceBreak tmpBreak = GetNearestBreakForQty(tmpQty);

            return tmpBreak.unitPrice;
        }

        public double GetCurrentReqestedUnitPrice() => GetUnitPriceForQty(); 
        /* Above is what I call a pass-through: A function that only makes a single function call. They can be minimally useful visually as hints but,
           I normally try to avoid them as usually they tend to clutter, and the ROI is pretty low in practice */
        
        public long GetMaxOrderableQty(long? qty = null) {
            long returnedQty = 0;

            if (qty < QtyMinimum)
                return returnedQty;

            var tmpQty = qty ?? QtyRequested;
            var cappedQty = tmpQty < QtyStock ? tmpQty : QtyStock;

            var tmpAdjustment = cappedQty % QtyInterval;
            if(tmpAdjustment != 0)
                returnedQty = cappedQty - tmpAdjustment;
            

            return returnedQty.ZeroFloored();
        }

        private PriceBreak GetNearestBreakForQty(long qty) {
            var tmpPB = PriceBreakList.Where(pb => pb.qty <= qty)
                                      .OrderByDescending(pb => pb.qty)
                                      .FirstOrDefault();
            return tmpPB;
        }

    }
}
