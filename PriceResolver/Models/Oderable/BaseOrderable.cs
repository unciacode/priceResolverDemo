using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PriceResolver.Helpers;

namespace PriceResolver.Models.Oderable {
    public class BaseOrderable { 

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
        //We won't be using QtyRequested in this current iteration. 
        //All functions using QtyRequested are just forward-functionality and normally would be flagged for removal during planning if the next set of features doesn't need it

        public long QtyMinimum { get; set; } = 1L;
        public long QtyInterval { get; set; } = 1L;


        public BaseOrderable() { }  //always include constructor, even if unused


        public double GetUnitPriceForQty(long? qty = null) {
            long tmpQty = qty ?? _QtyOrderable;
            PriceBreak tmpBreak = GetNearestBreakForQty(tmpQty);

            return tmpBreak.unitPrice;
        }

        public double GetCurrentReqestedUnitPrice() => GetUnitPriceForQty(); 
        /* Above is what I call a pass-through: A function that only makes a single function call. They can be slightly useful visually as hints but,
           I normally try to avoid them as usually they tend to clutter, and the ROI is pretty low in practice */
        
        public long GetMaxOrderableQty(long? qty = null) {
            long tmpQty = qty ?? QtyRequested;
            long returnedQty = 0;

            if (tmpQty < QtyMinimum)
                return returnedQty;

            var cappedQty = tmpQty < QtyStock ? tmpQty : QtyStock;

            var tmpAdjustment = cappedQty % QtyInterval;
            returnedQty = cappedQty - tmpAdjustment;

            return returnedQty.ZeroFloored();
        }

        public PriceBreak GetNearestBreakForQty(long qty) {
            var tmpPB = PriceBreakList.Where(pb => pb.qty <= qty)
                                      .OrderByDescending(pb => pb.qty)
                                      .DefaultIfEmpty(new PriceBreak())
                                      .FirstOrDefault();
            return tmpPB;
        }

        public long GetMinimumAmountToFulfillInterval(long? qty) {
            long tmpQty = qty ?? QtyRequested;
            long returnedQty = QtyMinimum;

            if (tmpQty < QtyMinimum)
                return returnedQty;

            if (tmpQty % QtyInterval == 0)
                return 0;

            var intervalsNeeded = (int)Math.Ceiling(tmpQty / QtyInterval * 1D) + 1; //just a quirk of the funciton forcing whole numbers into a double, alwasy want the next interval up

            returnedQty = (intervalsNeeded * QtyInterval) - tmpQty;

            return returnedQty;
        }

    }
}
