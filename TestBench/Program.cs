using System;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography.X509Certificates;
using XorShiftAddSharp;

using intSpan= System.Span<int>;

namespace TestBench
{
    class Program
    {

        static void Main(string[] args)
        {
            Span<char> buff = stackalloc char[33];

            XorShiftAddCore.CalculateJumpPolynomial(buff, 1, "0x10");
        }
    }
}
