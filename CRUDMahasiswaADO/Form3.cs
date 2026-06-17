

                listMahasiswa.SetDataSource(dtMahasiswa);
                crystalReportViewer2.ReportSource = listMahasiswa;
                crystalReportViewer2.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load data: " + ex.Message);
            }
        }

  