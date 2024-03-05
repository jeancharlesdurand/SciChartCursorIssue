using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SciChart.Charting;
using SciChart.Charting.ChartModifiers;
using SciChart.Core.Utility.Mouse;

namespace SciChart_FIFOScrollingCharts.Modifiers
{
    public class MyXAxisDragModifier : XAxisDragModifier
    {
        public event EventHandler StopTracking;

        public MyXAxisDragModifier()
        {
            this.ClipModeX = ClipMode.None;
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
                if (e.Modifier == MouseModifier.Ctrl)
                {
                    this.DragMode = AxisDragModes.Scale;
                }
                else
                {
                    this.DragMode = AxisDragModes.Pan;
                }
            }
            
            base.OnModifierMouseDown(e);
        }

        public override void OnModifierMouseWheel(ModifierMouseArgs e)
        {
            base.OnModifierMouseWheel(e);

            var xAxis = GetXAxis(this.AxisId);
            bool isOnAxis = IsPointWithinBounds(e.MousePoint, xAxis);
            if (isOnAxis)
            {
                this.StopTracking?.Invoke(this, new EventArgs());


                if (e.Modifier == MouseModifier.Ctrl)
                {
                    double zoomFactor = 0.1 * e.Delta / 120d;
                    xAxis.ZoomBy(-zoomFactor, -zoomFactor);
                }
                else
                {
                    double numPixels = 0.1 * e.Delta;
                    xAxis.Scroll(-numPixels, ClipMode.None);
                }
            }
        }
    }
}
