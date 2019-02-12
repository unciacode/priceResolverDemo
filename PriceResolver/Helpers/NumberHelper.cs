using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceResolver.Helpers {
    public static class NumberHelper {

        public static long ZeroFloored(this long number) => number < 0 ? 0L : number;
        public static double ZeroFloored(this double number) => number < 0 ? 0L : number;

    }
}
