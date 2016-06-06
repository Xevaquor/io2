using IO2.Messages;
using System;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace IO2.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        string statusText = "Gotowy";
        public string StatusText
        {
            get { return statusText; }
            set
            {
                statusText = value;
                RaisePropertyChanged();
            }
        }

        DispatcherTimer timer = new DispatcherTimer();

        public MainViewModel()
        {
            Messenger.Default.Register<UpdateStatusMessage>(this, OnStatusMessage);
            
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += (s, e) => StatusText = "Gotowy";
            timer.Start();
        }

        void OnStatusMessage(UpdateStatusMessage statusMessage)
        {
            StatusText = statusMessage.StatusText;
            timer.Stop();
            timer.Start();
        }
    }
}
