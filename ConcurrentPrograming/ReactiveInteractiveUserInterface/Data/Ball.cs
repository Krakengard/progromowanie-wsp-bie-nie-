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
    public class Ball : ISimulationBall
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
        private Vector _position;
        private Vector _velocity;

        #endregion Synchronization

        #region ISimulationBall implementation

        public double Diameter { get; } = 20;

        public Vector Position
        {
            get
            {
                lock (_lock)
                    return _position;
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
                RaiseNewPositionChangeNotification(_position);
            }
        }

        private void RaiseNewPositionChangeNotification(Vector newPos)
        {
            // Poza lockiem – aby uniknąć deadlocków w zewnętrznych handlerach
            NewPositionNotification?.Invoke(this, newPos);
        }

        public IVector GetPosition()
        {
            lock (_lock)
                return _position;
        }



        #endregion
    }
    public interface ISimulationBall : IBall
    {
        double Mass { get; }
        double Diameter { get; }
        Vector Position { get; }
        void UpdatePosition(Vector delta);
    }

}
