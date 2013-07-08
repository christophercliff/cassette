using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cassette
{
    public interface ICachePerRequestProvider<T>
    {
        T GetCachedValue();
        void SetCachedValue(T value);
    }
}
