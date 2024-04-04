using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Timers;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SciChart.Charting.Model.DataSeries;
using SciChart.Core.Utility;
using SciChart.Data.Model;
using SciChart_FIFOScrollingCharts.Modifiers;

namespace SciChart_RealChart
{
    /// <summary>
    /// Interaction logic for FiftySeries.xaml
    /// </summary>
    public partial class FiftySeries : UserControl
    {
        // Data Sample Rate (sec) - 10 Hz
        private const double dt = 0.1;
        private double _t = dt;

        // Timer to process updates 
        private readonly Timer _timerNewDataUpdate;

        // The current time
        private DateTime _currentTime = DateTime.Now;
        private readonly Random _random = new Random();

        // The dataseries to fill
        private List<IXyDataSeries<DateTime, double>> _seriesList;

        private TimedMethod _startDelegate;

        private volatile bool _isTrackingEnabled;
        private bool _isCursorEnabled;

        private ICursor _xyCursor;

        public FiftySeries()
        {
            InitializeComponent();

            _seriesList = new List<IXyDataSeries<DateTime, double>>();

            _isTrackingEnabled = false;
            this.myXAxisDragModifier.StopTracking += MyXAxisDragModifier_StopTracking;

            _isCursorEnabled = false;

            _timerNewDataUpdate = new Timer(dt * 1000) { AutoReset = true };
            _timerNewDataUpdate.Elapsed += OnNewData;

            CreateDataSetAndSeries();

            SetupXAxis();
            this.InitializeCursor();
        }

        private void InitializeCursor()
        {
            _xyCursor = new MyXYCursor_RelativeX(this.xAxis);
            modifierGroup.ChildModifiers.Add(_xyCursor);
            this._xyCursor.SetInitialRelativePosition(0.5);
        }

        private void MyXAxisDragModifier_StopTracking(object sender, EventArgs e)
        {
            ChangeTrackingEnableFromCode(false);
        }

        private void SetupXAxis()
        {
            TimeSpan halfSpan = TimeSpan.FromHours(0.125);
            DateRange range = new DateRange(_currentTime - halfSpan, _currentTime + halfSpan);
            this.xAxis.VisibleRange = range;
        }

        private void CreateDataSetAndSeries()
        {
            // Create new Dataseries of type X=DateTime, Y=double
            for (int i = 0; i != 50; ++i)
            {
                _seriesList.Add(new XyDataSeries<DateTime, double>() { AcceptsUnsortedData = false });
            }

            // Set the dataseries on the chart's RenderableSeries
            for (int i = 0; i < sciChart.RenderableSeries.Count; ++i)
            {
                sciChart.RenderableSeries[i].DataSeries = _seriesList[i];
            }
        }

        private void ClearDataSeries()
        {
            using (sciChart.SuspendUpdates())
            {
                _seriesList.ForEach(series => series.Clear());
            }
        }

        private void OnNewData(object sender, EventArgs e)
        {
            // Suspending updates is optional, and ensures we only get one redraw
            // once all three dataseries have been appended to
            using (sciChart.SuspendUpdates())
            {
                int baseValue = 10;
                for (int i = 0; i < _seriesList.Count; ++i)
                {
                    double y = baseValue + i;
                    _seriesList[i].Append(_currentTime, y);
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
                this.xAxis.VisibleRange = new DateRange(_currentTime - TimeSpan.FromHours(0.25), _currentTime);
                //this.myXYCursor.UpdateCursorPositionOnXAxisVisibleRangeChanged();
            });
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

        private void OnExampleLoaded(object sender, RoutedEventArgs e)
        {
            ResetButton_Click(this, null);
        }

        private void OnExampleUnloaded(object sender, RoutedEventArgs e)
        {
            if (_startDelegate != null)
            {
                _startDelegate.Dispose();
                _startDelegate = null;
            }

            PauseButton_Click(this, null);
        }

        private void TrackingButton_Click(object sender, RoutedEventArgs e)
        {
            this._isTrackingEnabled = !this._isTrackingEnabled;
        }

        private void ChangeTrackingEnableFromCode(bool isEnabled)
        {
            this._isTrackingEnabled = isEnabled;
            this.TrackingButton.IsChecked = isEnabled;
        }

        private void CursorButton_Click(object sender, RoutedEventArgs e)
        {
            this._isCursorEnabled = !this._isCursorEnabled;
            this._xyCursor.SetVisible(this._isCursorEnabled);
        }
    }
}
