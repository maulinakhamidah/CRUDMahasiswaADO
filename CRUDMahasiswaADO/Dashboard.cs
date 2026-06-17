using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using CrystalDecisions.CrystalReports.Engine;

namespace CRUDMahasiswaaADO
{
    public partial class Dashboard : Form
    {
        DAL dbLogic = new DAL();
        bool isInitializing = true;
        DataTable dt;
        int button = 0;

        public Dashboard()
        {
            InitializeComponent();

            dtpTanggalMasuk.MinDate = new DateTime(2000, 1, 1);
            dtpTanggalMasuk.Format = DateTimePickerFormat.Custom;
            dtpTanggalMasuk.CustomFormat = "yyyy";
            dtpTanggalMasuk.ShowUpDown = true;
            dtpTanggalMasuk.MaxDate = DateTime.Now;

            cmbTipe.DropDownStyle = ComboBoxStyle.DropDownList;
            var items = new List<KeyValuePair<string, SeriesChartType>>()
            {
                new KeyValuePair<string, SeriesChartType>("Kolom", SeriesChartType.Column),
                new KeyValuePair<string, SeriesChartType>("Pie", SeriesChartType.Pie)
            };

            isInitializing = true;
            cmbTipe.DataSource = items;
            cmbTipe.DisplayMember = "Key";
            cmbTipe.ValueMember = "Value";
            cmbTipe.SelectedIndex = 0;
            isInitializing = false;

            loadDataChart();
        }

        public void loadDataChart()
        {
            chart1.Series.Clear();
            chart1.Titles.Clear();
            chart1.Legends.Clear();
            chart1.ChartAreas.Clear();

            ChartArea ca = new ChartArea("MainArea");
            ca.AxisX.Title = "Program Studi";
            ca.AxisY.Title = "Jumlah Mahasiswa";
            ca.AxisX.LabelStyle.Angle = -45;
            ca.BackColor = Color.Transparent;
            chart1.ChartAreas.Add(ca);

            try
            {
                dt = (button == 1) ? dbLogic.getDataChartByTahun(dtpTanggalMasuk.Value) : dbLogic.getAllDataChart();

                SeriesChartType tipe = (SeriesChartType)cmbTipe.SelectedValue;
                Series s = new Series("Mahasiswa");
                s.ChartType = tipe;

                if (tipe != SeriesChartType.Column)
                {
                    s.IsValueShownAsLabel = true;
                    s.Label = "#VAL";
                    s.LegendText = "#VALX";
                }

                foreach (DataRow row in dt.Rows)
                {
                    string prodi = row["NamaProdi"].ToString();
                    int jumlah = Convert.ToInt32(row["JmlhMhs"]);
                    s.Points.AddXY(prodi, jumlah);
                }
                chart1.Series.Add(s);

                Title title = new Title("Jumlah Mahasiswa per Program Studi", Docking.Top, new Font("Arial", 14, FontStyle.Bold), Color.DarkBlue);
                chart1.Titles.Add(title);
                Legend legend = new Legend();
                legend.Docking = Docking.Right;
                chart1.Legends.Add(legend);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load data: " + ex.Message);
            }
        }

        private void cmbTipe_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitializing) loadDataChart();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            button = 1;
            loadDataChart();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            button = 0;
            loadDataChart();
        }
    }
}