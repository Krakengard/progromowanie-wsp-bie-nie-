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
using System.Numerics;
using System.Threading;
using TP.ConcurrentProgramming.Data;
using DataBall = TP.ConcurrentProgramming.Data.Ball;
using DataVector = TP.ConcurrentProgramming.Data.Vector;

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class BusinessLogicImplementation : BusinessLogicAbstractAPI
    {
        #region ctor

        public BusinessLogicImplementation()
        {
            InitializeSimulation();
        }

        private void InitializeSimulation()
        {
            random = new Random();
            balls = new List<DataBall>();
        }

        #endregion ctor

        

        #region BusinessLogicAbstractAPI

        public override void Dispose()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            simulationTimer?.Dispose();
            balls.Clear();
            Disposed = true;
        }

        public override void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));

            for (int i = 0; i < numberOfBalls; i++)
            {
                double x = random.Next(100, (int)(TableWidth - 100));
                double y = random.Next(100, (int)(TableHeight - 100));
                double angle = random.NextDouble() * 2 * Math.PI;
                DataVector velocity = new(Math.Cos(angle) * InitialSpeed, Math.Sin(angle) * InitialSpeed);
                DataVector position = new(x, y);

                DataBall ball = new(position, velocity);
                balls.Add(ball);
                upperLayerHandler(new Position(position.x, position.y), new BusinessBall(ball));
            }

            simulationTimer = new Timer(StepSimulation, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(16));
        }

        #endregion BusinessLogicAbstractAPI

        #region Simulation

        private void StepSimulation(object? state)
        {
            lock (balls)
            {
                for (int i = 0; i < balls.Count; i++)
                {
                    DataBall a = balls[i];
                    for (int j = i + 1; j < balls.Count; j++)
                    {
                        DataBall b = balls[j];
                        DataVector delta = new(b.Position.x - a.Position.x, b.Position.y - a.Position.y);
                        double distance = Math.Sqrt(delta.x * delta.x + delta.y * delta.y);
                        double minDist = (a.Diameter + b.Diameter) / 2.0;

                        if (distance < minDist && distance > 0.0)
                            ResolveCollision(a, b, delta, distance);
                    }
                }

                foreach (DataBall ball in balls)
                {
                    DataVector newVelocity = new(ball.Velocity.x * Friction, ball.Velocity.y * Friction);

                    if (Math.Abs(newVelocity.x) < MinVelocity && Math.Abs(newVelocity.y) < MinVelocity)
                        newVelocity = new DataVector(0, 0);

                    double newX = ball.Position.x + newVelocity.x;
                    double newY = ball.Position.y + newVelocity.y;

                    if (newX <= 0 || newX >= TableWidth - ball.Diameter)
                    {
                        newVelocity = new DataVector(-newVelocity.x, newVelocity.y);
                        newX = Math.Clamp(newX, 0, TableWidth - ball.Diameter);
                    }

                    if (newY <= 0 || newY >= TableHeight - ball.Diameter)
                    {
                        newVelocity = new DataVector(newVelocity.x, -newVelocity.y);
                        newY = Math.Clamp(newY, 0, TableHeight - ball.Diameter);
                    }

                    ball.Velocity = newVelocity;
                    ball.UpdatePosition(new DataVector(newX - ball.Position.x, newY - ball.Position.y));
                }
            }
        }

        private void ResolveCollision(DataBall a, DataBall b, DataVector delta, double distance)
        {
            double nx = delta.x / distance;
            double ny = delta.y / distance;
            double tx = -ny;
            double ty = nx;

            double vA_n = a.Velocity.x * nx + a.Velocity.y * ny;
            double vB_n = b.Velocity.x * nx + b.Velocity.y * ny;
            double vA_t = a.Velocity.x * tx + a.Velocity.y * ty;
            double vB_t = b.Velocity.x * tx + b.Velocity.y * ty;

            double m1 = a.Mass;
            double m2 = b.Mass;

            double vA_n_prime = (vA_n * (m1 - m2) + 2 * m2 * vB_n) / (m1 + m2);
            double vB_n_prime = (vB_n * (m2 - m1) + 2 * m1 * vA_n) / (m1 + m2);

            a.Velocity = new DataVector(vA_n_prime * nx + vA_t * tx, vA_n_prime * ny + vA_t * ty);
            b.Velocity = new DataVector(vB_n_prime * nx + vB_t * tx, vB_n_prime * ny + vB_t * ty);
        }

        #endregion Simulation

        #region private

        private readonly double TableWidth = 400;
        private readonly double TableHeight = 400;
        private readonly double Friction = 0.99;
        private readonly double MinVelocity = 0.1;
        private readonly double InitialSpeed = 30.0;

        private Timer? simulationTimer;
        private bool Disposed = false;
        private List<DataBall> balls = new();
        private Random random = new();

        #endregion private

        #region TestingInfrastructure

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
        {
            returnInstanceDisposed(Disposed);
        }

        #endregion TestingInfrastructure
    }
}