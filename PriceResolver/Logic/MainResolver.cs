using PriceResolver.Models;
using PriceResolver.Models.Oderable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceResolver.Logic {
    class MainResolver {

        public long TotalRequestedQuantity { set; get; } = 0L;
        private long _RemainderQuantityBucket = 0L;

        private List<IOrderable> _OriginalParts;
        private List<IOrderable> _WorkingParts;

        public MainResolver(long qty) {
            TotalRequestedQuantity = qty;
            _RemainderQuantityBucket = qty;
        }

        //not the most elegant, but it gets the job done fast and well;
        public void LoadParts(List<BaseOrderable> toLoad) {
            if (toLoad.Any())
                _OriginalParts = toLoad.AsParallel().Select(tl => tl as IOrderable).ToList();
            
        }
        public void LoadParts(List<OrderablePart> toLoad) {
            if (toLoad.Any())
                _OriginalParts = toLoad.AsParallel().Select(tl => tl as IOrderable).ToList();
            
        }

        public ResolverResult RunLogic() {
            ResolverResult returned = new ResolverResult();
            _WorkingParts = new List<IOrderable>(_OriginalParts); //If we don't init a new list, these can be linked and will really not work as intended

            do {
                /* We're gonna select the most quantity at the cheapest rate we can get.
                 * Filtered by what we can order from
                 * This loop will end either when there isn't any more viable candidate parts, or we get a full order
                 */ 
                var candidatePart = _WorkingParts.Select(p => new ResolverMatrix(p, _RemainderQuantityBucket))
                                                    .Where(p => p.MaxOrderable > 0)
                                                    .OrderByDescending(p => p.UnitCost)
                                                    .FirstOrDefault();

                if (candidatePart == null)
                    break;

                _RemainderQuantityBucket -= candidatePart.MaxOrderable;
                returned.AddCandidate(candidatePart.ID, candidatePart.MaxOrderable);

                //We're going to assume that all parts are keyed on their IDs (if they don't then we'd simply move this over to whatever key was unique, compound or not)
                _WorkingParts.RemoveAll(i => i.ID == candidatePart.ID);

            } while (_RemainderQuantityBucket > 0);


            if(_RemainderQuantityBucket > 0 && _WorkingParts.Any()) {
                /*
                 * Theres a few logic reasons we're here:
                 * - We exhausted all stock in all parts < we can't do anything about this
                 * - We have a part with a minimum/interval that is larger than our remaining qty bucket < we can so something, sometimes
                 */

                
            }

            return returned;
        }

        private class ResolverMatrix {
            public string ID { set; get; }

            public long MaxOrderable { set; get; }

            public double UnitCost { set; get; }
            public double TotalCost { set; get; }

            public ResolverMatrix(IOrderable part, long qty) {
                ID = part.ID;
                MaxOrderable = part.GetMaxOrderableQty(qty);
                UnitCost = part.GetUnitPriceForQty(MaxOrderable);
                TotalCost = UnitCost * MaxOrderable;
            }
        }

    }
}
