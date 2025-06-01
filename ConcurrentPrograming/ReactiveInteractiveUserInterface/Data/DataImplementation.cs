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
using System.Collections.Concurrent;



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
            _cancellationTokenSource?.Cancel();
            _loggerTask?.Wait();
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

        private CancellationTokenSource _cancellationTokenSource;

        public override void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler)
        {
            Random rand = new Random();
            double spawnWidth = 100;
            double spawnHeight = 100;
            double offsetX = 150;
            double offsetY = 150;

            for (int i = 0; i < numberOfBalls; i++)
            {
                double x = rand.NextDouble() * spawnWidth + offsetX;
                double y = rand.NextDouble() * spawnHeight + offsetY;

                double vx = (rand.NextDouble()) * 5;
                double vy = (rand.NextDouble()) * 5;

                var position = CreateVector(x, y);
                var velocity = CreateVector(vx, vy);
                var ball = CreateBall(position, velocity);

                lock (_ballsLock)
                    BallsList.Add((Ball)ball);

                upperLayerHandler(position, ball);
            }

            _cancellationTokenSource = new CancellationTokenSource();
            // 🔽 Вот здесь добавляется вызов логгера:
            StartLogger();

            // 🔽 Запускаем симуляцию:
            Task.Run(() => RunSimulation(_cancellationTokenSource.Token));
        }



        private async Task RunSimulation(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                lock (_ballsLock)
                {
                    foreach (var ball in BallsList)
                    {
                        MoveBall(ball);
                        _logQueue.Enqueue(ball); // для логгера
                    }
                }
                await Task.Delay(20); // 50 FPS
            }
        }

        private readonly ConcurrentQueue<IBall> _logQueue = new();
        private Task _loggerTask;

        private void StartLogger()
        {
            _loggerTask = Task.Run(() =>
            {
                using StreamWriter writer = new("diagnostics.txt", append: true);
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    while (_logQueue.TryDequeue(out var ball))
                    {
                        var pos = ball.GetPosition();
                        var vel = ball.Velocity;
                        var logEntry = $"Time: {DateTime.UtcNow:HH:mm:ss.fff} | Pos: ({pos.x:F2}, {pos.y:F2}) | Vel: ({vel.x:F2}, {vel.y:F2})";
                        writer.WriteLine(logEntry);
                    }
                    Thread.Sleep(100); // минимизация нагрузки
                }
            });
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