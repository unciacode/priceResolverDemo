using PriceResolver.Models;
using PriceResolver.Models.Oderable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceResolver.Logic {
    class MainResolver {

        public long TotalRequestedQuantity { set; get; }
        private long _RemainderQuantityBucket;

        public IEnumerable<IOrderable> PartsToOrderFrom { set; get; }

        public MainResolver(long qty) {
            TotalRequestedQuantity = qty;
            _RemainderQuantityBucket = qty;
        }

        //not the most elegant, but it gets the job done fast and well;
        public void Load(List<BaseOrderable> toLoad) {
            if(toLoad.Any())
                PartsToOrderFrom = toLoad.AsParallel().Select(tl => tl as IOrderable);
        }
        public void Load(List<OrderablePart> toLoad) {
            if(toLoad.Any())
                PartsToOrderFrom = toLoad.AsParallel().Select(tl => tl as IOrderable);
        }

        public ResolverResult RunLogic() {
            ResolverResult returned = new ResolverResult();

            return returned;
        }
    }
}
