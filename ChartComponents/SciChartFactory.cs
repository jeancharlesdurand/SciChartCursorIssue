using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using SciChart.Charting;

namespace SciChart_RealChart.ChartComponents
{
    public class SciChartFactory
    {
        public static SciChartDateTimeAxis CreateXAxis()
        {
            return new SciChartDateTimeAxis();
        }

        public static SciChartNumericAxis CreateYAxis(string title, System.Windows.Media.Color color)
        {
            return new SciChartNumericAxis(title, color); 
        }

        public static SciChartChannel CreateChannel(string name, System.Windows.Media.Color color, string xAxisId, string yAxisId)
        {
            return new SciChartChannel(name, color, xAxisId, yAxisId);
        }

        public static SciChartControl CreateControl(Panel panel)
        {
            SciChartControl control = new SciChartControl();
            panel.Children.Add(control);
            ThemeManager.SetTheme(control, "Chrome");

            return control;
        }
    }
}
