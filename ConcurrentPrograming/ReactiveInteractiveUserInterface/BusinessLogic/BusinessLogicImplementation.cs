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
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class BusinessLogicImplementation : BusinessLogicAbstractAPI
    {
        #region ctor

        public BusinessLogicImplementation(DataAbstractAPI? data = null)
        {
            this.dataLayer = data ?? DataAbstractAPI.GetDataLayer();
        }

        #endregion ctor

        #region BusinessLogicAbstractAPI

        public override void Dispose()
        {
            Stop();
            _cts.Dispose();
        }

        public override void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler)
        {
            if (_isRunning) return;
            _isRunning = true;

            balls.Clear();
            _threads.Clear();

            dataLayer.Start(numberOfBalls, (vector, rawBall) =>
            {
                var logicBall = new BusinessBall(rawBall);
                balls.Add(logicBall);
               
                if (balls.All(b => b is BusinessBall))
                {
                    var logicBalls = balls.Cast<BusinessBall>().ToList();
                    foreach (var ball in logicBalls)
                        ball.SetOtherBalls(logicBalls);
                }


                upperLayerHandler(new Position(vector.x, vector.y), logicBall);

                var thread = new Thread(() => BallLoop(rawBall, _cts.Token));
                _threads.Add(thread);
                thread.Start();
            });
        }

        public override void Stop()
        {
            _cts.Cancel();
            foreach (var t in _threads)
                t.Join();
            _threads.Clear();

            balls.Clear();
            dataLayer.Dispose();

            _cts = new CancellationTokenSource();
            _isRunning = false;
        }


        #endregion BusinessLogicAbstractAPI

        #region Simulation

        private void BallLoop(Data.IBall ball, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                dataLayer.MoveBall(ball);
                Thread.Sleep(16); 
            }
        }

        #endregion Simulation

        #region private

        private readonly DataAbstractAPI dataLayer;
        private readonly List<IBall> balls = new();
        private List<Thread> _threads = new();
        private CancellationTokenSource _cts = new();
        private bool _isRunning = false;

        #endregion private

        #region TestingInfrastructure

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
        {
            returnInstanceDisposed(_isRunning == false);
        }

        [Conditional("DEBUG")]
        internal void CheckBallCount(Action<int> returnBallCount)
        {
            returnBallCount(balls.Count);
        }


        #endregion TestingInfrastructure


    }

}