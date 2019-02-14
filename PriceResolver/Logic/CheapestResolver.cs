using MongoDB.Bson;
using PriceResolver.Database;
using PriceResolver.Models;
using PriceResolver.Models.Oderable;
using System.Collections.Generic;
using System.Linq;

namespace PriceResolver.Logic {
    class MainResolver : IResolver {

        public IOrderableProvider DataProvider { set; get; }

        public long TotalRequestedQuantity { set; get; } = 0L;
        private long _RemainderQuantityBucket = 0L;

        public List<BaseOrderable> PartsListLibrary { set; get; }
        private List<BaseOrderable> _WorkingPartsBucket;
        private List<ResolverMatrix> _SelectedForResults;

        public ResolverResult Result { set; get; } = new ResolverResult();
        public bool isLeftoversRequired { set; get; } = false;


        public MainResolver(long qty) {
            TotalRequestedQuantity = qty;
            _RemainderQuantityBucket = qty;
        }
        public MainResolver(long qty, IOrderableProvider orderableProvider) {
            TotalRequestedQuantity = qty;
            _RemainderQuantityBucket = qty;

            DataProvider = orderableProvider;
        }

        public void LoadFromDatabase(string mongoIdentifier) {
            PartsListLibrary = DataProvider.GetOderableList(mongoIdentifier).ToList();
        }
        public void LoadFromDatabase(ObjectId mongoIdentifier) {
            PartsListLibrary = DataProvider.GetOderableList(mongoIdentifier.ToString()).ToList();
        }

        //not the most elegant, but it gets the job done fast and well;
        public void LoadPartsList(List<BaseOrderable> toLoad) {
            if (toLoad.Any())
                PartsListLibrary = toLoad;
            else
                PartsListLibrary = new List<BaseOrderable>();
        }
        public void LoadPartsList(List<OrderablePart> toLoad) {
            if (toLoad.Any())
                PartsListLibrary = toLoad.AsParallel().Select(tl => (BaseOrderable)tl).ToList();
            else
                PartsListLibrary = new List<BaseOrderable>();

        }

        public ResolverResult RunLogic() {

            ProcessMain();

            if (isLeftoversRequired)
                ProcessLeftovers();

            PublishResults();

            return Result;
        }

        public void ProcessMain() {
            _SelectedForResults = new List<ResolverMatrix>();
            _WorkingPartsBucket = new List<BaseOrderable>(PartsListLibrary); //If we don't init a new list, these can be linked and will really not work as intended

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
                _SelectedForResults.Add(candidatePart);

                //We're going to assume that all parts are keyed on their IDs (if they don't then we'd simply move this over to whatever key was unique, compound or not)
                _WorkingPartsBucket.RemoveAll(i => i.ID == candidatePart.ID);

            } while (_RemainderQuantityBucket > 0);

            if (_RemainderQuantityBucket > 0 && _WorkingPartsBucket.Any())
                isLeftoversRequired = true;

        }

        public void ProcessLeftovers() {
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
            ResolverMatrix bestDonorCandidate = _SelectedForResults.Where(matrixObj => {
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
                _SelectedForResults.Remove(bestDonorCandidate);

                if (bestDonorCandidate.MaxOrderable > miniumAdjustmentNeeded) {
                    //we could just adjust the MaxOderable Value, but to make sure we capture all the pricing info, 
                    //we're going to recreate our object with it's new adjusted quantity

                    long newQty = bestDonorCandidate.MaxOrderable - miniumAdjustmentNeeded;
                    BaseOrderable originalDonorData = PartsListLibrary.Single(p => p.ID == bestDonorCandidate.ID);

                    _SelectedForResults.Add(new ResolverMatrix(originalDonorData, newQty));
                }

                _SelectedForResults.Add(bestResolutionTarget);
            }
        }

        private void PublishResults() {
            foreach (var result in _SelectedForResults)
                Result.AddCandidate(result.ID, result.MaxOrderable);
        }
    }
}
