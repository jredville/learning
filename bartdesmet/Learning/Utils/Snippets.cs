using System;
using System.Collections.Generic;
using System.Linq;
using Runner;
namespace Utils {
    [Sample("Snippets", ClassSample = false)]
    public static class Snippets {
        [Sample("Sample1")]
        public static void DivByZero() {
            IEnumerable<Double> res = null;
            try {
                res = from x in Enumerable.Range(0, 10)
                      select 1.0 / x;

            } catch (DivideByZeroException) {
                Log.I("Error");
            }

            try {
                foreach (var rcp in res)
                    Log.I(rcp);
            } catch (DivideByZeroException) {
                Log.I("Error 2");
            }
        }
    }
}
