using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceResolver.Models {

    /* 
     * I like to avoid having hundreds of small 10-line (stub) files. Bundling smaller, close-knit objects into a single file drasitcally 
     * reduces the need to change visual context and increases readability. 
     */
    public class BasicPriceBreak {
        public long qty { set; get; }
        public double unitPrice { set; get; }

        //Catching it at the source is the easiest way to prevent null references (even works for most serialization libs)
        public BasicPriceBreak() {  
            qty = 0L;
            unitPrice = 0D;
        }
    }

    public class PriceBreak : BasicPriceBreak {
        public double totalPrice => qty * unitPrice;
    }
}
