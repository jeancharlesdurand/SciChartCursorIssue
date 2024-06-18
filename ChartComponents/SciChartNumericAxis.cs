using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using SciChart.Charting.Visuals.Axes;
using SciChart.Data.Model;

namespace SciChart_RealChart.ChartComponents
{
    public class SciChartNumericAxis : NumericAxis
    {
        public SciChartNumericAxis(string title, System.Windows.Media.Color color)
        {
            Id = Guid.NewGuid().ToString();
            AxisAlignment = AxisAlignment.Left;
            AutoRange = AutoRange.Never;
            AxisTitle = title;
            DrawMinorGridLines = false;
            DrawMinorTicks = false;
            TextFormatting = "0.00";
            BorderBrush = new SolidColorBrush(color);
            TickTextBrush = new SolidColorBrush(color);
            VisibleRange = new DoubleRange(-5, 5);
        }
    }
}
