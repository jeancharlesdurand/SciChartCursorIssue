using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SciChart.Charting.ChartModifiers;

namespace SciChart_FIFOScrollingCharts.Modifiers
{
    internal interface ICursor : IChartModifier
    {
        void SetInitialRelativePosition(double position);
        void SetVisible(bool isVisible);
        void SetCusrorStatic(bool isStatic);
    }
}
