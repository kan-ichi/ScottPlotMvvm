using Prism.Events;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Windows.Threading;

namespace ScottPlotMvvm.ViewModels
{
    public class MainWindowViewModel : BindableBase, IDisposable
    {
        private const int INTERVAL_MILLISECONDS = 200;

        public ReactiveCommand PauseClick { get; private set; }
        public ReactiveCommand ResumeClick { get; private set; }
        public ReactiveCommand ClearClick { get; private set; }

        private DispatcherTimer UpdateChartTimer;

        private IEventAggregator EventAggregator;

        private CompositeDisposable Disposables = new CompositeDisposable();

        void IDisposable.Dispose() { this.Disposables.Dispose(); }

        public MainWindowViewModel(IEventAggregator _eventAggregator)
        {
            this.EventAggregator = _eventAggregator;

            this.UpdateChartTimer = new DispatcherTimer(DispatcherPriority.Normal);
            this.UpdateChartTimer.Tick += new EventHandler(this.UpdateChartTimer_Tick);
            this.UpdateChartTimer.Interval = TimeSpan.FromMilliseconds(INTERVAL_MILLISECONDS);
            this.UpdateChartTimer.Start();

            this.PauseClick = new ReactiveCommand().AddTo(this.Disposables).WithSubscribe(() => this.UpdateChartTimer?.Stop());

            this.ResumeClick = new ReactiveCommand().AddTo(this.Disposables).WithSubscribe(() => this.UpdateChartTimer?.Start());

            this.ClearClick = new ReactiveCommand().AddTo(this.Disposables).WithSubscribe(() =>
            {
                this.UpdateChartTimer?.Stop();
                this.EventAggregator.GetEvent<PubSubEvent<MainWindowViewModelUpdateChart>>().Publish(new MainWindowViewModelUpdateChart());
            });
        }

        private void UpdateChartTimer_Tick(object _sender, EventArgs _e)
        {
            var now = DateTime.Now;
            var xs = new List<double>();
            var ys = new List<double>();

            for (int i = 100; i > 0; i--)
            {
                var plotDateTime = now.AddMilliseconds(-i * 100);
                xs.Add(plotDateTime.ToOADate());
                var degree = (1.0 * plotDateTime.TimeOfDay.Ticks / TimeSpan.TicksPerDay) * (INTERVAL_MILLISECONDS * 100) * 360;
                ys.Add(Math.Sin(degree * (Math.PI / 180)));
            }

            this.EventAggregator.GetEvent<PubSubEvent<MainWindowViewModelUpdateChart>>().Publish(new MainWindowViewModelUpdateChart { Xs = xs.ToArray(), Ys = ys.ToArray() });
        }

    }
}
