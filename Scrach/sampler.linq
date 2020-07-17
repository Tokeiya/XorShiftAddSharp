<Query Kind="Program">
  <Reference>G:\SfmtPsudoRandoms\CSfmt\CSfmt\bin\Release\netcoreapp3.1\CSfmt.dll</Reference>
  <Namespace>CSfmt</Namespace>
  <Namespace>CSfmt.Float</Namespace>
  <Namespace>CSfmt.Integer</Namespace>
</Query>

#load ".\DigitRandom"


void Main()
{	
	
	var rnd=new DigitRandom(42);

	for (var i = 1; i < 10; ++i)
	{
		Console.Write($"({rnd.Next(i)}u,\"0x{rnd.Next(i).ToString("x")}\"),");
	}
	
	
}

// Define other methods, classes and namespaces here
