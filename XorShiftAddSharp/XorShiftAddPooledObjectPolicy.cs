using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.ObjectPool;

namespace XorShiftAddSharp
{
    public sealed class XorShiftAddPooledObjectPolicy : PooledObjectPolicy<XorShiftAdd>
    {
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
