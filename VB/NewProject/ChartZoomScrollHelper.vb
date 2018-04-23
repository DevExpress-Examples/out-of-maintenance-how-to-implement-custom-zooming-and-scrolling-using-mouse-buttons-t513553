Imports DevExpress.XtraCharts
Imports System
Imports System.Drawing
Imports System.Windows.Forms

Namespace NewProject
    Friend Class ChartZoomScrollHelper
        Private ReadOnly chartControl As ChartControl
        #Region "Properties"
        Public Property CurrentViewType() As ViewType
        Private privateFirstPoint? As Point
        Public Property FirstPoint() As Point?
            Get
                Return privateFirstPoint
            End Get
            Private Set(ByVal value? As Point)
                privateFirstPoint = value
            End Set
        End Property
        Private privateSecondPoint? As Point
        Public Property SecondPoint() As Point?
            Get
                Return privateSecondPoint
            End Get
            Private Set(ByVal value? As Point)
                privateSecondPoint = value
            End Set
        End Property
        #End Region

        Public Sub New(ByVal chart As ChartControl)
            chartControl = chart
            AddHandler chartControl.CustomPaint, AddressOf chartControl1_CustomPaint
            AddHandler chartControl.MouseDown, AddressOf chartControl1_MouseDown
            AddHandler chartControl.MouseMove, AddressOf chartControl1_MouseMove
            AddHandler chartControl.MouseUp, AddressOf chartControl1_MouseUp
        End Sub
        Private Sub chartControl1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
            FirstPoint = e.Location
        End Sub

        Private Sub chartControl1_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
            If ShouldZoom(e) Then
                Dim diagram As XYDiagram = CType(chartControl.Diagram, XYDiagram)

                Dim argMin As Object = Nothing, argMax As Object = Nothing
                CalcArgVisualRange(diagram, argMin, argMax)

                diagram.AxisX.VisualRange.SetMinMaxValues(argMin, argMax)
                FirstPoint = Nothing
            End If

        End Sub

        Private Sub CalcArgVisualRange(ByVal diagram As XYDiagram, ByRef argMin As Object, ByRef argMax As Object)
            Dim minX As Integer = Math.Min(FirstPoint.Value.X, SecondPoint.Value.X)
            Dim maxX As Integer = Math.Max(FirstPoint.Value.X, SecondPoint.Value.X)
            argMin = diagram.PointToDiagram(New Point(minX, SecondPoint.Value.Y)).NumericalArgument
            argMax = diagram.PointToDiagram(New Point(maxX, SecondPoint.Value.Y)).NumericalArgument
        End Sub

        Private Sub chartControl1_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
            SecondPoint = e.Location
            If ShouldScroll() Then
                Dim diagram As XYDiagram = CType(chartControl.Diagram, XYDiagram)

                Dim argMin As Double = diagram.PointToDiagram(New Point(FirstPoint.Value.X, SecondPoint.Value.Y)).NumericalArgument
                Dim argMax As Double = diagram.PointToDiagram(New Point(SecondPoint.Value.X, SecondPoint.Value.Y)).NumericalArgument
                Dim diff As Double = -(argMax - argMin)
                Dim newMin As Double = CDbl(diagram.AxisX.VisualRange.MinValue) + diff
                Dim newMax As Double = CDbl(diagram.AxisX.VisualRange.MaxValue) + diff
                If (newMin >= CDbl(diagram.AxisX.WholeRange.MinValue)) AndAlso newMax <= CDbl(diagram.AxisX.WholeRange.MaxValue) Then
                    diagram.AxisX.VisualRange.SetMinMaxValues(newMin, newMax)
                End If
                FirstPoint = SecondPoint
            End If
        End Sub
        Private Sub chartControl1_CustomPaint(ByVal sender As Object, ByVal e As CustomPaintEventArgs)
            If ShouldZoom() Then
                DrawZoomBox(e)
            End If
        End Sub

        Private Sub DrawZoomBox(ByVal e As CustomPaintEventArgs)
            Dim minX As Integer = Math.Min(FirstPoint.Value.X, SecondPoint.Value.X)
            Dim minY As Integer = Math.Min(FirstPoint.Value.Y, SecondPoint.Value.Y)
            Dim maxX As Integer = Math.Max(FirstPoint.Value.X, SecondPoint.Value.X)
            Dim maxY As Integer = Math.Max(FirstPoint.Value.Y, SecondPoint.Value.Y)
            e.Graphics.DrawRectangle(Pens.Black, minX, minY, maxX - minX, maxY - minY)
        End Sub
        Private Function ShouldScroll() As Boolean
            Return FirstPoint IsNot Nothing AndAlso Control.MouseButtons = MouseButtons.Right
        End Function
        Private Function ShouldZoom() As Boolean
            Return FirstPoint IsNot Nothing AndAlso Control.MouseButtons = MouseButtons.Left
        End Function
        Private Function ShouldZoom(ByVal e As MouseEventArgs) As Boolean
            Return FirstPoint IsNot Nothing AndAlso e.Button = MouseButtons.Left
        End Function
    End Class
End Namespace