using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceResolver.Models {
    public class PriceBreak {

        public long qty { set; get; }
        public double unitPrice { set; get; }

        //Catching it at the source is the easiest way to prevent null references (even works for most serialization libs)
        public PriceBreak() {  
            qty = 0L;
            unitPrice = 0D;
        }
    }

}
