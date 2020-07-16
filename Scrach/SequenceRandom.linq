<Query Kind="Program" />

public class SequenceRandom : System.Random
{
	private int _current;

	public SequenceRandom(int initial) => _current = initial;

	private int InternalSample()
	{
		if (++_current == int.MaxValue) _current = 0;
		return _current;
	}

	protected override double Sample()
	{
		const double factor=1.0/int.MaxValue;
		return InternalSample()*factor;
	}

	public override int Next()=>InternalSample();

	public override double NextDouble()=>Sample();

	public override int Next(int maxValue)=> (int)(Sample() * maxValue);
	


}
