using System;
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
            var a=new XorShiftAdd(42);
            var b = new XorShiftAdd(42);

            a=a.Jump(1, "0x1000000000000000000000000");
            Console.WriteLine(a.NextUnsignedInt());

            b=b.Jump("5d9ae8e063f5deee4fd1583cf8f7f9d5");
            Console.WriteLine(b.NextUnsignedInt());

            var c=new InternalState();
            XorShiftAddCore.Init(ref c,42);
            XorShiftAddCore.Jump(ref c,1, "0x1000000000000000000000000");
            Console.WriteLine(XorShiftAddCore.NextUint32(ref c));

            XorShiftAddCore.Init(ref c,42);
            XorShiftAddCore.Jump(ref c, "5d9ae8e063f5deee4fd1583cf8f7f9d5");
            Console.WriteLine(XorShiftAddCore.NextUint32(ref c));
        }
    }
}
