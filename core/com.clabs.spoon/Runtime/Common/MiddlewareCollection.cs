using System;
using System.Collections.Generic;

namespace CLabs.Spoon {
    public sealed class MiddlewareCollection<TState> where TState : struct {
        public IReadOnlyList<IMiddleware<TState>> Middleware { get; }

        public MiddlewareCollection() : this(Array.Empty<IMiddleware<TState>>()) { }

        public MiddlewareCollection(params IMiddleware<TState>[] middleware) {
            Middleware = middleware ?? Array.Empty<IMiddleware<TState>>();
        }
    }
}
