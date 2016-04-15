using IO2.Messages;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Threading;

namespace IO2.ViewModel
{
    public class MainViewModel : ReactiveObject
    {
        string statusText = "Gotowy";
        public string StatusText
        {
            get { return statusText; }
            set
            {
                this.RaiseAndSetIfChanged(ref statusText, value);
            }
        }

        DispatcherTimer timer = new DispatcherTimer();

        public MainViewModel()
        {
            MessageBus.Current.Listen<Messages.UpdateStatusMessage>()
                .Subscribe(OnStatusMessage);

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
