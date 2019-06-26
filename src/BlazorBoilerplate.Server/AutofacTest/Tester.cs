using System;

namespace BlazorBoilerplate.Server.AutofacTest
{
    internal class Tester : ITester
    {
        private int count = 1;

        public string RunTest(string text)
        {
            var x = text + new string('!', count);
            count++;
            Console.WriteLine($"RunTest: {x}");
            return x;
        }
    }
}