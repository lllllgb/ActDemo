using System;
using System.Collections.Generic;

namespace AosHotfixFramework
{
    public interface IObjectManager
    {
        void Awake(ObjectDisposer disposer);

        void Awake<P1>(ObjectDisposer disposer, P1 p1);

        void Awake<P1, P2>(ObjectDisposer disposer, P1 p1, P2 p2);

        void Awake<P1, P2, P3>(ObjectDisposer disposer, P1 p1, P2 p2, P3 p3);
    }
}
