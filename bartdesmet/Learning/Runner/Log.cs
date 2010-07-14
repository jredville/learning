using System;

namespace Runner {
    public class Log {
        public static void I(string str) {
            Console.WriteLine(str);
        }

        public static void I(double rcp) {
            I(rcp.ToString());
        }

    }
}
