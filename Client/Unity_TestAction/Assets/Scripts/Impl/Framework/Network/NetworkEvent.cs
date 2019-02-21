using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AosHotfixFramework
{
    public class ServiceConnectSucceedEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(ServiceConnectSucceedEventArgs).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public int CustomServerID { get; set; }
    }

    public class ServiceConnectFailedEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(ServiceConnectFailedEventArgs).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public int CustomServerID { get; set; }
    }

    public class ServiceDisConnectEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(ServiceDisConnectEventArgs).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public int CustomServerID { get; set; }
    }
}
