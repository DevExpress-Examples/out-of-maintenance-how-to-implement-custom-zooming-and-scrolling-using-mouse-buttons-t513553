Imports System
Imports DevExpress.XtraEditors
Imports System.Data
Imports DevExpress.XtraCharts

Namespace NewProject
    Partial Public Class Form1
        Inherits XtraForm

        #Region "Properties"
        Public Property CurrentViewType() As ViewType
        #End Region
        Private chartHelper As ChartZoomScrollHelper
        Public Sub New()
            InitializeComponent()

            UpdateDataSource()
            InitializeChartControl()
        End Sub


        Private Function CreateData(ByVal rows As Integer) As DataTable
            Dim _r As New Random()

            Dim dt As New DataTable()
            dt.Columns.Add("Arg", GetType(Integer))
            dt.Columns.Add("Value", GetType(Integer))
            For i As Integer = 0 To rows - 1
                dt.Rows.Add(i, _r.Next(100))
            Next i
            Return dt
        End Function


        Private Sub InitializeChartControl()
            CurrentViewType = ViewType.Area
            chartControl1.Series.Clear()
            Dim lastSeriesIndex As Integer = chartControl1.Series.Add("MainSeries", CurrentViewType)

            chartControl1.Series(lastSeriesIndex).ValueDataMembers.AddRange(New String() { "Value" })
            chartControl1.Series(lastSeriesIndex).ArgumentDataMember = "Arg"
            CType(chartControl1.Diagram, XYDiagram).AxisY.VisualRange.Auto = False

            chartHelper = New ChartZoomScrollHelper(chartControl1)
        End Sub

        Private Sub UpdateDataSource()
            chartControl1.DataSource = CreateData(10)
        End Sub

    End Class
End Namespace
