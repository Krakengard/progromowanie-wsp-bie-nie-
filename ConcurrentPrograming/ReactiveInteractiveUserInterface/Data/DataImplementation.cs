//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System;
using System.Diagnostics;


namespace TP.ConcurrentProgramming.Data
{

    internal class DataImplementation : DataAbstractAPI
    {
        #region ctor

        public DataImplementation() { }

        #endregion ctor

        #region DataAbstractAPI
        public override void MoveBall(IBall ball)
        {
            if (ball is Ball concreteBall)
            {
                concreteBall.Move();
            }
        }

        public override void Stop()
        {
            lock (_ballsLock)
            {
                BallsList.Clear();
            }
            _isRunning = false;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    BallsList.Clear();
                    Disposed = true;
                }
            }
            else
                throw new ObjectDisposedException(nameof(DataImplementation));
        }

        public override void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public override void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler)
        {
            Random rand = new Random();
            double margin = 10;
            double width = 400;
            double height = 400;

            for (int i = 0; i < numberOfBalls; i++)
            {
                double x = rand.NextDouble() * (width - 2 * margin) + margin;
                double y = rand.NextDouble() * (height - 2 * margin) + margin;

                double vx = (rand.NextDouble() ) * 5;
                double vy = (rand.NextDouble() ) * 5;

                var position = CreateVector(x, y);
                var velocity = CreateVector(vx, vy);

                var ball = CreateBall(position, velocity);
                lock (_ballsLock)
                {
                    BallsList.Add((Ball)ball);
                }
                upperLayerHandler(position, ball);
            }
        }



        public override IBall CreateBall(IVector position, IVector velocity)
        {
            return new Ball((Vector)position, (Vector)velocity);
        }

        public override IVector CreateVector(double x, double y)
        {
            return new Vector(x, y);
        }


        #endregion DataAbstractAPI

        #region IDisposable
        #endregion IDisposable

        #region private

        private bool Disposed = false;
        private List<Ball> BallsList = [];
        private readonly List<IBall> balls = new();
        private readonly object _ballsLock = new object();
        private bool _isRunning = false;


        #endregion private

        #region TestingInfrastructure

        [Conditional("DEBUG")]
        internal void CheckBallsList(Action<IEnumerable<IBall>> returnBallsList)
        {
            returnBallsList(BallsList);
        }

        [Conditional("DEBUG")]
        internal void CheckNumberOfBalls(Action<int> returnNumberOfBalls)
        {
            returnNumberOfBalls(BallsList.Count);
        }

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
        {
            returnInstanceDisposed(Disposed);
        }

        #endregion TestingInfrastructure
    }
}