using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceResolver.Models.Oderable {
    public class OrderablePart : BaseOrderable {   //pretty much the display/storage class at this level

        public string Description { set; get; }
        public string Manufacturer { set; get; }
        public string PartNumber { set; get; }

        public OrderablePart() : base() {
            Description = String.Empty;
            Manufacturer = String.Empty;
            PartNumber = String.Empty;
        }

        public OrderablePart(long stock, List<PriceBreak> breaks) : base() {
            QtyStock = stock;
            PriceBreakList = breaks;

            Description = String.Empty;
            Manufacturer = String.Empty;
            PartNumber = String.Empty;
        }

        public OrderablePart(long stock, long minterval) : base() {
            QtyStock = stock;
            QtyMinimum = minterval;
            QtyInterval = minterval;

            Description = String.Empty;
            Manufacturer = String.Empty;
            PartNumber = String.Empty;
        }
    }
}
