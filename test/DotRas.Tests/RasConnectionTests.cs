﻿using System;
using System.Linq;
using System.Threading;
using DotRas.Internal.Abstractions.Services;
using DotRas.Internal.Interop;
using DotRas.Internal;
using DotRas.Tests.Stubs;
using Moq;
using NUnit.Framework;

namespace DotRas.Tests
{
    [TestFixture]
    public class RasConnectionTests
    {
        private Mock<IServiceProvider> container;

        [SetUp]
        public void Setup()
        {
            container = new Mock<IServiceProvider>();
            ServiceLocator.Default = container.Object;
        }

        [TearDown]
        public void TearDown()
        {
            ServiceLocator.Clear();
        }

        [Test]
        public void EqualsTheHashCodeOfTheHandle()
        {
            var handle = new IntPtr(1);

            var target = new Mock<RasConnection>();
            target.Setup(o => o.Handle).Returns(handle);
            target.Setup(o => o.GetHashCode()).CallBase();

            var actual = target.Object.GetHashCode();

            Assert.AreEqual(handle.GetHashCode(), actual);
        }

        [Test]
        public void DoesNotEqualNullWhenUsingEqualsWithObject()
        {
            var target = new Mock<RasConnection>();

            object other = null;
            target.Setup(o => o.Equals(other)).CallBase();

            Assert.False(target.Object.Equals(other));
        }

        [Test]
        public void EqualsTheOtherConnection()
        {
            var target = new Mock<RasConnection>();
            target.Setup(o => o.Handle).Returns(new IntPtr(1));

            var other = new Mock<RasConnection>();
            other.Setup(o => o.Handle).Returns(new IntPtr(1));

            target.Setup(o => o.Equals(other.Object)).CallBase();

            Assert.True(target.Object == other.Object);
        }

        [Test]
        public void EqualsTheOtherConnectionWhenOtherIsAnObject()
        {
            var target = new Mock<RasConnection>();
            target.Setup(o => o.Handle).Returns(new IntPtr(1));
            
            var other = new Mock<RasConnection>();
            other.Setup(o => o.Handle).Returns(new IntPtr(1));

            target.Setup(o => o.Equals(other.Object)).CallBase();

            object otherTarget = other.Object;
            target.Setup(o => o.Equals(otherTarget)).CallBase();

            Assert.True(target.Object.Equals(otherTarget));
        }

        [Test]
        public void DoesNotEqualTheOtherConnection()
        {
            var target = new Mock<RasConnection>();
            target.Setup(o => o.Handle).Returns(new IntPtr(1));

            var other = new Mock<RasConnection>();
            other.Setup(o => o.Handle).Returns(new IntPtr(2));

            Assert.True(target.Object != other.Object);
        }

        [Test]
        public void DoesEqualWhenBothAreNull()
        {
            RasConnection targetA = null;
            RasConnection targetB = null;

            Assert.True(targetA == targetB);
        }

        [Test]
        public void DoesNotEqualWhenOneIsExpectedToBeNull()
        {
            var target = new Mock<RasConnection>();

            Assert.False(target.Object == null);
        }

        [Test]
        public void DoesNotEqualWhenOtherIsExpectedToBeNullUsingEquals()
        {
            var target = new Mock<RasConnection>();

            RasConnection other = null;
            target.Setup(o => o.Equals(other)).CallBase();

            Assert.False(target.Object.Equals(other));
        }

        [Test]
        public void DoesNotEqualWhenOneIsExpectedToBeNullWhenUsingYodaSyntax()
        {
            var target = new Mock<RasConnection>();

            Assert.False(null == target.Object);
        }

        [Test]
        public void EnumeratesTheConnectionCorrectly()
        {
            var enumConnections = new Mock<IRasEnumConnections>();
            enumConnections.Setup(o => o.EnumerateConnections()).Returns(new RasConnection[0]);

            container.Setup(o => o.GetService(typeof(IRasEnumConnections))).Returns(enumConnections.Object);
            var result = RasConnection.EnumerateConnections();
            
            Assert.IsNotNull(result);
            container.Verify(o => o.GetService(typeof(IRasEnumConnections)), Times.Once);
            enumConnections.Verify(o => o.EnumerateConnections(), Times.Once);
        }

        [Test]
        public void WillReturnTheCorrectConnectionWhenUsingLinq()
        {
            var connection1 = new Mock<RasConnection>();
            connection1.Setup(o => o.EntryName).Returns("Test1");

            var connection2 = new Mock<RasConnection>();
            connection2.Setup(o => o.EntryName).Returns("Test2");

            var enumConnections = new Mock<IRasEnumConnections>();
            enumConnections.Setup(o => o.EnumerateConnections()).Returns(new[] { connection1.Object, connection2.Object });

            container.Setup(o => o.GetService(typeof(IRasEnumConnections))).Returns(enumConnections.Object);

            var result = RasConnection.EnumerateConnections().SingleOrDefault(o => o.EntryName == "Test2");

            Assert.IsNotNull(result);
            Assert.AreSame(connection2.Object, result);
        }

        [Test]
        public void ReturnTheHandle()
        {
            var handle = new IntPtr(1);
            var device = new TestDevice("Test");
            var entryName = "Test";
            var phoneBook = @"C:\Test.pbk";
            var entryId = Guid.NewGuid();
            var options = new RasConnectionOptions(Ras.RASCF.AllUsers);
            var sessionId = new Luid(1, 1);
            var correlationId = Guid.NewGuid();

            var rasGetConnectStatus = new Mock<IRasGetConnectStatus>();
            var rasGetConnectionStatistics = new Mock<IRasGetConnectionStatistics>();
            var rasHangUp = new Mock<IRasHangUp>();
            var rasClearConnectionStatistics = new Mock<IRasClearConnectionStatistics>();

            var target = new RasConnection(handle, device, entryName, phoneBook, entryId, options, sessionId, correlationId, rasGetConnectStatus.Object, rasGetConnectionStatistics.Object, rasHangUp.Object, rasClearConnectionStatistics.Object);
            Assert.AreEqual(handle, target.Handle);
        }

        [Test]
        public void ReturnTheDevice()
        {
            var handle = new IntPtr(1);
            var device = new TestDevice("Test");
            var entryName = "Test";
            var phoneBook = @"C:\Test.pbk";
            var entryId = Guid.NewGuid();
            var options = new RasConnectionOptions(Ras.RASCF.AllUsers);
            var sessionId = new Luid(1, 1);
            var correlationId = Guid.NewGuid();

            var rasGetConnectStatus = new Mock<IRasGetConnectStatus>();
            var rasGetConnectionStatistics = new Mock<IRasGetConnectionStatistics>();
            var rasHangUp = new Mock<IRasHangUp>();
            var rasClearConnectionStatistics = new Mock<IRasClearConnectionStatistics>();

            var target = new RasConnection(handle, device, entryName, phoneBook, entryId, options, sessionId, correlationId, rasGetConnectStatus.Object, rasGetConnectionStatistics.Object, rasHangUp.Object, rasClearConnectionStatistics.Object);
            Assert.AreEqual(device, target.Device);
        }

        [Test]
        public void ReturnTheEntryName()
        {
            var handle = new IntPtr(1);
            var device = new TestDevice("Test");
            var entryName = "Test";
            var phoneBook = @"C:\Test.pbk";
            var entryId = Guid.NewGuid();
            var options = new RasConnectionOptions(Ras.RASCF.AllUsers);
            var sessionId = new Luid(1, 1);
            var correlationId = Guid.NewGuid();

            var rasGetConnectStatus = new Mock<IRasGetConnectStatus>();
            var rasGetConnectionStatistics = new Mock<IRasGetConnectionStatistics>();
            var rasHangUp = new Mock<IRasHangUp>();
            var rasClearConnectionStatistics = new Mock<IRasClearConnectionStatistics>();

            var target = new RasConnection(handle, device, entryName, phoneBook, entryId, options, sessionId, correlationId, rasGetConnectStatus.Object, rasGetConnectionStatistics.Object, rasHangUp.Object, rasClearConnectionStatistics.Object);
            Assert.AreEqual(entryName, target.EntryName);
        }

        [Test]
        public void ReturnThePhoneBook()
        {
            var handle = new IntPtr(1);
            var device = new TestDevice("Test");
            var entryName = "Test";
            var phoneBook = @"C:\Test.pbk";
            var entryId = Guid.NewGuid();
            var options = new RasConnectionOptions(Ras.RASCF.AllUsers);
            var sessionId = new Luid(1, 1);
            var correlationId = Guid.NewGuid();

            var rasGetConnectStatus = new Mock<IRasGetConnectStatus>();
            var rasGetConnectionStatistics = new Mock<IRasGetConnectionStatistics>();
            var rasHangUp = new Mock<IRasHangUp>();
            var rasClearConnectionStatistics = new Mock<IRasClearConnectionStatistics>();

            var target = new RasConnection(handle, device, entryName, phoneBook, entryId, options, sessionId, correlationId, rasGetConnectStatus.Object, rasGetConnectionStatistics.Object, rasHangUp.Object, rasClearConnectionStatistics.Object);
            Assert.AreEqual(phoneBook, target.PhoneBookPath);
        }        

        [Test]
        public void ReturnTheEntryId()
        {
            var handle = new IntPtr(1);
            var device = new TestDevice("Test");
            var entryName = "Test";
            var phoneBook = @"C:\Test.pbk";
            var entryId = Guid.NewGuid();
            var options = new RasConnectionOptions(Ras.RASCF.AllUsers);
            var sessionId = new Luid(1, 1);
            var correlationId = Guid.NewGuid();

            var rasGetConnectStatus = new Mock<IRasGetConnectStatus>();
            var rasGetConnectionStatistics = new Mock<IRasGetConnectionStatistics>();
            var rasHangUp = new Mock<IRasHangUp>();
            var rasClearConnectionStatistics = new Mock<IRasClearConnectionStatistics>();

            var target = new RasConnection(handle, device, entryName, phoneBook, entryId, options, sessionId, correlationId, rasGetConnectStatus.Object, rasGetConnectionStatistics.Object, rasHangUp.Object, rasClearConnectionStatistics.Object);
            Assert.AreEqual(entryId, target.EntryId);
        }

        [Test]
        public void ReturnTheOptions()
        {
            var handle = new IntPtr(1);
            var device = new TestDevice("Test");
            var entryName = "Test";
            var phoneBook = @"C:\Test.pbk";
            var entryId = Guid.NewGuid();
            var options = new RasConnectionOptions(Ras.RASCF.AllUsers);
            var sessionId = new Luid(1, 1);
            var correlationId = Guid.NewGuid();

            var rasGetConnectStatus = new Mock<IRasGetConnectStatus>();
            var rasGetConnectionStatistics = new Mock<IRasGetConnectionStatistics>();
            var rasHangUp = new Mock<IRasHangUp>();
            var rasClearConnectionStatistics = new Mock<IRasClearConnectionStatistics>();

            var target = new RasConnection(handle, device, entryName, phoneBook, entryId, options, sessionId, correlationId, rasGetConnectStatus.Object, rasGetConnectionStatistics.Object, rasHangUp.Object, rasClearConnectionStatistics.Object);
            Assert.AreEqual(options, target.Options);
        }

        [Test]
        public void ReturnTheSessionId()
        {
            var handle = new IntPtr(1);
            var device = new TestDevice("Test");
            var entryName = "Test";
            var phoneBook = @"C:\Test.pbk";
            var entryId = Guid.NewGuid();
            var options = new RasConnectionOptions(Ras.RASCF.AllUsers);
            var sessionId = new Luid(1, 1);
            var correlationId = Guid.NewGuid();

            var rasGetConnectStatus = new Mock<IRasGetConnectStatus>();
            var rasGetConnectionStatistics = new Mock<IRasGetConnectionStatistics>();
            var rasHangUp = new Mock<IRasHangUp>();
            var rasClearConnectionStatistics = new Mock<IRasClearConnectionStatistics>();

            var target = new RasConnection(handle, device, entryName, phoneBook, entryId, options, sessionId, correlationId, rasGetConnectStatus.Object, rasGetConnectionStatistics.Object, rasHangUp.Object, rasClearConnectionStatistics.Object);
            Assert.AreEqual(sessionId, target.SessionId);
        }

        [Test]
        public void ReturnTheCorrelationId()
        {
            var handle = new IntPtr(1);
            var device = new TestDevice("Test");
            var entryName = "Test";
            var phoneBook = @"C:\Test.pbk";
            var entryId = Guid.NewGuid();
            var options = new RasConnectionOptions(Ras.RASCF.AllUsers);
            var sessionId = new Luid(1, 1);
            var correlationId = Guid.NewGuid();

            var rasGetConnectStatus = new Mock<IRasGetConnectStatus>();
            var rasGetConnectionStatistics = new Mock<IRasGetConnectionStatistics>();
            var rasHangUp = new Mock<IRasHangUp>();
            var rasClearConnectionStatistics = new Mock<IRasClearConnectionStatistics>();

            var target = new RasConnection(handle, device, entryName, phoneBook, entryId, options, sessionId, correlationId, rasGetConnectStatus.Object, rasGetConnectionStatistics.Object, rasHangUp.Object, rasClearConnectionStatistics.Object);
            Assert.AreEqual(correlationId, target.CorrelationId);
        }

        [Test]
        public void ConstructorThrowsExceptionWhenHandleIsNull()
        {
            var device = new TestDevice("Test");
            var entryName = "Test";
            var phoneBook = @"C:\Test.pbk";
            var entryId = Guid.NewGuid();
            var options = new RasConnectionOptions(Ras.RASCF.AllUsers);
            var sessionId = new Luid(1, 1);
            var correlationId = Guid.NewGuid();

            var rasGetConnectStatus = new Mock<IRasGetConnectStatus>();
            var rasGetConnectionStatistics = new Mock<IRasGetConnectionStatistics>();
            var rasHangUp = new Mock<IRasHangUp>();
            var rasClearConnectionStatistics = new Mock<IRasClearConnectionStatistics>();

            Assert.Throws<ArgumentNullException>(() =>
            {
                var unused = new RasConnection(IntPtr.Zero, device, entryName, phoneBook, entryId, options, sessionId, correlationId, rasGetConnectStatus.Object, rasGetConnectionStatistics.Object, rasHangUp.Object, rasClearConnectionStatistics.Object);
            });
        }

        [Test]
        public void ConstructorThrowsExceptionWhenDeviceIsNull()
        {
            var handle = new IntPtr(1);
            var entryName = "Test";
            var phoneBook = @"C:\Test.pbk";
            var entryId = Guid.NewGuid();
            var options = new RasConnectionOptions(Ras.RASCF.AllUsers);
            var sessionId = new Luid(1, 1);
            var correlationId = Guid.NewGuid();

            var rasGetConnectStatus = new Mock<IRasGetConnectStatus>();
            var rasGetConnectionStatistics = new Mock<IRasGetConnectionStatistics>();
            var rasHangUp = new Mock<IRasHangUp>();
            var rasClearConnectionStatistics = new Mock<IRasClearConnectionStatistics>();

            Assert.Throws<ArgumentNullException>(() =>
            {
                var unused = new RasConnection(handle, null, entryName, phoneBook, entryId, options, sessionId, correlationId, rasGetConnectStatus.Object, rasGetConnectionStatistics.Object, rasHangUp.Object, rasClearConnectionStatistics.Object);
            });
        }

        [Test]
        public void ConstructorThrowsExceptionWhenEntryNameIsNull()
        {
            var handle = new IntPtr(1);
            var device = new TestDevice("Test");
            var phoneBook = @"C:\Test.pbk";
            var entryId = Guid.NewGuid();
            var options = new RasConnectionOptions(Ras.RASCF.AllUsers);
            var sessionId = new Luid(1, 1);
            var correlationId = Guid.NewGuid();

            var rasGetConnectStatus = new Mock<IRasGetConnectStatus>();
            var rasGetConnectionStatistics = new Mock<IRasGetConnectionStatistics>();
            var rasHangUp = new Mock<IRasHangUp>();
            var rasClearConnectionStatistics = new Mock<IRasClearConnectionStatistics>();

            Assert.Throws<ArgumentNullException>(() =>
            {
                var unused = new RasConnection(handle, device, null, phoneBook, entryId, options, sessionId, correlationId, rasGetConnectStatus.Object, rasGetConnectionStatistics.Object, rasHangUp.Object, rasClearConnectionStatistics.Object);
            });
        }

        [Test]
        public void ConstructorThrowsExceptionWhenEntryNameIsEmpty()
        {
            var handle = new IntPtr(1);
            var device = new TestDevice("Test");
            var phoneBook = @"C:\Test.pbk";
            var entryId = Guid.NewGuid();
            var options = new RasConnectionOptions(Ras.RASCF.AllUsers);
            var sessionId = new Luid(1, 1);
            var correlationId = Guid.NewGuid();

            var rasGetConnectStatus = new Mock<IRasGetConnectStatus>();
            var rasGetConnectionStatistics = new Mock<IRasGetConnectionStatistics>();
            var rasHangUp = new Mock<IRasHangUp>();
            var rasClearConnectionStatistics = new Mock<IRasClearConnectionStatistics>();

            Assert.Throws<ArgumentNullException>(() =>
            {
                var unused = new RasConnection(handle, device, "", phoneBook, entryId, options, sessionId, correlationId, rasGetConnectStatus.Object, rasGetConnectionStatistics.Object, rasHangUp.Object, rasClearConnectionStatistics.Object);
            });
        }

        [Test]
        public void ConstructorThrowsExceptionWhenEntryNameIsWhitespace()
        {
            var handle = new IntPtr(1);
            var device = new TestDevice("Test");
            var phoneBook = @"C:\Test.pbk";
            var entryId = Guid.NewGuid();
            var options = new RasConnectionOptions(Ras.RASCF.AllUsers);
            var sessionId = new Luid(1, 1);
            var correlationId = Guid.NewGuid();

            var rasGetConnectStatus = new Mock<IRasGetConnectStatus>();
            var rasGetConnectionStatistics = new Mock<IRasGetConnectionStatistics>();
            var rasHangUp = new Mock<IRasHangUp>();
            var rasClearConnectionStatistics = new Mock<IRasClearConnectionStatistics>();

            Assert.Throws<ArgumentNullException>(() =>
            {
                var unused = new RasConnection(handle, device, "                ", phoneBook, entryId, options, sessionId, correlationId, rasGetConnectStatus.Object, rasGetConnectionStatistics.Object, rasHangUp.Object, rasClearConnectionStatistics.Object);
            });
        }

        [Test]
        public void ConstructorThrowsExceptionWhenPhoneBookIsNull()
        {
            var handle = new IntPtr(1);
            var device = new TestDevice("Test");
            var entryName = "Test";
            var entryId = Guid.NewGuid();
            var options = new RasConnectionOptions(Ras.RASCF.AllUsers);
            var sessionId = new Luid(1, 1);
            var correlationId = Guid.NewGuid();

            var rasGetConnectStatus = new Mock<IRasGetConnectStatus>();
            var rasGetConnectionStatistics = new Mock<IRasGetConnectionStatistics>();
            var rasHangUp = new Mock<IRasHangUp>();
            var rasClearConnectionStatistics = new Mock<IRasClearConnectionStatistics>();

            Assert.Throws<ArgumentNullException>(() =>
            {
                var unused = new RasConnection(handle, device, entryName, null, entryId, options, sessionId, correlationId, rasGetConnectStatus.Object, rasGetConnectionStatistics.Object, rasHangUp.Object, rasClearConnectionStatistics.Object);
            });
        }

        [Test]
        public void ConstructorThrowsExceptionWhenPhoneBookIsEmpty()
        {
            var handle = new IntPtr(1);
            var device = new TestDevice("Test");
            var entryName = "Test";
            var entryId = Guid.NewGuid();
            var options = new RasConnectionOptions(Ras.RASCF.AllUsers);
            var sessionId = new Luid(1, 1);
            var correlationId = Guid.NewGuid();

            var rasGetConnectStatus = new Mock<IRasGetConnectStatus>();
            var rasGetConnectionStatistics = new Mock<IRasGetConnectionStatistics>();
            var rasHangUp = new Mock<IRasHangUp>();
            var rasClearConnectionStatistics = new Mock<IRasClearConnectionStatistics>();

            Assert.Throws<ArgumentNullException>(() =>
            {
                var unused = new RasConnection(handle, device, entryName, "", entryId, options, sessionId, correlationId, rasGetConnectStatus.Object, rasGetConnectionStatistics.Object, rasHangUp.Object, rasClearConnectionStatistics.Object);
            });
        }

        [Test]
        public void ConstructorThrowsExceptionWhenPhoneBookIsWhitespace()
        {
            var handle = new IntPtr(1);
            var device = new TestDevice("Test");
            var entryName = "Test";
            var entryId = Guid.NewGuid();
            var options = new RasConnectionOptions(Ras.RASCF.AllUsers);
            var sessionId = new Luid(1, 1);
            var correlationId = Guid.NewGuid();

            var rasGetConnectStatus = new Mock<IRasGetConnectStatus>();
            var rasGetConnectionStatistics = new Mock<IRasGetConnectionStatistics>();
            var rasHangUp = new Mock<IRasHangUp>();
            var rasClearConnectionStatistics = new Mock<IRasClearConnectionStatistics>();

            Assert.Throws<ArgumentNullException>(() =>
            {
                var unused = new RasConnection(handle, device, entryName, "             ", entryId, options, sessionId, correlationId, rasGetConnectStatus.Object, rasGetConnectionStatistics.Object, rasHangUp.Object, rasClearConnectionStatistics.Object);
            });
        }

        [Test]
        public void RetrievesTheStatusAsExpected()
        {
            var handle = new IntPtr(1);
            var device = new TestDevice("Test");
            var entryName = "Test";
            var phoneBook = @"C:\Test.pbk";
            var entryId = Guid.NewGuid();
            var options = new RasConnectionOptions(Ras.RASCF.AllUsers);
            var sessionId = new Luid(1, 1);
            var correlationId = Guid.NewGuid();

            var status = new Mock<RasConnectionStatus>();

            var rasGetConnectStatus = new Mock<IRasGetConnectStatus>();
            var rasGetConnectionStatistics = new Mock<IRasGetConnectionStatistics>();
            var rasHangUp = new Mock<IRasHangUp>();
            var rasClearConnectionStatistics = new Mock<IRasClearConnectionStatistics>();

            var target = new RasConnection(handle, device, entryName, phoneBook, entryId, options, sessionId, correlationId, rasGetConnectStatus.Object, rasGetConnectionStatistics.Object, rasHangUp.Object, rasClearConnectionStatistics.Object);
            rasGetConnectStatus.Setup(o => o.GetConnectionStatus(target)).Returns(status.Object).Verifiable();

            var result = target.GetStatus();

            Assert.AreEqual(status.Object, result);
            rasGetConnectStatus.Verify();
        }

        [Test]
        public void RetrievesTheStatisticsAsExpected()
        {
            var handle = new IntPtr(1);
            var device = new TestDevice("Test");
            var entryName = "Test";
            var phoneBook = @"C:\Test.pbk";
            var entryId = Guid.NewGuid();
            var options = new RasConnectionOptions(Ras.RASCF.AllUsers);
            var sessionId = new Luid(1, 1);
            var correlationId = Guid.NewGuid();
            
            var rasGetConnectStatus = new Mock<IRasGetConnectStatus>();

            var rasConnectionStatistics = new Mock<RasConnectionStatistics>();
            var rasGetConnectionStatistics = new Mock<IRasGetConnectionStatistics>();
            var rasHangUp = new Mock<IRasHangUp>();
            var rasClearConnectionStatistics = new Mock<IRasClearConnectionStatistics>();

            var target = new RasConnection(handle, device, entryName, phoneBook, entryId, options, sessionId, correlationId, rasGetConnectStatus.Object, rasGetConnectionStatistics.Object, rasHangUp.Object, rasClearConnectionStatistics.Object);

            rasGetConnectionStatistics.Setup(o => o.GetConnectionStatistics(target)).Returns(rasConnectionStatistics.Object).Verifiable();

            var result = target.GetStatistics();

            Assert.AreEqual(rasConnectionStatistics.Object, result);
            rasGetConnectionStatistics.Verify();
        }

        [Test]
        public void DisconnectTheConnectionAsExpected()
        {
            var handle = new IntPtr(1);
            var device = new TestDevice("Test");
            var entryName = "Test";
            var phoneBook = @"C:\Test.pbk";
            var entryId = Guid.NewGuid();
            var options = new RasConnectionOptions(Ras.RASCF.AllUsers);
            var sessionId = new Luid(1, 1);
            var correlationId = Guid.NewGuid();

            var cancellationToken = CancellationToken.None;

            var rasGetConnectStatus = new Mock<IRasGetConnectStatus>();
            var rasGetConnectionStatistics = new Mock<IRasGetConnectionStatistics>();
            var rasHangUp = new Mock<IRasHangUp>();
            var rasClearConnectionStatistics = new Mock<IRasClearConnectionStatistics>();

            var target = new RasConnection(handle, device, entryName, phoneBook, entryId, options, sessionId, correlationId, rasGetConnectStatus.Object, rasGetConnectionStatistics.Object, rasHangUp.Object, rasClearConnectionStatistics.Object);
            target.Disconnect(cancellationToken);

            rasHangUp.Verify(o => o.HangUp(target, It.IsAny<bool>(), cancellationToken), Times.Once);
        }

        [Test]
        public void ClearsTheConnectionStatisticsAsExpected()
        {
            var handle = new IntPtr(1);
            var device = new TestDevice("Test");
            var entryName = "Test";
            var phoneBook = @"C:\Test.pbk";
            var entryId = Guid.NewGuid();
            var options = new RasConnectionOptions(Ras.RASCF.AllUsers);
            var sessionId = new Luid(1, 1);
            var correlationId = Guid.NewGuid();

            var rasGetConnectStatus = new Mock<IRasGetConnectStatus>();
            var rasGetConnectionStatistics = new Mock<IRasGetConnectionStatistics>();
            var rasHangUp = new Mock<IRasHangUp>();
            var rasClearConnectionStatistics = new Mock<IRasClearConnectionStatistics>();

            var target = new RasConnection(handle, device, entryName, phoneBook, entryId, options, sessionId, correlationId, rasGetConnectStatus.Object, rasGetConnectionStatistics.Object, rasHangUp.Object, rasClearConnectionStatistics.Object);
            target.ClearStatistics();

            rasClearConnectionStatistics.Verify(o => o.ClearConnectionStatistics(target), Times.Once);
        }

        [Test]
        public void DisconnectShouldCloseAllReferencesByDefault()
        {
            var handle = new IntPtr(1);
            var device = new TestDevice("Test");
            var entryName = "Test";
            var phoneBook = @"C:\Test.pbk";
            var entryId = Guid.NewGuid();
            var options = new RasConnectionOptions(Ras.RASCF.AllUsers);
            var sessionId = new Luid(1, 1);
            var correlationId = Guid.NewGuid();

            var rasGetConnectStatus = new Mock<IRasGetConnectStatus>();
            var rasGetConnectionStatistics = new Mock<IRasGetConnectionStatistics>();
            var rasHangUp = new Mock<IRasHangUp>();
            var rasClearConnectionStatistics = new Mock<IRasClearConnectionStatistics>();

            var target = new RasConnection(handle, device, entryName, phoneBook, entryId, options, sessionId, correlationId, rasGetConnectStatus.Object, rasGetConnectionStatistics.Object, rasHangUp.Object, rasClearConnectionStatistics.Object);
            target.Disconnect(CancellationToken.None);

            rasHangUp.Verify(o => o.HangUp(target, true, CancellationToken.None), Times.Once);
        }
    }
}