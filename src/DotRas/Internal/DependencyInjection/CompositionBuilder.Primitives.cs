﻿using System;
using DotRas.Internal.Abstractions.DependencyInjection;
using DotRas.Internal.Abstractions.Primitives;
using DotRas.Internal.Primitives;

namespace DotRas.Internal.DependencyInjection
{
    internal static partial class CompositionBuilder
    {
        private static void RegisterThreading(ICompositionRegistry registry)
        {
            registry.RegisterCallback<IManualResetEvent>(
                c => new ManualResetEvent());

            registry.RegisterCallback<IValueWaiter<IntPtr>>(
                c => new ValueWaiter<IntPtr>(
                    c.GetRequiredService<IManualResetEvent>()));

            registry.RegisterCallback<IFileSystem>(
                c => new FileSystem());
        }
    }
}