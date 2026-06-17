using System;
using System.Data;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;

namespace CRUDMahasiswaADO
{
    public partial class Report : Form
    {
        DAL dbLogic = new DAL();
        ReportDocument listMahasiswa = new ReportDocument();
        string prodi;
        DateTime tglmasuk;

        public Report(string Prodi, DateTime TglMasuk)
        {
            InitializeComponent();
            prodi = Prodi;
            tglmasuk = TglMasuk;
        }

        private void Report_Load(object sender, EventArgs e)
        {
            try
            {
                DataTable dtMahasiswa = dbLogic.getDataRekap(prodi, tglmasuk);

                string reportPath = Application.StartupPath + @"\CrystalReport1.rpt";
                listMahasiswa.Load(reportPath);
                listMahasiswa.SetDataSource(dtMahasiswa);

                crystalReportViewer1.ReportSource = listMahasiswa;
                crystalReportViewer1.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load data: " + ex.Message);
            }
        }
    }
}