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
    internal class Ball : IBall
    {
         #region ctor

         internal Ball(Vector initialPosition, Vector initialVelocity)
         {
           Position = initialPosition;
           Velocity = initialVelocity;
         }

         #endregion ctor

         #region IBall

         public event EventHandler<IVector>? NewPositionNotification;

         public IVector Velocity { get; set; }

             #endregion IBall

             #region private

             internal Vector Position;

             public double Diameter { get; } = 20;
             private void RaiseNewPositionChangeNotification()
             {
           NewPositionNotification?.Invoke(this, Position);
             }

             internal void Move(Vector delta)
             {
                 /*Position = new Vector(Position.x + delta.x, Position.y + delta.y);
                 RaiseNewPositionChangeNotification();*/
                Position = new Vector(
                Math.Clamp(Position.x + delta.x, 0, 400 - Diameter),
                Math.Clamp(Position.y + delta.y, 0, 400 - Diameter)
                );
                RaiseNewPositionChangeNotification();
             }
        public IVector GetPosition() => Position;
        #endregion private
    }
}
