using System;
using System.Runtime.ExceptionServices;
using XorShiftAddSharp;

namespace TestBench
{
    class Program
    {
        static void Dump(ReadOnlySpan<char> scr)
        {
            var i = 0;
            for (; i < scr.Length; i++)
            {
                if (scr[i] != '\0') Console.Write(scr[i]);
                else break;
            }

            Console.WriteLine($" {i}character(s)");
        }

        static void Main(string[] args)
        {
            Span<char> buff = stackalloc char[33];

            XorShiftAddSharp.XorShiftAddCore.CalculateJumpPolynomial(buff, 10, "gg");


        }
    }
}
