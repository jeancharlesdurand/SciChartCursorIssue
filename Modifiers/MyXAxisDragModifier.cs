using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SciChart.Charting.ChartModifiers;
using SciChart.Core.Utility.Mouse;

namespace SciChart_FIFOScrollingCharts.Modifiers
{
    public class MyXAxisDragModifier : XAxisDragModifier
    {
        public event EventHandler StopTracking;

        public MyXAxisDragModifier()
        {
            this.ClipModeX = SciChart.Charting.ClipMode.None;
            this.DragMode = SciChart.Charting.AxisDragModes.Pan;
        }

        public override void OnModifierMouseDown(ModifierMouseArgs e)
        {
            if (e.MouseButtons != MouseButtons.Left)
            {
                return;
            }

            var xAxis = GetXAxis(this.AxisId);
            bool isOnAxis = IsPointWithinBounds(e.MousePoint, xAxis);
            if (isOnAxis)
            {
                this.StopTracking?.Invoke(this, new EventArgs());
            }
            
            base.OnModifierMouseDown(e);
        }
    }
}
