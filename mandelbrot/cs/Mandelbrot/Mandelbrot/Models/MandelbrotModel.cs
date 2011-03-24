using Mandelbrot.Utils;
using System.Numerics;

namespace Mandelbrot.Models {
    class MandelbrotModel {
        private FractalCalculator calculator;

        public MandelbrotModel()
        {
            calculator = new FractalCalculator((z, c) => { return z * z + c; });
            calculator.Constant = Complex.Zero;
        }
    }
}
