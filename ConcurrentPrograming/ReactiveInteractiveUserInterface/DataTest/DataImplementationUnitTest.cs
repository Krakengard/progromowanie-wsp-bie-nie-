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
  public class DataImplementationUnitTest
  {
    [TestMethod]
    public void ConstructorTestMethod()
    {
      using (DataImplementation newInstance = new DataImplementation())
      {
        IEnumerable<IBall>? ballsList = null;
        newInstance.CheckBallsList(x => ballsList = x);
        Assert.IsNotNull(ballsList);
        int numberOfBalls = 0;
        newInstance.CheckNumberOfBalls(x => numberOfBalls = x);
        Assert.AreEqual<int>(0, numberOfBalls);
      }
    }

    [TestMethod]
    public void DisposeTestMethod()
    {
      DataImplementation newInstance = new DataImplementation();
      bool newInstanceDisposed = false;
      newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
      Assert.IsFalse(newInstanceDisposed);
      newInstance.Dispose();
      newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
      Assert.IsTrue(newInstanceDisposed);
      IEnumerable<IBall>? ballsList = null;
      newInstance.CheckBallsList(x => ballsList = x);
      Assert.IsNotNull(ballsList);
      newInstance.CheckNumberOfBalls(x => Assert.AreEqual<int>(0, x));
     }

        [TestMethod]
        public void StartTestMethod()
        {
            using (DataImplementation newInstance = new DataImplementation())
            {
                int numberOfCallbackInvoked = 0;
                int numberOfBallsToCreate = 10;

                newInstance.Start(
                    numberOfBallsToCreate,
                    (startingPosition, ball) =>
                    {
                        numberOfCallbackInvoked++;
                        Assert.IsTrue(startingPosition.x >= 0);
                        Assert.IsTrue(startingPosition.y >= 0);
                        Assert.IsNotNull(ball);
                    });
                Assert.AreEqual(numberOfBallsToCreate, numberOfCallbackInvoked);

                int actualBallCount = -1;
                newInstance.CheckNumberOfBalls(x => actualBallCount = x);
                Assert.AreEqual(numberOfBallsToCreate, actualBallCount);

                newInstance.Stop();
            }
        }


        [TestMethod]
        public void BallShouldBounceOffWallTest()
        {
            var data = new DataImplementation();
            var ball = (Ball)data.CreateBall(new Vector(390, 390), new Vector(15, 15));

            Assert.IsNotNull(ball);
            Assert.IsNotNull(ball.Position);

            // symuluj odbicie od ściany
            ball.UpdatePosition(new Vector(15, 15));  // ruch w prawo i w dół (na granicy)

            var pos = ball.GetPosition();
            Assert.IsTrue(pos.x <= 400 - ball.Diameter);
            Assert.IsTrue(pos.y <= 400 - ball.Diameter);
        }

         [TestMethod]
          public void CreateBallAndVectorTest()
         {
            using var data = new DataImplementation();
            var vector = data.CreateVector(5.5, -3.3);
            Assert.AreEqual(5.5, vector.x);
            Assert.AreEqual(-3.3, vector.y);

            var ball = data.CreateBall(vector, new Vector(1, 0));
            Assert.IsNotNull(ball);
            Assert.AreEqual(vector, ((Ball)ball).Position);
         }


        }
  }