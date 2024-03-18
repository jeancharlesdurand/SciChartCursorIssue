using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using SciChart.Charting.ChartModifiers;
using SciChart.Charting.Visuals.Annotations;

namespace SciChart_FIFOScrollingCharts.Modifiers
{
    public class MyXYCursor_RelativeX : VerticalSliceModifier
    {
        private VerticalLineAnnotation _cursor;

        public MyXYCursor_RelativeX()
        {
            SolidColorBrush colorBrush = new SolidColorBrush(Colors.Red);
            SolidColorBrush foreColorBrush = new SolidColorBrush(Colors.White);
            this._cursor = new VerticalLineAnnotation()
            {
                IsEditable = true,
                IsHidden = true,
                LabelPlacement = LabelPlacement.Axis,
                ShowLabel = true,
                FontSize = 12,
                Stroke = colorBrush,
                Foreground = foreColorBrush,
                StrokeThickness = 2.0,
                CoordinateMode = AnnotationCoordinateMode.RelativeX
            };

            this.VerticalLines.Add(_cursor);
            this.ShowTooltipOn = ShowTooltipOptions.Always;
            this.ShowAxisLabels = true;
            this.UseInterpolation = true;
        }

        public void SetVisible(bool visible)
        {
            this._cursor.IsHidden = !visible;
        }

        public void SetInitialRelativePosition(double relativePosition)
        {
            this._cursor.X1 = relativePosition;
        }
    }   
}
