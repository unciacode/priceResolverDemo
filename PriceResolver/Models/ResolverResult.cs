using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceResolver.Models {
    class ResolverResult {
        public long QtyTotalOrdered { set; get; }
        public long QtyRemainder { set; get; } = 0;
        public bool isCompleteOrder => QtyRemainder > 0;

        public List<ResolverResultPair> OrderSet = new List<ResolverResultPair>();

        public ResolverResult() { }
        
        public class ResolverResultPair {
            public string ID { set; get; }
            public long QtyRequested { set; get; }

            public ResolverResultPair() {
                ID = String.Empty;  //String.Empty looks more intentional than "", the later could be/is a placeholder in a lot of people's workflows, or a copy-paste error
                QtyRequested = 0L;

            }
        }

        public class ResolverResultStepwiseFormula {
            //For a later time... it's pretty involved
        }

        public class ResolverResultPlotSet {
            //For a later time... it's less involved
        }

    }

}
