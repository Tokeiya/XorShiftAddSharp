<Query Kind="Program">
  <Reference Relative="..\..\CSfmt\CSfmt\bin\Release\netcoreapp3.1\CSfmt.dll">G:\SfmtPsudoRandoms\CSfmt\CSfmt\bin\Release\netcoreapp3.1\CSfmt.dll</Reference>
  <Namespace>CSfmt</Namespace>
  <Namespace>CSfmt.Float</Namespace>
  <Namespace>CSfmt.Integer</Namespace>
</Query>

public class DigitRandom:IDisposable
{
	private const int BufferSize=640;

	private static readonly (uint limit, uint threshold, uint mask)[] Mask ={
		(0,10,15),
		(10,100,127),
		(100,1_000,1_023),
		(1_000,10_000,16_383),
		(10_000,100_000,131_071),
		(100_000,1_000_000,1_048_575),
		(1_000_000,10_000_000,16_777_215),
		(10_000_000,100_000_000,134_217_727),
		(100_000_000,1_000_000_000,1_073_741_823),
		(1_000_000_000,uint.MaxValue,uint.MaxValue)};

	private SfmtPrimitiveState _rnd;
	private AlignedArray<uint> _buffer;
	private int _cursor;
	
	public DigitRandom(uint seed)
	{
		_rnd=new SfmtPrimitiveState();
		SfmtPrimitive.InitGenRand(_rnd,seed);
		_buffer=new AlignedArray<uint>(BufferSize,16);
		Fill();
	}
	
	private void Fill()
	{
		SfmtPrimitive.FillArray32(_rnd,_buffer,BufferSize);
		_cursor=0;
	}
	
	public uint Next()
	{
		if(_cursor>=_buffer.Count)	Fill();
		return _buffer[_cursor++];
	}
	
	public uint Next(uint digit)
	{
		var mask=Mask[digit-1];
		
		for(;;)
		{
			var candidate=Next()&mask.mask;
			
			if(candidate>=mask.limit && candidate<mask.threshold) return candidate;			
		}
	}
	public void Dispose()
	{
		_rnd.Dispose();
		_buffer.Dispose();
	}
}