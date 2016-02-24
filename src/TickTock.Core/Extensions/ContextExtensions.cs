using System;

namespace TickTock.Core.Extensions
{
    public static class ContextExtensions
    {
        public static TResult Apply<TContext, TResult>(this Action<TContext> with, Func<TContext, TResult> callback)
            where TContext : class, new()
        {
            TContext context = new TContext();
            with(context);
            return callback(context);
        }
    }
}