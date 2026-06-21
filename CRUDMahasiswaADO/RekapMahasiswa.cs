using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace CRUDMahasiswaADO
{
    public partial class RekapMahasiswa : Form
    {
        private static string connectionString = @"Data Source=LAPTOP-66MU6CLK\MAULINAA;Initial Catalog=DBAkademikADO;Integrated Security=True";
        private SqlConnection conn;
        private SqlDataAdapter da;
        private DataTable dtMahasiswa;
        private DataTable dtProdi;

        string prodi { get; set; }
        DateTime tglmasuk { get; set; }

        public RekapMahasiswa()
        {
            InitializeComponent();
            conn = new SqlConnection(connectionString);
        }

        public RekapMahasiswa(string Prodi, DateTime TglMasuk)
        {
            InitializeComponent();
            conn = new SqlConnection(connectionString);
            this.prodi = Prodi;
            this.tglmasuk = TglMasuk;
        }

        private void RekapMahasiswa_Load(object sender, EventArgs e)
        {
            dtpTanggalMasuk.Format = DateTimePickerFormat.Custom;
            dtpTanggalMasuk.CustomFormat = "yyyy";
            dtpTanggalMasuk.ShowUpDown = true;
            dtpTanggalMasuk.MinDate = new DateTime(2000, 1, 1);
            dtpTanggalMasuk.MaxDate = DateTime.Now;

            cmbProdi.DropDownStyle = ComboBoxStyle.DropDownList;

            btnCetak.Enabled = true;

            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT namaprodi FROM programstudi", conn);
                dtProdi = new DataTable();
                da = new SqlDataAdapter(cmd);
                da.Fill(dtProdi);

                cmbProdi.DataSource = dtProdi;
                cmbProdi.DisplayMember = "namaprodi";
                cmbProdi.ValueMember = "namaprodi";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load data prodi: " + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        private void btnCari_Click(object sender, EventArgs e)
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                SqlCommand cmd = new SqlCommand("sp_Report", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@inProdi", SqlDbType.VarChar, 50).Value = cmbProdi.SelectedValue;
                cmd.Parameters.Add("@inTglMsuk", SqlDbType.Int).Value = dtpTanggalMasuk.Value.Year;

                da = new SqlDataAdapter(cmd);
                dtMahasiswa = new DataTable();
                da.Fill(dtMahasiswa);

                dgvMahasiswa.DataSource = dtMahasiswa;

                if (dtMahasiswa.Rows.Count > 0)
                {
                    btnCetak.Enabled = true;
                }
                else
                {
                    btnCetak.Enabled = true;
                    MessageBox.Show("Data tidak ditemukan");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load data: " + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        private void btnCetak_Click(object sender, EventArgs e)
        {
            try
            {
                string prodiTerpilih = cmbProdi.SelectedValue != null ? cmbProdi.SelectedValue.ToString() : "";
                DateTime tanggalTerpilih = dtpTanggalMasuk.Value;

                Form3 frm3 = new Form3(prodiTerpilih, tanggalTerpilih);

                frm3.Show();

                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal membuka halaman cetak: " + ex.Message);
            }
        }

        private void cmbProdi_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}