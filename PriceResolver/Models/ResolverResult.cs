using PriceResolver.Helpers;
using System;
using System.Collections.Generic;

namespace PriceResolver.Models {
    class ResolverResult {
        public long QtyTotalOrdered { set; get; } = 0L;
        public long QtyRemainder { set; get; } = 0L;
        public bool isCompleteOrder => QtyRemainder > 0;

        public List<ResolverResultPair> OrderSet = new List<ResolverResultPair>();

        public ResolverResult() { }
        public ResolverResult(long initialQuantity) {
            QtyRemainder = initialQuantity;
        }
        
        public void AddCandidate(string id, long qty) {
            QtyRemainder -= qty;

            if(QtyRemainder <0) {
                //thow a warning or something, TBD
            }

            QtyRemainder = QtyRemainder.ZeroFloored();
            OrderSet.Add(new ResolverResultPair { ID = id, QtyRequested = qty });
        }

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
            //For a later time... it's less involved, but for ver2
        }

    }

}
