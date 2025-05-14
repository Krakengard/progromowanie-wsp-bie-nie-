//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.Data.Test
{
  [TestClass]
  public class BallUnitTest
  {
    [TestMethod]
    public void ConstructorTestMethod()
    {
      Vector testinVector = new Vector(0.0, 0.0);
      Ball newInstance = new(testinVector, testinVector);
    }

    [TestMethod]
    public void MoveTestMethod()
    {
      Vector initialPosition = new(10.0, 10.0);
      Ball newInstance = new(initialPosition, new Vector(0.0, 0.0));
      IVector curentPosition = new Vector(0.0, 0.0);
      int numberOfCallBackCalled = 0;
      newInstance.NewPositionNotification += (sender, position) => { Assert.IsNotNull(sender); curentPosition = position; numberOfCallBackCalled++; };
            newInstance.UpdatePosition(new Vector(0.0, 0.0));
            Assert.AreEqual<int>(1, numberOfCallBackCalled);
      Assert.AreEqual<IVector>(initialPosition, curentPosition);
    }
        [TestMethod]
        public void VelocityAndPropertiesTest()
        {
            var position = new Vector(0, 0);
            var velocity = new Vector(2, 3);
            var ball = new Ball(position, velocity);

            Assert.AreEqual(2, ball.Velocity.x);
            Assert.AreEqual(3, ball.Velocity.y);
            Assert.AreEqual(20, ball.Diameter);
            Assert.AreEqual(5.0, ball.Mass);
        }

        [TestMethod]
        public void UpdatePosition_IsThreadSafe()
        {
            var position = new Vector(0, 0);
            var ball = new Ball(position, new Vector(1, 1));

            int updateCount = 380;
            var tasks = new List<Task>();

            for (int i = 0; i < updateCount; i++)
            {
                tasks.Add(Task.Run(() => ball.UpdatePosition(new Vector(1, 0))));
            }

            Task.WaitAll(tasks.ToArray());

            var finalPosition = ball.GetPosition();
            Assert.AreEqual(updateCount, finalPosition.x);
            Assert.AreEqual(0, finalPosition.y); // y się nie zmienia
        }

    }
}