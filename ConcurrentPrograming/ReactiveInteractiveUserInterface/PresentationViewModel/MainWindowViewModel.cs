//__________________________________________________________________________________________
//
//  Copyright 2024 Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and to get started
//  comment using the discussion panel at
//  https://github.com/mpostol/TP/discussions/182
//__________________________________________________________________________________________

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

using TP.ConcurrentProgramming.Presentation.Model;
using TP.ConcurrentProgramming.Presentation.ViewModel.MVVMLight;
using static System.Net.Mime.MediaTypeNames;
using ModelIBall = TP.ConcurrentProgramming.Presentation.Model.IBall;

namespace TP.ConcurrentProgramming.Presentation.ViewModel
{
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        #region ctor
         public RelayCommand StartCommand { get; }
         public RelayCommand StopCommand { get; }
        public RelayCommand IncreaseBallsCommand { get; }
        public RelayCommand DecreaseBallsCommand { get; }

        private int _ballCount;
         private const int MaxBalls = 10;
         private const int MinBalls = 0;
         private IDisposable Observer = null;
         private ModelAbstractApi ModelLayer;
         private bool Disposed = false;

         public event PropertyChangedEventHandler PropertyChanged;

         protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
         {
             PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
         }
         private bool _isRunning = false;
         public bool IsRunning
         {
             get => _isRunning;
             private set
             {
                 if (_isRunning != value)
                 {
                     _isRunning = value;
                     OnPropertyChanged();
                     // aktualizuemy przycisk
                     StartCommand.RaiseCanExecuteChanged();
                     StopCommand.RaiseCanExecuteChanged();
                 }
             }
         }


         public int BallCount
         {
             get => _ballCount;
            set
            {
                if (value < MinBalls || value > MaxBalls)
                    return;

                _ballCount = value;
                RaisePropertyChanged();
                IncreaseBallsCommand.RaiseCanExecuteChanged();
                DecreaseBallsCommand.RaiseCanExecuteChanged();
            }
        }

         public MainWindowViewModel() : this(null)
         {
             StartCommand = new RelayCommand(() => Start(BallCount), () => !IsRunning);
             StopCommand = new RelayCommand(() => Stop(), () => IsRunning);

         }

         internal MainWindowViewModel(ModelAbstractApi modelLayerAPI)
         {
              ModelLayer = modelLayerAPI == null ? ModelAbstractApi.CreateModel() : modelLayerAPI;
              Observer = ModelLayer.Subscribe<ModelIBall>(x => Balls.Add(x));

            StartCommand = new RelayCommand(() => Start(BallCount), () => !IsRunning);
            StopCommand = new RelayCommand(() => Stop(), () => IsRunning);
            IncreaseBallsCommand = new RelayCommand(() => BallCount++, () => BallCount < MaxBalls);
            DecreaseBallsCommand = new RelayCommand(() => BallCount--, () => BallCount > MinBalls);

        }

        #endregion ctor

        #region public API

        public void Start(int _ballCount)
         {
             if (Disposed)
             {
                 ModelLayer = ModelAbstractApi.CreateModel();
                 Disposed = false;
             }

             Balls.Clear();
             ModelLayer.Start(_ballCount);
             Observer = ModelLayer.Subscribe<ModelIBall>(x => Balls.Add(x));
             IsRunning = true; // dla StopCommand
         }

         public void Stop()
         {
             if (ModelLayer != null)
             {
                 ModelLayer.Dispose();
                 ModelLayer = null; 
             }
             Balls.Clear(); 
             Observer?.Dispose(); 
             IsRunning = false;
             Disposed = true;
         }
         public ObservableCollection<ModelIBall> Balls { get; } = new ObservableCollection<ModelIBall>();




        #endregion public API

        #region IDisposable

        protected virtual void Dispose(bool disposing)
         {
             if (!Disposed)
         {
                  if (disposing)
                  {
                     Balls.Clear();
                     Observer.Dispose();
                     ModelLayer.Dispose();
                  }

         // TODO: free unmanaged resources (unmanaged objects) and override finalizer
         // TODO: set large fields to null
         Disposed = true;
             }
         }

         public void Dispose()
         {
         if (Disposed)
             throw new ObjectDisposedException(nameof(MainWindowViewModel));
         Dispose(disposing: true);
         GC.SuppressFinalize(this);
         }

     #endregion IDisposable
    }
}