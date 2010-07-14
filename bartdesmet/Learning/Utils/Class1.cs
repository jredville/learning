using Runner;
namespace Utils {
    [Sample("Class1")]
    public class Class1 {
        public Class1() {
            Log.I("In the constructor");
        }

        public void Execute() {
            Log.I("Executing");
        }
    }


    public class Class2 {
        public static void Execute() {
            Log.I("Executing");
        }
    }

}
