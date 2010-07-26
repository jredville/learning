using System;
using System.Numerics;

namespace Mandelbrot.Models {
    class FractalCalculator {

        // (z,c) => return z^2 + c;
        private Func<Complex, Complex, Complex> expression;
        private int limit;
        private Double range;

        public FractalCalculator(Func<Complex, Complex, Complex> exp, int limit = 50, double range = 3.0) {
            this.limit = limit;
            this.range = range;
            this.expression = exp;
        }

        
        public Func<Complex, int?> 
    }
}
