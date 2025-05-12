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
         private const double TableWidth = 400;
         private const double TableHeight = 400;
         private const double Friction = 0.99;//tarcie
         private const double MinVelocity = 0.1;//minimalna prędkość
         public DataImplementation()
         {
             MoveTimer = new Timer(Move, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(16));
         }

         #endregion ctor

         #region DataAbstractAPI
         protected virtual void Dispose(bool disposing)
         {
             if (!Disposed)
             {
                 if (disposing)
                 {
                     MoveTimer?.Dispose();
                     BallsList.Clear(); // usuwamy kule
                     Disposed = true;
                 }

             }
             else
                 throw new ObjectDisposedException(nameof(DataImplementation));
         }

         public override void Dispose()
         {
             // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
             Dispose(disposing: true);
             GC.SuppressFinalize(this);
         }
         public override void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler)
         {
              if (Disposed)
                  throw new ObjectDisposedException(nameof(DataImplementation));
              if (upperLayerHandler == null)
                  throw new ArgumentNullException(nameof(upperLayerHandler));
            Random random = new Random();
            //Prędkość początkowa
            const double InitialSpeed = 30.0;
            for (int i = 0; i < numberOfBalls; i++)
              {
                  Vector startingPosition = new(random.Next(100, 400 - 100), random.Next(100, 400 - 100));
                //Losowy kąt od 0 do 2π
                double angle = random.NextDouble() * 2 * Math.PI;

                // Obliczenie składowych wektora prędkości
                Vector initialVelocity = new(Math.Cos(angle) * InitialSpeed,Math.Sin(angle) * InitialSpeed);
                Ball newBall = new(startingPosition, initialVelocity);

                upperLayerHandler(startingPosition, newBall);
                  BallsList.Add(newBall);
              }
         }

         #endregion DataAbstractAPI

         #region IDisposable

         #endregion IDisposable

         #region private

         //private bool disposedValue;
         private bool Disposed = false;

         private readonly Timer MoveTimer;
         private Random RandomGenerator = new();
         private List<Ball> BallsList = [];

         private void Move(object? x)
         {
            lock (BallsList) 
            {
                for (int i = 0; i < BallsList.Count; i++)
                {
                    Ball ballA = BallsList[i];
                    for (int j = i + 1; j < BallsList.Count; j++)
                    {
                        Ball ballB = BallsList[j];

                        Vector delta = new Vector(ballB.Position.x - ballA.Position.x, ballB.Position.y - ballA.Position.y);
                        double distance = Math.Sqrt(delta.x * delta.x + delta.y * delta.y);
                        double minDist = (ballA.Diameter + ballB.Diameter) / 2.0;

                        if (distance < minDist && distance > 0.0)
                        {
                            
                            ResolveCollision(ballA, ballB, delta, distance);
                        }
                    }
                }
            }

            foreach (Ball item in BallsList)
             {
                 // stopniowe zatrzymanie
                 var newVelocity = new Vector(
                     item.Velocity.x * Friction,
                     item.Velocity.y * Friction);

                 // minimalna szybkosc
                 if (Math.Abs(newVelocity.x) < MinVelocity && Math.Abs(newVelocity.y) < MinVelocity)
                 {
                     newVelocity = new Vector(0, 0);
                 }

                 // pozycja uwzględniająca szybkość
                 double newX = item.Position.x + newVelocity.x;
                 double newY = item.Position.y + newVelocity.y;

                 // granicн
                 if (newX <= 0 || newX >= TableWidth - item.Diameter)
                 {
                     newVelocity = new Vector(-newVelocity.x, newVelocity.y);
                     newX = Math.Clamp(newX, 0, TableWidth - item.Diameter);
                 }
                 if (newY <= 0 || newY >= TableHeight - item.Diameter)
                 {
                     newVelocity = new Vector(newVelocity.x, -newVelocity.y);
                     newY = Math.Clamp(newY, 0, TableHeight - item.Diameter);
                 }

                 item.Velocity = newVelocity;
                 item.Move(new Vector(newX - item.Position.x, newY - item.Position.y));
             }

         }
        private void ResolveCollision(Ball a, Ball b, Vector delta, double distance)
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

            // Now new velocities
            a.Velocity = new Vector(vA_n_prime * nx + vA_t * tx, vA_n_prime * ny + vA_t * ty);
            b.Velocity = new Vector(vB_n_prime * nx + vB_t * tx, vB_n_prime * ny + vB_t * ty);
        }


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
    } }