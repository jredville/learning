using System;
using System.Collections.Generic;
using System.Linq;

namespace MinLINQ
{
    static class Program
    {
        static void Main(string[] args)
        {
            for (var c = new Counter(); c.Get() < 10; c.Inc(1))
                Console.WriteLine(c.Get());
            Console.ReadLine();
        }
    }
}
