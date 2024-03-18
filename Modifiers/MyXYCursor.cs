using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using SciChart.Charting.ChartModifiers;
using SciChart.Charting.Visuals.Annotations;
using SciChart.Charting.Visuals.Axes;
using SciChart.Core.Utility.Mouse;
using SciChart.Data.Model;

namespace SciChart_FIFOScrollingCharts.Modifiers
{
    public class MyXYCursor : VerticalSliceModifier
    {
        private VerticalLineAnnotation _cursor;
        private double? _pixelCoordX = null;
        private double? _lastRelativePosition;
        private bool _relativePositionNeedToBeSet;

        public string CursorName;

        public MyXYCursor()
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
                StrokeThickness = 2.0
            };

            this._cursor.DragDelta += Cursor_DragDelta;
            this.VerticalLines.Add(_cursor);
            this.ShowTooltipOn = ShowTooltipOptions.Always;
            this.ShowAxisLabels = true;
            this.UseInterpolation = true;
        }

        public void InitilaizeAxisId(string xAxisId, string yAxisId)
        {
            this._cursor.XAxisId = xAxisId;
            this._cursor.YAxisId = yAxisId;
        }

        public void SetVisible(bool visible)
        {
            this._cursor.IsHidden = !visible;
        }

        public void SetInitialRelativePosition(double relativePosition)
        {
            this.SetRelativePosition(relativePosition);
        }

        public void UpdateCursorPositionOnXAxisVisibleRangeChanged()
        {
            if (this._cursor.IsHidden)
            {
                return;
            }

            if (!this._cursor.IsHidden && !_pixelCoordX.HasValue && this._lastRelativePosition.HasValue)
            {
                this.SetRelativePosition(this._lastRelativePosition.Value);
            }

            if (!this._cursor.IsHidden && this._cursor.X1 != null)
            {
                if (!_pixelCoordX.HasValue || double.IsNaN(_pixelCoordX.Value))
                {
                    _pixelCoordX = this.GetCursorPixelPosition();
                }

                if (double.IsNaN(_pixelCoordX.Value))
                {
                    if (this._lastRelativePosition.HasValue && this._relativePositionNeedToBeSet)
                    {
                        this.SetRelativePosition(this._lastRelativePosition.Value);

                        this._relativePositionNeedToBeSet = false;
                        _pixelCoordX = this.GetCursorPixelPosition();
                    }
                    else
                    {
                        return;
                    }
                }

                var axis = this.ParentSurface.XAxes.FirstOrDefault();
                if (axis == null)
                {
                    return;
                }

                DateTime dataValue = (DateTime)axis.GetDataValue(_pixelCoordX.Value);
                this.SetCursorPosition(dataValue);
            }
        }

        private void Cursor_DragDelta(object sender, SciChart.Charting.Visuals.Events.AnnotationDragDeltaEventArgs e)
        {
            _pixelCoordX = this.GetCursorPixelPosition();
        }

        private double GetCursorPixelPosition()
        {
            var axis = this.ParentSurface.XAxes.GetAxisById(this._cursor.XAxisId);
            var cursorPixelPosition = axis.GetCurrentCoordinateCalculator().GetCoordinate((DateTime)this._cursor.X1);
            return cursorPixelPosition;
        }

        private void SetRelativePosition(double relativePosition)
        {
            try
            {
                this._lastRelativePosition = relativePosition;

                if (!string.IsNullOrEmpty(this._cursor.XAxisId))
                {
                    this._relativePositionNeedToBeSet = true;

                    IAxis axis = this.ParentSurface.XAxes.GetAxisById(this._cursor.XAxisId);
                    if (axis != null && axis.VisibleRange != null)
                    {
                        DateRange dateRange = axis.VisibleRange as DateRange;
                        if (dateRange != null)
                        {
                            TimeSpan diffSpan = dateRange.Max - dateRange.Min;
                            TimeSpan relativeSpan = new TimeSpan((long)(diffSpan.Ticks * relativePosition));
                            DateTime value = dateRange.Min.Add(relativeSpan);

                            this.SetCursorPosition(value);
                            this._cursor.UpdateLayout();
                            double pixelPosition = this.GetCursorPixelPosition();
                            if (!double.IsNaN(pixelPosition))
                            {
                                _pixelCoordX = pixelPosition;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void SetCursorPosition(DateTime value)
        {
            this._cursor.X1 = value;
        }
    }
}
