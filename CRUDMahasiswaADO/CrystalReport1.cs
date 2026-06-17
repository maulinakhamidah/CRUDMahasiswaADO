
        
        public virtual CrystalDecisions.CrystalReports.Engine.ReportDocument CreateReport() {
            CrystalReport1 rpt = new CrystalReport1();
            rpt.Site = this.Site;
            return rpt;
        }
        
      