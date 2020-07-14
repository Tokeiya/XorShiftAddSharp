<Query Kind="Statements">
  <Reference Relative="..\..\CSfmt\CSfmt\bin\Release\netcoreapp3.1\CSfmt.dll">G:\SfmtPsudoRandoms\CSfmt\CSfmt\bin\Release\netcoreapp3.1\CSfmt.dll</Reference>
  <Namespace>CSfmt</Namespace>
  <Namespace>CSfmt.Float</Namespace>
  <Namespace>CSfmt.Integer</Namespace>
</Query>

#load "DigitRandom.linq"
using var rnd=new DigitRandom(42);

void write(int size)
{
	Console.Write("new []{");
	
	for (int i = 0; i < size; i++)
	{
		Console.Write($"{rnd.Next((uint)i+1)}u,");
	}
	
	Console.WriteLine("}");
}

void genArray(int size)
{
	//seeds[0] = std::unique_ptr<std::vector<uint32_t>>(new std::vector<uint32_t> { 1, 2, 3 });

	var bld = new StringBuilder($"std::unique_ptr<std::vector<uint32_t>>(new std::vector<uint32_t> {{ ");
	
	for(uint i=1;i<=size;i++)
	{
		bld.Append(rnd.Next(i)).Append("u,");
	}
	
	bld.Extract(..^1);
	bld.Append("});");
	
	Console.WriteLine(bld.ToString());
}


for(var i=1;i<=10;++i) genArray(i);