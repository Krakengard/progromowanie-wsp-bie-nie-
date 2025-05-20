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
using TP.ConcurrentProgramming.Data;


namespace TP.ConcurrentProgramming.BusinessLogic
{
  internal class BusinessBall : IBall
  {
        private readonly Data.IBall _dataBall;
        private List<BusinessBall> _otherBalls = new();
        private readonly object _ballsLock = new();
        public BusinessBall(Data.IBall dataBall)
    {
            _dataBall = dataBall;
            _dataBall.NewPositionNotification += RaisePositionChangeEvent;
    }

        public void SetOtherBalls(List<BusinessBall> otherBalls)
        {
            _otherBalls = otherBalls;
        }
        #region IBall

        public event EventHandler<IPosition>? NewPositionNotification;
        public Data.IBall DataBall => _dataBall;
        #endregion IBall

        #region private

        private void RaisePositionChangeEvent(object? sender, Data.IVector e)
        {
            CheckCollisionWithWalls(_dataBall.GetPosition());
            CheckCollisionWithOtherBalls();
            NewPositionNotification?.Invoke(this, new Position(e.x, e.y));
        }

        private void CheckCollisionWithWalls(IVector position)
        {
            double radius = 10;
            double minX = 0 ;
            double maxX = 380 ;
            double minY = 0 ;
            double maxY = 380 ;
            lock (_ballsLock)
            {
                if (position.x <= minX || position.x >= maxX)
                {
                    _dataBall.Velocity = new Vector(-_dataBall.Velocity.x, _dataBall.Velocity.y);
                }

                if (position.y <= minY || position.y >= maxY)
                {
                    _dataBall.Velocity = new Vector(_dataBall.Velocity.x, -_dataBall.Velocity.y);
                }
            }

        }

        private void CheckCollisionWithOtherBalls()
        {
            double radius = 10;
            var posA = _dataBall.GetPosition();

            foreach (var other in _otherBalls)
            {
                if (other == this) continue;

                var posB = other._dataBall.GetPosition();
                double dx = posB.x - posA.x;
                double dy = posB.y - posA.y;
                double distanceSquared = dx * dx + dy * dy;
                double minDist = 2 * radius;

                if (distanceSquared <= minDist * minDist && distanceSquared > 0)
                {
                    lock (_ballsLock)
                    {
                        var velA = _dataBall.Velocity;
                        var velB = other._dataBall.Velocity;

                        // Wektor normalny
                        double nx = dx / Math.Sqrt(distanceSquared);
                        double ny = dy / Math.Sqrt(distanceSquared);

                        // Wektor styczny
                        double tx = -ny;
                        double ty = nx;


                        double vA_n = velA.x * nx + velA.y * ny;
                        double vB_n = velB.x * nx + velB.y * ny;
                        double vA_t = velA.x * tx + velA.y * ty;
                        double vB_t = velB.x * tx + velB.y * ty;

                        // Sprężyste odbicie 
                        double vA_n_after = vB_n;
                        double vB_n_after = vA_n;

                        // Nowe prędkości
                        Vector newVelA = new Vector(
                            vA_n_after * nx + vA_t * tx,
                            vA_n_after * ny + vA_t * ty
                        );

                        Vector newVelB = new Vector(
                            vB_n_after * nx + vB_t * tx,
                            vB_n_after * ny + vB_t * ty
                        );

                        _dataBall.Velocity = newVelA;
                        other._dataBall.Velocity = newVelB;

                        // Drobna korekta pozycji, żeby kule się nie nakładały
                        double overlap = 0.5 * (minDist - Math.Sqrt(distanceSquared) + 0.01);
                        _dataBall.SetPosition(new Vector(posA.x - nx * overlap, posA.y - ny * overlap));
                        other._dataBall.SetPosition(new Vector(posB.x + nx * overlap, posB.y + ny * overlap));
                    }
                }
            }
        }



        #endregion private
    }
}