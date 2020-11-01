using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary
{
    /// <summary>
    /// Klasa wyliczjaca n-ty element ciagu Fibinacciego
    /// </summary>
    class Fibonacci
    {
        int n;
        int result = 0;
        int a = 0;
        int b = 1;

        public Fibonacci(int n)
        {
            for (; n > 1; n--)
            {
                Result = b;
                b = a + b;
                a = Result;
            }
        }

        public int Result { get => result; set => result = value; }
    }
}
