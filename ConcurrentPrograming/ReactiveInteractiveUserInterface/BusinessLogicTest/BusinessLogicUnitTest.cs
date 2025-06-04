using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.BusinessLogic.Test
{
    [TestClass]
    public class BusinessLogicImplementationUnitTest
    {
        [TestMethod]
        public void ConstructorTestMethod()
        {
            var newInstance = new BusinessLogicImplementation(new DataLayerConstructorFixcure());

            bool disposed = true;
            newInstance.CheckObjectDisposed(x => disposed = x);
            Assert.IsTrue(disposed);

            newInstance.Dispose();
        }



        [TestMethod]
        public void DisposeTestMethod()
        {
            var dataLayer = new DataLayerDisposeFixcure();
            var instance = new BusinessLogicImplementation(dataLayer);
            Assert.IsFalse(dataLayer.Disposed);
            instance.Dispose();
            Assert.IsTrue(dataLayer.Disposed);
        }



        #region testing instrumentation

        private class DataLayerConstructorFixcure : DataAbstractAPI
        {
            public override void Dispose() { }

            public override void Start(int numberOfBalls, Action<TP.ConcurrentProgramming.Data.IVector, TP.ConcurrentProgramming.Data.IBall> upperLayerHandler)
            {

            }

            public override TP.ConcurrentProgramming.Data.IBall CreateBall(TP.ConcurrentProgramming.Data.IVector position, TP.ConcurrentProgramming.Data.IVector velocity)
            {
                return new DummyBall();
            }

            public override TP.ConcurrentProgramming.Data.IVector CreateVector(double x, double y)
            {
                return new DummyVector(x, y);
            }

            public override void MoveBall(Data.IBall ball)
            {
                throw new NotImplementedException();
            }

            public override void Stop()
            {
                throw new NotImplementedException();
            }
        }

        private class DataLayerDisposeFixcure : DataAbstractAPI
        {
            internal bool Disposed = false;

            public override void Dispose() => Disposed = true;

            public override void Start(int numberOfBalls, Action<TP.ConcurrentProgramming.Data.IVector, TP.ConcurrentProgramming.Data.IBall> upperLayerHandler)
            {

            }

            public override TP.ConcurrentProgramming.Data.IBall CreateBall(TP.ConcurrentProgramming.Data.IVector position, TP.ConcurrentProgramming.Data.IVector velocity)
            {
                return new DummyBall();
            }

            public override TP.ConcurrentProgramming.Data.IVector CreateVector(double x, double y)
            {
                return new DummyVector(x, y);
            }

            public override void MoveBall(Data.IBall ball)
            {
                throw new NotImplementedException();
            }

            public override void Stop()
            {
                throw new NotImplementedException();
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

            public override void MoveBall(Data.IBall ball)
            {
                throw new NotImplementedException();
            }

            public override void Stop()
            {
                throw new NotImplementedException();
            }
        }

        private record DummyVector(double x, double y) : TP.ConcurrentProgramming.Data.IVector;

        private class DummyBall : TP.ConcurrentProgramming.Data.IBall
        {
            public TP.ConcurrentProgramming.Data.IVector Velocity { get; set; } = new DummyVector(0, 0);
            public event EventHandler<TP.ConcurrentProgramming.Data.IVector>? NewPositionNotification;

            public IVector GetPosition()
            {
                throw new NotImplementedException();
            }

            public void SetPosition(IVector newPosition)
            {
                throw new NotImplementedException();
            }
        }


        private class DataLayerTestFakeWithBall : DataAbstractAPI
        {
            public Ball TestBall = new Ball(new Vector(0, 0), new Vector(1, 0));

            public override void Start(int numberOfBalls, Action<IVector, TP.ConcurrentProgramming.Data.IBall> upperLayerHandler)
            {
                upperLayerHandler(new Vector(0, 0), TestBall);
            }

            public override TP.ConcurrentProgramming.Data.IBall CreateBall(IVector position, IVector velocity) => TestBall;
            public override IVector CreateVector(double x, double y) => new Vector(x, y);
            public override void Dispose() { }

            public override void MoveBall(Data.IBall ball)
            {
                throw new NotImplementedException();
            }

            public override void Stop()
            {
                throw new NotImplementedException();
            }
        }


        #endregion
    }
}