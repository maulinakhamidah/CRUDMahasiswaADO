using System;
using System.Data;
using System.Data.SqlClient;

namespace CRUDMahasiswaADO 
{
    public class DAL
    {
        static string connectionString = @"Data Source=LAPTOP-66MU6CLK\MAULINAA;Initial Catalog=DBAkademikADO;Integrated Security=True;";

        public string GetConnectionString()
        {
            return connectionString;
        }

        SqlConnection conn;
        SqlDataAdapter da;
        DataTable dtMahasiswa;
        DataTable dtProdi;

        public DAL()
        {
            conn = new SqlConnection(connectionString);
        }

        public DataTable GetMhs()
        {
            if (conn.State == ConnectionState.Closed) conn.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("sp_GetMahasiswa", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                da = new SqlDataAdapter(cmd);
                dtMahasiswa = new DataTable();
                da.Fill(dtMahasiswa);
                return dtMahasiswa;
            }
            finally { conn.Close(); }
        }

        public int CountMhs()
        {
            if (conn.State == ConnectionState.Closed) conn.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("sp_CountMahasiswa", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter outputParam = new SqlParameter("@Total", SqlDbType.Int);
                outputParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outputParam);
                cmd.ExecuteNonQuery();
                
                return outputParam.Value != DBNull.Value ? Convert.ToInt32(outputParam.Value) : 0;
            }
            finally { conn.Close(); }
        }

        public void InsertMhs(string nim, string nama, string alamat, string jenisKelamin, DateTime tanggalLahir, string kodeProdi, byte[] foto)
        {
            if (conn.State == ConnectionState.Closed) conn.Open();
            SqlTransaction trans = conn.BeginTransaction();
            try
            {
                SqlCommand command = new SqlCommand("sp_InsertMahasiswa", conn, trans);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@pNIM", nim);
                command.Parameters.AddWithValue("@pNama", nama);
                command.Parameters.AddWithValue("@pAlamat", alamat);
                command.Parameters.AddWithValue("@pTanggalLahir", tanggalLahir);
                command.Parameters.AddWithValue("@pJenisKelamin", jenisKelamin);
                command.Parameters.AddWithValue("@pKodeProdi", kodeProdi);
                if (foto != null)
                    command.Parameters.Add("@pFoto", SqlDbType.VarBinary, -1).Value = foto;
                else
                    command.Parameters.Add("@pFoto", SqlDbType.VarBinary, -1).Value = DBNull.Value;
                
                command.ExecuteNonQuery();
                trans.Commit();
            }
            catch
            {
                trans.Rollback();
                throw; 
            }
            finally
            {
                conn.Close();
            }
        }

        public void UpdateMhs(string nim, string nama, string alamat, string jenisKelamin, DateTime tanggalLahir, string kodeProdi, byte[] foto)
        {
            if (conn.State == ConnectionState.Closed) conn.Open();
            try
            {
                SqlCommand command = new SqlCommand("sp_UpdateMahasiswa", conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@pNIM", nim);
                command.Parameters.AddWithValue("@pNama", nama);
                command.Parameters.AddWithValue("@pAlamat", alamat);
                command.Parameters.AddWithValue("@pJenisKelamin", jenisKelamin);
                command.Parameters.AddWithValue("@pTanggalLahir", tanggalLahir);
                command.Parameters.AddWithValue("@pKodeProdi", kodeProdi);
                if (foto != null)
                    command.Parameters.Add("@pFoto", SqlDbType.VarBinary, -1).Value = foto;
                else
                    command.Parameters.Add("@pFoto", SqlDbType.VarBinary, -1).Value = DBNull.Value;
                
                command.ExecuteNonQuery();
            }
            finally { conn.Close(); }
        }

        public void DeleteMhs(string nim)
        {
            if (conn.State == ConnectionState.Closed) conn.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("sp_DeleteMahasiswa", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NIM", nim);
                cmd.ExecuteNonQuery();
            }
            finally { conn.Close(); }
        }

        public void resetData()
        {
            if (conn.State == ConnectionState.Closed) conn.Open();
            try
            {
                string deleteQuery = "DELETE FROM mahasiswa;";
                SqlCommand cmdDelete = new SqlCommand(deleteQuery, conn);
                cmdDelete.ExecuteNonQuery();

                string insertQuery = "INSERT INTO mahasiswa (NIM, Nama, JenisKelamin, TanggalLahir, Alamat, KodeProdi, TanggalDaftar) SELECT NIM, Nama, JenisKelamin, TanggalLahir, Alamat, KodeProdi, TanggalDaftar FROM mahasiswa_backup;";
                SqlCommand cmdInsert = new SqlCommand(insertQuery, conn);
                cmdInsert.ExecuteNonQuery();
            }
            finally { conn.Close(); }
        }

        public void testInject(string nim)
        {
            if (conn.State == ConnectionState.Closed) conn.Open();
            try
            {
                string query = "Update mahasiswa set nama = 'HACKED' where NIM = '" + nim + "'";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.ExecuteNonQuery();
            }
            finally { conn.Close(); }
        }

        public DataTable GetMhsByNIM(string nim)
        {
            if (conn.State == ConnectionState.Closed) conn.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("sp_GetMahasiswaByNIM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@pNIM", nim);
                da = new SqlDataAdapter(cmd);
                dtMahasiswa = new DataTable();
                da.Fill(dtMahasiswa);
                return dtMahasiswa;
            }
            finally { conn.Close(); }
        }

        public void InsertLog(string message)
        {
            if (conn.State == ConnectionState.Closed) conn.Open();
            try
            {
                string query = "INSERT INTO LogError VALUES(GETDATE(), @Pesan)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Pesan", message);
                cmd.ExecuteNonQuery();
            }
            finally { conn.Close(); }
        }

        public DataTable getProdi()
        {
            if (conn.State == ConnectionState.Closed) conn.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("select namaprodi from prodi", conn);
                cmd.CommandType = CommandType.Text;
                dtProdi = new DataTable();
                da = new SqlDataAdapter(cmd);
                da.Fill(dtProdi);
                return dtProdi;
            }
            finally { conn.Close(); }
        }

        public DataTable getDataRekap(string prodi, DateTime tanggalMasuk)
        {
            if (conn.State == ConnectionState.Closed) conn.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("sp_Report", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inProdi", prodi);
                cmd.Parameters.AddWithValue("@intglMsuk", tanggalMasuk.Year.ToString());
                da = new SqlDataAdapter(cmd);
                dtMahasiswa = new DataTable();
                da.Fill(dtMahasiswa);
                return dtMahasiswa;
            }
            finally { conn.Close(); }
        }

        public DataTable getAllDataChart()
        {
            if (conn.State == ConnectionState.Closed) conn.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("sp_Dashboard", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                da = new SqlDataAdapter(cmd);
                dtMahasiswa = new DataTable();
                da.Fill(dtMahasiswa);
                return dtMahasiswa;
            }
            finally { conn.Close(); }
        }

        public DataTable getDataChartByTahun(DateTime thMasuk)
        {
            if (conn.State == ConnectionState.Closed) conn.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("sp_DashboardByTahun", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inTglMsuk", thMasuk.Year);
                da = new SqlDataAdapter(cmd);
                dtMahasiswa = new DataTable();
                da.Fill(dtMahasiswa);
                return dtMahasiswa;
            }
            finally { conn.Close(); }
        }
    }
}
