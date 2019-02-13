using PriceResolver.Models;
using PriceResolver.Models.Oderable;
using System.Collections.Generic;
using System.Linq;

namespace PriceResolver.Logic {
    class MainResolver {

        public long TotalRequestedQuantity { set; get; } = 0L;
        private long _RemainderQuantityBucket = 0L;

        private List<IOrderable> _OriginalParts;
        private List<IOrderable> _WorkingPartsBucket;

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
            ResolverResult returnedResult = new ResolverResult();
            List<ResolverMatrix> selectedForResults = new List<ResolverMatrix>();
            _WorkingPartsBucket = new List<IOrderable>(_OriginalParts); //If we don't init a new list, these can be linked and will really not work as intended

            do {
                /* 
                 * We're gonna select the most quantity at the cheapest rate we can get.
                 * Filtered by what we can order from
                 * This loop will end either when there isn't any more viable candidate parts, or we get a full order
                 */ 
                var candidatePart = _WorkingPartsBucket.Select(p => new ResolverMatrix(p, _RemainderQuantityBucket))
                                                    .Where(p => p.MaxOrderable > 0)
                                                    .OrderByDescending(p => p.UnitCost)
                                                    .FirstOrDefault();

                if (candidatePart == null)
                    break;

                _RemainderQuantityBucket -= candidatePart.MaxOrderable;
                selectedForResults.Add(candidatePart);

                //We're going to assume that all parts are keyed on their IDs (if they don't then we'd simply move this over to whatever key was unique, compound or not)
                 _WorkingPartsBucket.RemoveAll(i => i.ID == candidatePart.ID);

            } while (_RemainderQuantityBucket > 0);


            if(_RemainderQuantityBucket > 0 && _WorkingPartsBucket.Any()) {
                /*
                 * Theres a few logic reasons we're here:
                 * - We exhausted all stock in all parts < we can't do anything about this
                 * - We have a part with a minimum/interval that is larger than our remaining qty bucket < we can so something, sometimes
                 * 
                 * Here's some assumptions that aren't apparent
                 * -Every interval-ed part has already been fulfilled
                 * -The only spare we're going to be able to take from are parts with a minim of 1
                 */
                bool isAdjustmentPossible = false;
                ResolverMatrix bestResolutionTarget = _WorkingPartsBucket.Select(p => new ResolverMatrix(p, _RemainderQuantityBucket))
                                                           .OrderBy(p => p.MinAdjustment)
                                                           .FirstOrDefault();

                long miniumAdjustmentNeeded = bestResolutionTarget.MinAdjustment;

                //Any already ordered parts we can remove the adjustment from?
                // - needs to have enough orderable quantity to subtract from
                // - needs to not have a minimum
                ResolverMatrix bestDonorCandidate = selectedForResults.Where(matrixObj => {
                    if (!matrixObj.isMOQ1)
                        return false;
                    if (matrixObj.MaxOrderable < miniumAdjustmentNeeded)
                        return false;

                    return true;

                })
                .OrderByDescending(matrixObj => matrixObj.UnitCost) //select from most expensive 
                .FirstOrDefault();

                isAdjustmentPossible = bestDonorCandidate != null;

                if (isAdjustmentPossible) { //lets adjust or outright remove the donor from the result set as needed
                    selectedForResults.Remove(bestDonorCandidate); 

                    if (bestDonorCandidate.MaxOrderable > miniumAdjustmentNeeded) {
                        //we could just adjust the MaxOderable Value, but to make sure we capture all the pricing info, 
                        //we're going to recreate our object with it's new adjusted quantity

                        long newQty = bestDonorCandidate.MaxOrderable - miniumAdjustmentNeeded;
                        IOrderable originalDonorData = _OriginalParts.Single(p => p.ID == bestDonorCandidate.ID);

                        selectedForResults.Add(new ResolverMatrix(originalDonorData, newQty));
                    }

                    selectedForResults.Add(bestResolutionTarget);
                }
            }

            foreach (var result in selectedForResults)
                returnedResult.AddCandidate(result.ID, result.MaxOrderable);

            return returnedResult;
        }

        private class ResolverMatrix {
            public string ID { set; get; }

            public long MaxOrderable { set; get; }
            public long MinAdjustment { set; get; }

            public bool isMOQ1 { set; get; }

            public double UnitCost { set; get; }
            public double TotalCost { set; get; }

            public ResolverMatrix(IOrderable part, long qty) {
                ID = part.ID;

                MaxOrderable = part.GetMaxOrderableQty(qty);
                MinAdjustment = part.GetMinimumAmountToFulfillInterval(qty);

                UnitCost = part.GetUnitPriceForQty(MaxOrderable);
                TotalCost = UnitCost * MaxOrderable;

                isMOQ1 = part.QtyInterval == 1;
            }
        }

    }
}
