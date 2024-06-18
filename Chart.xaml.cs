using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SciChart.Data.Model;
using SciChart_RealChart.ChartComponents;

namespace SciChart_RealChart
{
    /// <summary>
    /// Interaction logic for Chart.xaml
    /// </summary>
    public partial class Chart : UserControl
    {
        private bool _isTrackingEnabled;
        private bool _isCursorEnabled;
        private const double dt = 0.005;
        private double _t = dt;
        private readonly Timer _timerNewDataUpdate;
        private SciChartControl _chartControl;
        private SciChartDateTimeAxis _xAxis;
        private DateTime _currentTime = DateTime.Now;
        private List<SciChartChannel> _channels;

        private readonly Color[] _colors = new Color[]
        {
            Colors.Red,
            Colors.Green,
            Colors.Blue
        };

        private readonly double[] _coeff = new double[]
        {
            1.4,
            0.8,
            2.2
        };

        public Chart()
        {
            InitializeComponent();

            _isTrackingEnabled = false;
            _isCursorEnabled = false;

            _chartControl = SciChartFactory.CreateControl(this.content);
            _xAxis = new SciChartDateTimeAxis();

            _chartControl.AddXAxis(_xAxis);

            //this.myXAxisDragModifier.StopTracking += MyXAxisDragModifier_StopTracking;

            CreateDataSetAndSeries();

            _timerNewDataUpdate = new Timer(dt * 1000) { AutoReset = true };
            _timerNewDataUpdate.Elapsed += OnNewData;

        }

        private void CreateDataSetAndSeries()
        {
            _channels = new List<SciChartChannel>();
            for (int i = 0; i < 3; i++)
            {
                string name = string.Format("Serie{0}", i);
                SciChartNumericAxis axis = SciChartFactory.CreateYAxis(name, _colors[i]);
                _channels.Add(new SciChartChannel(name, _colors[i], _xAxis.Id, axis.Id));
                _chartControl.AddChannel(_channels[i], axis);
            }

            //create a channel and an axis per channel
            //add them to the chart
        }

        private void OnNewData(object sender, EventArgs e)
        {
            // Suspending updates is optional, and ensures we only get one redraw
            // once all three dataseries have been appended to
            using (_chartControl.SuspendUpdates())
            {
                for (int i = 0; i < 3; i++)
                {
                    double y = (i+1) * Math.Sin(2 * Math.PI * _coeff[i] * _t * 0.5);
                    this.Dispatcher.Invoke(() =>
                    {
                        _channels[i].Addpoint(_currentTime, y);
                    }); 
                }

                //update x visible range if tracking is on
                if (this._isTrackingEnabled)
                {
                    this.UpdateTimeVisibleRangeToLastTime();
                }
            }

            // Increment current time
            _currentTime += TimeSpan.FromSeconds(1);
            _t += dt;
        }

        private void UpdateTimeVisibleRangeToLastTime()
        {
            this.Dispatcher.Invoke(() =>
            {
                this._xAxis.VisibleRange = new DateRange(_currentTime - TimeSpan.FromHours(0.5), _currentTime);
                //this.myXYCursor.UpdateCursorPositionOnXAxisVisibleRangeChanged();
            });
        }

        private void TrackingButton_Click(object sender, RoutedEventArgs e)
        {
            this._isTrackingEnabled = !this._isTrackingEnabled;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartButton.IsChecked = true;
            PauseButton.IsChecked = false;
            ResetButton.IsChecked = false;

            PauseButton.IsEnabled = true;

            // Start a timer to create new data and append on each tick
            _timerNewDataUpdate.Start();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            _timerNewDataUpdate?.Stop();

            StartButton.IsChecked = false;
            PauseButton.IsChecked = true;
            ResetButton.IsChecked = false;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            PauseButton_Click(this, null);

            StartButton.IsChecked = false;
            PauseButton.IsChecked = false;
            ResetButton.IsChecked = true;
            ChangeTrackingEnableFromCode(false);

            PauseButton.IsEnabled = false;

            ClearDataSeries();
        }

        private void CursorButton_Click(object sender, RoutedEventArgs e)
        {
            this._isCursorEnabled = !this._isCursorEnabled;
            //this._xyCursor.SetVisible(this._isCursorEnabled);
        }

        private void ClearDataSeries()
        {
            using (_chartControl.SuspendUpdates())
            {
                _channels.ForEach(c => c.Clear());
            }
        }

        private void ChangeTrackingEnableFromCode(bool isEnabled)
        {
            this._isTrackingEnabled = isEnabled;
            this.TrackingButton.IsChecked = isEnabled;
        }
    }
}
