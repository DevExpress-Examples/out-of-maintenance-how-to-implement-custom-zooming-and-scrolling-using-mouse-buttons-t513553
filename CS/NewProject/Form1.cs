using System;
using DevExpress.XtraEditors;
using System.Data;
using DevExpress.XtraCharts;

namespace NewProject
{
    public partial class Form1 : XtraForm
    {
        #region Properties
        public ViewType CurrentViewType { get; set; }
        #endregion
        ChartZoomScrollHelper chartHelper;
        public Form1()
        {
            InitializeComponent();

            UpdateDataSource();
            InitializeChartControl();
        }


        DataTable CreateData(int rows)
        {
            Random _r = new Random();

            DataTable dt = new DataTable();
            dt.Columns.Add("Arg", typeof(int));
            dt.Columns.Add("Value", typeof(int));
            for (int i = 0; i < rows; i++)
                dt.Rows.Add(i, _r.Next(100));
            return dt;
        }


        private void InitializeChartControl()
        {
            CurrentViewType = ViewType.Area;
            chartControl1.Series.Clear();
            int lastSeriesIndex = chartControl1.Series.Add("MainSeries", CurrentViewType);

            chartControl1.Series[lastSeriesIndex].ValueDataMembers.AddRange(new string[] { "Value" });
            chartControl1.Series[lastSeriesIndex].ArgumentDataMember = "Arg";
            ((XYDiagram)chartControl1.Diagram).AxisY.VisualRange.Auto = false;

            chartHelper = new ChartZoomScrollHelper(chartControl1);
        }

        private void UpdateDataSource()
        {
            chartControl1.DataSource = CreateData(10);
        }

    }
}
