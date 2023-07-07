using Prism.Events;
using System.Linq;
using System.Windows;

namespace ScottPlotMvvm.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(IEventAggregator _eventAggregator)
        {
            InitializeComponent();

            this.LineGraph.Plot.XAxis.DateTimeFormat(true);

            _eventAggregator.GetEvent<PubSubEvent<ViewModels.MainWindowViewModelUpdateChart>>().Subscribe(this.UpdateChart);
        }

        private void UpdateChart(ViewModels.MainWindowViewModelUpdateChart _parameter)
        {
            this.LineGraph.Plot.Clear();
            if (_parameter.Xs.Any() && _parameter.Ys.Any())
            {
                this.LineGraph.Plot.AddSignalXY(_parameter.Xs, _parameter.Ys);
            }
            this.LineGraph.Render();
        }
    }
}
