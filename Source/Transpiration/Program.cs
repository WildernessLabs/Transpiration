using System;
using System.Threading;
using Meadow;

namespace Transpiration
{
    class Program
    {
        static IApp app;
        public static void Main(string[] args)
        {
            Console.WriteLine("Main");

            // instantiate and run new meadow app
            app = new MeadowApp();

            Thread.Sleep(Timeout.Infinite);
        }
    }
}