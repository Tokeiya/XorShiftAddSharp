using System;
using Microsoft.Extensions.ObjectPool;

namespace XorShiftAddSharp
{
    /// <summary>
    /// Distribute and manage the XorShiftAdd instances, divided into 2^32.
    /// This one's internal state is 2^100 jumped from recent created instance.
    /// </summary>
    public sealed class XorShiftAddPooledObjectPolicy : PooledObjectPolicy<XorShiftAdd>
    {
        private const string Polynomial = "1ab1a440fe2c6b42c36ae0425a31262";
        private InternalState _state;

        public XorShiftAddPooledObjectPolicy(uint seed)
        {

#warning XorShiftAddPooledObjectPolicy_Is_NotImpl
            throw new NotImplementedException("XorShiftAddPooledObjectPolicy is not implemented");
        }

        public XorShiftAddPooledObjectPolicy(uint[] keys)
        {
#warning XorShiftAddPooledObjectPolicy_Is_NotImpl
            throw new NotImplementedException("XorShiftAddPooledObjectPolicy is not implemented");
        }



        public override XorShiftAdd Create()
        {
            throw new NotImplementedException();
        }

        public override bool Return(XorShiftAdd obj)
        {
            throw new NotImplementedException();
        }
    }
}
