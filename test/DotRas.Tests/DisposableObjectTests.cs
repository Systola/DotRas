﻿using System;
using DotRas.Tests.Stubs;
using NUnit.Framework;

namespace DotRas.Tests
{
    [TestFixture]
    public class DisposableObjectTests
    {
        [Test]
        public void DoesNotThrowAnExceptionWhenNotDisposed()
        {
            var target = new StubDisposableObject();
            Assert.DoesNotThrow(() => target.GuardMustNotBeDisposed());
        }

        [Test]
        public void ThrowsAnExceptionWhenDisposed()
        {
            var target = new StubDisposableObject();
            target.Dispose();

            var ex = Assert.Throws<ObjectDisposedException>(() => target.GuardMustNotBeDisposed());
            Assert.AreEqual(typeof(StubDisposableObject).FullName, ex.ObjectName);
        }

        [Test]
        public void DoesNotDisposeTheObjectMoreThanOnce()
        {
            var target = new StubDisposableObject();
            target.Dispose();

            Assert.AreEqual(1, target.Counter);

            target.Dispose();

            Assert.AreEqual(1, target.Counter);
        }
    }
}