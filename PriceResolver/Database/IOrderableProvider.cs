using PriceResolver.Models.Oderable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PriceResolver.Database {
    public interface IOrderableProvider {
        ManualResetEvent isReady { get; }
        string SessionId { set; get; }

        void SetSessionId(string id);

        IEnumerable<BaseOrderable> GetOderableList(string id);

        bool AddOrderable(BaseOrderable toAdd);
        bool AddOrderable(IEnumerable<BaseOrderable> toAdd);

        bool RemoveOrderableData(string id);
    }
}
