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
using System.Collections.Concurrent;////



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
            _loggerTask?.Wait();//czekamy zapisow ligow
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
            // logger
            StartLogger();
            Task.Run(() => RunSimulation(_cancellationTokenSource.Token));
        }

        private Task RunSimulation(CancellationToken token)
        {
            // Rozpoczynanie pomiar czasu od startu symulacji
            _stopwatch = Stopwatch.StartNew();
            _lastTime = _stopwatch.Elapsed;

            // Utworzenie timera z interwałem 10 ms (czyli 100 "klatek" na sekundę)
            _simulationTimer = new System.Timers.Timer(10);

            // Obsługa zdarzenia wykonywanego cyklicznie przez timer
            _simulationTimer.Elapsed += (sender, e) =>
            {
                // Jeśli żądanie anulowania zostało wysłane, zatrzymaj timer
                if (token.IsCancellationRequested)
                {
                    _simulationTimer?.Stop();
                    return;
                }

                // Obliczanie czasu, który upłynął od poprzedniego cyklu (deltaTime w sekundach)
                var now = _stopwatch.Elapsed;
                var deltaTime = (now - _lastTime).TotalSeconds;
                _lastTime = now;

                // Zablokuj dostęp do listy kul, aby zapewnić bezpieczeństwo wątków
                lock (_ballsLock)
                {
                    foreach (var ball in BallsList)
                    {
                        // Pobierz prędkość i przemnóż przez czas, by uzyskać przesunięcie
                        var velocity = ball.Velocity;
                        var delta = new Vector(velocity.x * deltaTime, velocity.y * deltaTime);

                        // Zaktualizuj pozycję kuli
                        ball.UpdatePosition(delta);

                        // Dodaj kulę do kolejki logowania (do późniejszego zapisu w pliku)
                        _logQueue.Enqueue(ball);
                    }
                }
            };

            // Ustawianie automatycznego wywoływania co każde 10 ms i uruchomianie timera
            _simulationTimer.AutoReset = true;
            _simulationTimer.Start();

            // Zwracamy zakończone zadanie (Task.CompletedTask) 
            return Task.CompletedTask;
        }

        private void StartLogger()
        {
            // Tworzymy unikalną nazwę pliku na podstawie bieżącej daty i godziny
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string logFileName = $"logi_{timestamp}.txt";

            // Uruchamiamy logger jako zadanie asynchroniczne
            _loggerTask = Task.Run(() =>
            {
                // Otwieramy plik do zapisu, AutoFlush zapewnia natychmiastowy zapis każdej linii
                using StreamWriter writer = new(logFileName, append: false)
                {
                    AutoFlush = true
                };

                // Zapisujemy nagłówek logu z dokładnym czasem rozpoczęcia
                writer.WriteLine($"------Początek: {DateTime.Now:yyyy-MM-dd HH:mm:ss}------");

                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    // Odczytujemy wszystkie dostępne wpisy z kolejki
                    while (_logQueue.TryDequeue(out var ball))
                    {
                        var pos = ball.GetPosition();     // Pobranie aktualnej pozycji kuli
                        var vel = ball.Velocity;          // Pobranie aktualnej prędkości kuli
                        string time = DateTime.Now.ToString("HH:mm:ss.fff"); // Lokalny czas z milisekundami


                        // Tworzymy wpis logu zawierający czas, ID kuli, pozycję i prędkość
                        var logEntry = $"Czas: {time} | Pozycja: ({pos.x:F2}; {pos.y:F2}) | Pręskość: ({vel.x:F2}; {vel.y:F2})";

                        // Zapisujemy wpis do pliku
                        writer.WriteLine(logEntry);
                    }

                    // Krótkie uśpienie, by nie przeciążać CPU
                    Thread.Sleep(100);
                }

                // Zapisujemy stopkę logu z czasem zakończenia
                writer.WriteLine($"------Koniec: {DateTime.Now:yyyy-MM-dd HH:mm:ss}------");
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
        private System.Timers.Timer _simulationTimer;
        private Stopwatch _stopwatch;
        private TimeSpan _lastTime;
        private readonly ConcurrentQueue<IBall> _logQueue = new();
        private Task _loggerTask;

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