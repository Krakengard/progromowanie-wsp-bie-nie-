//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.Data
{
    public class Ball : IBall
    {
        #region ctor

        public double Mass { get; } = 5.0;

        public Ball(Vector initialPosition, Vector initialVelocity)
        {
            _position = initialPosition;
            _velocity = initialVelocity;
        }

        #endregion ctor

        #region IBall

        public event EventHandler<IVector>? NewPositionNotification;

        public IVector Velocity
        {
            get
            {
                lock (_lock)
                    return _velocity;
            }
            set
            {
                lock (_lock)
                    _velocity = (Vector)value;
            }
        }

        #endregion IBall

        #region Synchronization

        private readonly object _lock = new();
        private IVector _position;
        private Vector _velocity;

        #endregion Synchronization

        #region ISimulationBall implementation

        public double Diameter { get; } = 20;

        public Vector Position
        {
            get
            {
                lock (_lock)
                    return (Vector)_position;
            }
        }

        public void UpdatePosition(Vector delta)
        {
            lock (_lock)
            {
                _position = new Vector(
                    Math.Clamp(_position.x + delta.x, 0, 400 - Diameter),
                    Math.Clamp(_position.y + delta.y, 0, 400 - Diameter)
                );
                RaiseNewPositionChangeNotification((Vector)_position);
            }
        }

        private void RaiseNewPositionChangeNotification(Vector newPos)
        {

            NewPositionNotification?.Invoke(this, newPos);
        }


        public void Move()
        {
            double radius = 10;
            double maxX = 390;
            double maxY = 390;
            double minX = 0;
            double minY = 0;

            double newX = _position.x + Velocity.x;
            double newY = _position.y + Velocity.y;

            double vx = Velocity.x;
            double vy = Velocity.y;


            if (newX <= minX || newX >= maxX)
                vx = -vx;
            if (newY <= minY || newY >= maxY)
                vy = -vy;

            Velocity = new Vector(vx, vy);

            _position = new Vector(
                Math.Clamp(_position.x + vx, minX, maxX),
                Math.Clamp(_position.y + vy, minY, maxY)
            );

            NewPositionNotification?.Invoke(this, _position);
        }

        public IVector GetPosition()
        {
            lock (_lock)
            {
                return _position;
            }
        }


        public void SetPosition(IVector newPosition)
        {
            lock (_lock)
                _position = newPosition;
        }




        #endregion
    }


}