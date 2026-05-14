#pragma warning disable CS1591
#pragma warning disable CS0436

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using CLabs.Tickets.CompilerServices;

namespace CLabs.Tickets
{
    [AsyncMethodBuilder(typeof(AsyncTicketVoidMethodBuilder))]
    public readonly struct TicketVoid
    {
        public void Forget()
        {
        }
    }
}

