using System;
using System.Numerics;
using System.Linq;
using ReactiveXaml;

namespace Mandelbrot.Utils {
    class FractalCalculator {

        // (z,c) => return z^2 + c;
        private Func<Complex, Complex, Complex> expression;
        private Func<Complex, Complex> calc;
        private Complex constant;
        private int limit;
        private Double range;
        private QueuedAsyncMRUCache<Complex, Complex> cache;

        public Complex Constant
        {
            get
            {
                return constant;
            }
            set
            {
                constant = value;
                UpdateCalc();
            }
        }

        private void UpdateCalc()
        {
            calc = (z) => expression(z, Constant);
        }

        public FractalCalculator(Func<Complex, Complex, Complex> exp, int limit = 1000, double range = 3.0) {
            this.limit = limit;
            this.range = range;
            this.expression = exp;
        }




        
    }
}
