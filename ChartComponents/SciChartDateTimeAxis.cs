using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SciChart.Charting.Visuals.Axes;
using SciChart.Data.Model;

namespace SciChart_RealChart.ChartComponents
{
    public class SciChartDateTimeAxis : DateTimeAxis
    {
        public SciChartDateTimeAxis()
        {
            Id = Guid.NewGuid().ToString();
            MinHeight = 50;
            AutoRange = AutoRange.Never;
            AxisTitle = "Time";
            DrawMinorGridLines = false;
            DrawMinorTicks = false;
            TextFormatting = "dd-MMM-yyyy";
            SubDayTextFormatting = "HH:mm:ss";

            TimeSpan halfSpan = TimeSpan.FromHours(0.25);
            DateRange range = new DateRange(DateTime.Now - halfSpan, DateTime.Now + halfSpan);
            this.VisibleRange = range;
        }
    }
}
