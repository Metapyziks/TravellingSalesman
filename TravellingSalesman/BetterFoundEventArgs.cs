using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravellingSalesman
{
    public class BetterRouteFoundEventArgs : EventArgs
    {
        public readonly Route Route;

        public BetterRouteFoundEventArgs(Route route)
        {
            Route = route;
        }
    }
}
