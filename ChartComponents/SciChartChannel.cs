using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SciChart.Charting.Model.DataSeries;
using SciChart.Charting.Visuals.RenderableSeries;

namespace SciChart_RealChart.ChartComponents
{
    public class SciChartChannel : FastLineRenderableSeries
    {
        private IXyDataSeries<DateTime, double> _series;

        public SciChartChannel(string name, System.Windows.Media.Color color, string xAxisId, string yAxisId)
        {
            this.Name = name;
            this.Stroke = color;
            this.StrokeThickness = 1;
            this.XAxisId = xAxisId;
            this.YAxisId = yAxisId;

            _series = new XyDataSeries<DateTime, double>();
            _series.AcceptsUnsortedData = false;
            this.DataSeries = _series;
        }

        public void Clear()
        {
            _series.Clear();
        }

        public void Addpoint(DateTime dateTime, double x)
        {
            _series.Append(dateTime, x);
        }
    }
}
