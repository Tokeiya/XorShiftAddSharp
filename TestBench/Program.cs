﻿using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Extensions.ObjectPool;
using XorShiftAddSharp;

using intSpan= System.Span<int>;

namespace TestBench
{
    class Program
    {

        static void Main(string[] args)
        {
            XorShiftAddCore.Init(out var state,42);
            for (int i = 0; i < InternalState.Size; i++)
            {
	            Console.WriteLine(state[i]);
            }

            Console.WriteLine("");


            XorShiftAddCore.Jump(ref state, 4, "40000000");

            for (int i = 0; i < InternalState.Size; i++)
            {
	            Console.WriteLine(state[i]);
            }

            Console.WriteLine("");


            Span<char> buff = stackalloc char[33];
            XorShiftAddCore.CalculateJumpPolynomial(buff,4, "40000000");
            Console.WriteLine(new string(buff));

            XorShiftAddCore.Init(out state, 42);
            XorShiftAddCore.Jump(ref state, buff);

            for (int i = 0; i < InternalState.Size; i++)
            {
	            Console.WriteLine(state[i]);
            }



        }
    }
}
