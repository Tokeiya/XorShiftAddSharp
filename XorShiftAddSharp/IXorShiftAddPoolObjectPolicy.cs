using Microsoft.Extensions.ObjectPool;

namespace XorShiftAddSharp
{
	public interface IXorShiftAddPoolObjectPolicy : IPooledObjectPolicy<XorShiftAdd>
	{
		InternalState GetCurrentState();
	}
}