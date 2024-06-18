using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using SciChart.Charting.ChartModifiers;
using SciChart.Charting.Visuals;
using SciChart_FIFOScrollingCharts.Modifiers;

namespace SciChart_RealChart.ChartComponents
{
    public class SciChartControl : SciChartSurface
    {
        private ModifierGroup _chartModifiers;

        public SciChartControl()
        {
            Background = new SolidColorBrush(Colors.Black);
        }

        internal void AddXAxis(SciChartDateTimeAxis xAxis)
        {
            if (xAxis != null && !this.XAxes.Contains(xAxis))
            {
                this.XAxes.Add(xAxis);  
                this.InitializeModifiers();
            }
        }

        internal void AddChannel(SciChartChannel channel, SciChartNumericAxis yAxis)
        {
            if (yAxis != null && !this.YAxes.Contains(yAxis))
            {
                this.YAxes.Add(yAxis);
            }

            if (channel != null && !this.RenderableSeries.Contains(channel))
            {
                this.RenderableSeries.Add(channel);
            }
        }

        private void InitializeModifiers()
        {
            _chartModifiers = new ModifierGroup();
            this.ChartModifier = _chartModifiers;
            
            _chartModifiers.ChildModifiers.Add(new MyXAxisDragModifier(this.XAxes.First().Id));
        }
    }
}
