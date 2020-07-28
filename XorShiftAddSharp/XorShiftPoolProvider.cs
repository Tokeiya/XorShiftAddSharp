using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.ObjectPool;

namespace XorShiftAddSharp
{
    public sealed class XorShiftPoolProvider:ObjectPoolProvider
    {
        public override ObjectPool<T> Create<T>(IPooledObjectPolicy<T> policy)
        {
#warning Create_Is_NotImpl
            throw new NotImplementedException("Create is not implemented");
        }
    }
}
