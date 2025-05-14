using TP.ConcurrentProgramming.Data;
using TP.ConcurrentProgramming.BusinessLogic;

namespace TP.ConcurrentProgramming.BusinessLogic.Test
{
    [TestClass]
    public class BusinessLogicImplementationUnitTest
    {
        [TestMethod]
        public void ConstructorTestMethod()
        {
            using (var newInstance = new BusinessLogicImplementation(new DataLayerConstructorFixcure()))
            {
                bool newInstanceDisposed = true;
                newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
                Assert.IsFalse(newInstanceDisposed);
            }
        }

        [TestMethod]
        public void DisposeTestMethod()
        {
            var dataLayerFixcure = new DataLayerDisposeFixcure();
            var newInstance = new BusinessLogicImplementation(dataLayerFixcure);
            Assert.IsFalse(dataLayerFixcure.Disposed);
            bool newInstanceDisposed = true;
            newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
            Assert.IsFalse(newInstanceDisposed);
            newInstance.Dispose();
            newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
            Assert.IsTrue(newInstanceDisposed);
            Assert.ThrowsException<ObjectDisposedException>(() => newInstance.Dispose());
            Assert.ThrowsException<ObjectDisposedException>(() => newInstance.Start(0, (position, ball) => { }));
            Assert.IsTrue(dataLayerFixcure.Disposed);
        }

        [TestMethod]
        public void StartTestMethod()
        {
            var dataLayerFixcure = new DataLayerStartFixcure();
            using (var newInstance = new BusinessLogicImplementation(dataLayerFixcure))
            {
                int called = 0;
                int numberOfBalls2Create = 10;
                newInstance.Start(
                  numberOfBalls2Create,
                  (startingPosition, ball) =>
                  {
                      called++;
                      Assert.IsNotNull(startingPosition);
                      Assert.IsNotNull(ball);
                  });
                Assert.AreEqual(1, called); // tylko jedna kula w fixture
                Assert.IsTrue(dataLayerFixcure.StartCalled);
                Assert.AreEqual(numberOfBalls2Create, dataLayerFixcure.NumberOfBallseCreated);
            }
        }

        #region testing instrumentation

        private class DataLayerConstructorFixcure : DataAbstractAPI
        {
            public override void Dispose() { }

            public override void Start(int numberOfBalls, Action<TP.ConcurrentProgramming.Data.IVector, TP.ConcurrentProgramming.Data.IBall> upperLayerHandler)
            {
                // fixture bez implementacji logiki
            }

            public override TP.ConcurrentProgramming.Data.IBall CreateBall(TP.ConcurrentProgramming.Data.IVector position, TP.ConcurrentProgramming.Data.IVector velocity)
            {
                return new DummyBall();
            }

            public override TP.ConcurrentProgramming.Data.IVector CreateVector(double x, double y)
            {
                return new DummyVector(x, y);
            }
        }

        private class DataLayerDisposeFixcure : DataAbstractAPI
        {
            internal bool Disposed = false;

            public override void Dispose() => Disposed = true;

            public override void Start(int numberOfBalls, Action<TP.ConcurrentProgramming.Data.IVector, TP.ConcurrentProgramming.Data.IBall> upperLayerHandler)
            {
                // brak implementacji potrzebnej do testu Dispose
            }

            public override TP.ConcurrentProgramming.Data.IBall CreateBall(TP.ConcurrentProgramming.Data.IVector position, TP.ConcurrentProgramming.Data.IVector velocity)
            {
                return new DummyBall();
            }

            public override TP.ConcurrentProgramming.Data.IVector CreateVector(double x, double y)
            {
                return new DummyVector(x, y);
            }
        }

        private class DataLayerStartFixcure : DataAbstractAPI
        {
            internal bool StartCalled = false;
            internal int NumberOfBallseCreated = -1;

            public override void Dispose() { }

            public override void Start(int numberOfBalls, Action<TP.ConcurrentProgramming.Data.IVector, TP.ConcurrentProgramming.Data.IBall> upperLayerHandler)
            {
                StartCalled = true;
                NumberOfBallseCreated = numberOfBalls;

                // Zwracamy DummyBall jako Data.IBall, BusinessLogic opakuje go w BusinessBall
                upperLayerHandler(new DummyVector(0, 0), new DummyBall());
            }

            public override TP.ConcurrentProgramming.Data.IBall CreateBall(TP.ConcurrentProgramming.Data.IVector position, TP.ConcurrentProgramming.Data.IVector velocity)
            {
                return new DummyBall();
            }

            public override TP.ConcurrentProgramming.Data.IVector CreateVector(double x, double y)
            {
                return new DummyVector(x, y);
            }
        }

        private record DummyVector(double x, double y) : TP.ConcurrentProgramming.Data.IVector;

        private class DummyBall : TP.ConcurrentProgramming.Data.IBall
        {
            public TP.ConcurrentProgramming.Data.IVector Velocity { get; set; } = new DummyVector(0, 0);
            public event EventHandler<TP.ConcurrentProgramming.Data.IVector>? NewPositionNotification;
        }

        #endregion
    }
}
