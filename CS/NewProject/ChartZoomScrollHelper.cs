using DevExpress.XtraCharts;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace NewProject
{
    internal class ChartZoomScrollHelper
    {
        private readonly ChartControl chartControl;
        #region Properties
        public ViewType CurrentViewType { get; set; }
        public Point? FirstPoint { get; private set; }
        public Point? SecondPoint { get; private set; }
        #endregion

        public ChartZoomScrollHelper(ChartControl chart)
        {
            chartControl = chart;
            chartControl.CustomPaint += chartControl1_CustomPaint;
            chartControl.MouseDown += chartControl1_MouseDown;
            chartControl.MouseMove += chartControl1_MouseMove;
            chartControl.MouseUp += chartControl1_MouseUp;
        }
        private void chartControl1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            FirstPoint = e.Location;
        }

        private void chartControl1_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (ShouldZoom(e))
            {
                XYDiagram diagram = (XYDiagram)chartControl.Diagram;

                object argMin, argMax;
                CalcArgVisualRange(diagram, out argMin, out argMax);

                diagram.AxisX.VisualRange.SetMinMaxValues(argMin, argMax);
                FirstPoint = null;
            }

        }

        private void CalcArgVisualRange(XYDiagram diagram, out object argMin, out object argMax)
        {
            int minX = Math.Min(FirstPoint.Value.X, SecondPoint.Value.X);
            int maxX = Math.Max(FirstPoint.Value.X, SecondPoint.Value.X);
            argMin = diagram.PointToDiagram(new Point(minX, SecondPoint.Value.Y)).NumericalArgument;
            argMax = diagram.PointToDiagram(new Point(maxX, SecondPoint.Value.Y)).NumericalArgument;
        }

        private void chartControl1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            SecondPoint = e.Location;
            if (ShouldScroll())
            {
                XYDiagram diagram = (XYDiagram)chartControl.Diagram;

                double argMin = diagram.PointToDiagram(new Point(FirstPoint.Value.X, SecondPoint.Value.Y)).NumericalArgument;
                double argMax = diagram.PointToDiagram(new Point(SecondPoint.Value.X, SecondPoint.Value.Y)).NumericalArgument;
                double diff = -(argMax - argMin);
                double newMin = (double)diagram.AxisX.VisualRange.MinValue + diff;
                double newMax = (double)diagram.AxisX.VisualRange.MaxValue + diff;
                if ((newMin >= (double)diagram.AxisX.WholeRange.MinValue) && newMax <= (double)diagram.AxisX.WholeRange.MaxValue)
                    diagram.AxisX.VisualRange.SetMinMaxValues(newMin, newMax);
                FirstPoint = SecondPoint;
            }
        }
        private void chartControl1_CustomPaint(object sender, CustomPaintEventArgs e)
        {
            if (ShouldZoom())
                DrawZoomBox(e);
        }

        private void DrawZoomBox(CustomPaintEventArgs e)
        {
            int minX = Math.Min(FirstPoint.Value.X, SecondPoint.Value.X);
            int minY = Math.Min(FirstPoint.Value.Y, SecondPoint.Value.Y);
            int maxX = Math.Max(FirstPoint.Value.X, SecondPoint.Value.X);
            int maxY = Math.Max(FirstPoint.Value.Y, SecondPoint.Value.Y);
            e.Graphics.DrawRectangle(Pens.Black, minX, minY, maxX - minX, maxY - minY);
        }
        private bool ShouldScroll()
        {
            return FirstPoint != null && Control.MouseButtons == MouseButtons.Right;
        }
        private bool ShouldZoom()
        {
            return FirstPoint != null && Control.MouseButtons == MouseButtons.Left;
        }
        private bool ShouldZoom(MouseEventArgs e)
        {
            return FirstPoint != null && e.Button == MouseButtons.Left;
        }
    }
}