using System;
using System.Runtime.ExceptionServices;

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

            //(2771179739, "e700ac6c84f58579", "1671ade5bd6e8b5043b2c658b18e87de")
            XorShiftAddSharp.XsAddCore.CalculateJumpPolynomial(buff, 2771179739, "e700ac6c84f58579");
            Dump(buff);

            for (int i = 0; i < buff.Length; i++) buff[i] = '\0';

            XorShiftAddSharp.XsAddCore.CalculateJumpPolynomial(buff, 2771179739, "e700ac6c84f58579");
            Dump(buff);

        }
    }
}
