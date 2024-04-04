// *************************************************************************************
// SCICHART® Copyright SciChart Ltd. 2011-2023. All rights reserved.
//  
// Web: http://www.scichart.com
//   Support: support@scichart.com
//   Sales:   sales@scichart.com
// 
// RealtimeFifoChartView.xaml.cs is part of the SCICHART® Examples. Permission is hereby granted
// to modify, create derivative works, distribute and publish any part of this source
// code whether for commercial, private or personal use. 
// 
// The SCICHART® examples are distributed in the hope that they will be useful, but
// without any warranty. It is provided "AS IS" without warranty of any kind, either
// expressed or implied. 
// *************************************************************************************
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using SciChart.Charting.Model.DataSeries;
using SciChart.Charting.Visuals.Axes;
using SciChart.Charting.Visuals.RenderableSeries;
using SciChart.Core.Utility;
using SciChart.Data.Model;
using SciChart_FIFOScrollingCharts;
using SciChart_FIFOScrollingCharts.Modifiers;

namespace SciChart.Examples.Examples.CreateRealtimeChart
{
    public partial class LotsOfSeriesChartView : UserControl
    {
        // Data Sample Rate (sec) - 20 Hz
        private const double dt = 0.05;
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
        private bool _isStaticCursor;

        private ICursor _xyCursor;

        public LotsOfSeriesChartView()
        {
            InitializeComponent();

            _seriesList = new List<IXyDataSeries<DateTime, double>>();

            _isTrackingEnabled = false;
            this.myXAxisDragModifier.StopTracking += MyXAxisDragModifier_StopTracking;

            _isCursorEnabled = false;
            _isStaticCursor = false;

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
            for (int i = 0; i != 30; ++i)
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
            double y1 = 3.0 * Math.Sin(2 * Math.PI * _t * 0.5);
            double y2 = 2.0 * Math.Cos(2 * Math.PI * _t * 0.5);
            double y3 = 1.0 * Math.Sin(2 * Math.PI * _t * 0.5);

            // Suspending updates is optional, and ensures we only get one redraw
            // once all three dataseries have been appended to
            using (sciChart.SuspendUpdates())
            {
                for(int i = 0; i < _seriesList.Count; ++i)
                {
                    double y;
                    if (i % 2 == 0)
                    {
                        y = i * Math.Sin(2 * Math.PI * _t * 0.5);
                    }
                    else
                    {
                        y = i * Math.Cos(2 * Math.PI * _t * 0.5);
                    }


                    _seriesList[i].Append(_currentTime, y);
                }

                // Append x,y data to previously created series
                //_seriesList[0].Append(_currentTime, y1);
                //_seriesList[1].Append(_currentTime, y2);
                //_seriesList[2].Append(_currentTime, y3);

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
            this.StaticCursorButton.IsEnabled = !_isCursorEnabled;

            if (!this._isStaticCursor)
            {
                this._xyCursor.SetInitialRelativePosition(0.5);
            }

            this._xyCursor.SetVisible(this._isCursorEnabled);
        }

        private void StaticCursorButton_Click(object sender, RoutedEventArgs e)
        {
            this._isStaticCursor = !this._isStaticCursor;
            this._xyCursor.SetCusrorStatic(this._isStaticCursor);
        }        
    }
}