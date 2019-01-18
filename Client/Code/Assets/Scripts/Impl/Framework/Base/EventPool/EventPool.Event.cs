namespace AosHotfixFramework
{
    internal partial class EventPool<T>
    {
        /// <summary>
        /// 事件结点。
        /// </summary>
        private sealed class Event
        {
            private readonly object mSender;
            private readonly T mEventArgs;

            public Event(object sender, T e)
            {
                mSender = sender;
                mEventArgs = e;
            }

            public object Sender
            {
                get
                {
                    return mSender;
                }
            }

            public T EventArgs
            {
                get
                {
                    return mEventArgs;
                }
            }
        }
    }
}
