using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using SciChart.Charting.ChartModifiers;
using SciChart.Charting.Visuals;
using SciChart.Charting.Visuals.Annotations;
using SciChart.Charting.Visuals.Axes;
using SciChart.Data.Model;
using SciChart_RealChart;

namespace SciChart_FIFOScrollingCharts.Modifiers
{
    public class MyXYCursor_RelativeX : VerticalSliceModifier, ICursor
    {
        private VerticalLineAnnotation _cursor;
        private bool _isDragging = false;

        public MyXYCursor_RelativeX(IAxis axis)
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
                CoordinateMode = AnnotationCoordinateMode.RelativeX,
            };

            this._cursor.DragDelta += _cursor_DragDelta;
            this._cursor.DragStarted += (s, e) => { _isDragging = true; };
            this._cursor.DragEnded += (s, e) => { _isDragging = false; };
            this._cursor.DragLeave += (s, e) => { _isDragging = false; };

            this.VerticalLines.Add(_cursor);
            this.ShowTooltipOn = ShowTooltipOptions.Always;
            this.ShowAxisLabels = true;
            this.UseInterpolation = true;

            if (axis != null)
            {
                axis.VisibleRangeChanged += XAxis_VisibleRangeChanged;
            }
        }                

        public void SetVisible(bool visible)
        {
            this._cursor.IsHidden = !visible;
            this.UpdateCursorLabel();
        }

        public void SetInitialRelativePosition(double relativePosition)
        {
            this._cursor.X1 = relativePosition;
            this.UpdateCursorLabel();
        }

        private void XAxis_VisibleRangeChanged(object sender, SciChart.Charting.Visuals.Events.VisibleRangeChangedEventArgs e)
        {
            if (!_isDragging)
            {
                UpdateCursorLabel();
            }
        }

        private void _cursor_DragDelta(object sender, SciChart.Charting.Visuals.Events.AnnotationDragDeltaEventArgs e)
        {
            UpdateCursorLabel();
        }

        private void UpdateCursorLabel()
        {
            if (this._cursor.IsHidden ||  this._cursor.X1 == null) 
            {
                return;
            }

            DateRange dateRange = this.XAxis.VisibleRange as DateRange;

            double diffAsdouble = dateRange.Diff.ToOADate();
            double minAsDouble = dateRange.Min.ToOADate();

            double cursorAsDouble = minAsDouble + ((double)this._cursor.X1 * diffAsdouble);
            this._cursor.LabelValue = DateTime.FromOADate(cursorAsDouble);
        }

        Point lastMousePoint = new Point();
        protected override void HandleMasterMouseEvent(Point mousePoint)
        {
            if (this._cursor.IsHidden)
            {
                return;
            }

            if (_isDragging)
            {
                double tolerance = 0.5;
                // Implement mouse events filtering with tolerance on the change of mouse point between two calls
                if (Math.Abs(mousePoint.X - lastMousePoint.X) > tolerance)
                {
                    lastMousePoint = mousePoint;
                    base.HandleMasterMouseEvent(mousePoint);
                }
            }    
            else
            {
                base.HandleMasterMouseEvent(mousePoint);
            }
        }
    }   
}
