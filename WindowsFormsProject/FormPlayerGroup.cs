using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsProject.Reports;

namespace WindowsFormsProject
{
    public partial class FormPlayerGroup : Form
    {
        public FormPlayerGroup()
        {
            InitializeComponent();
        }

        private void FormPlayerGroup_Load(object sender, EventArgs e)
        {

            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Player", con))
                {
                    da.Fill(ds, "Playeri");
                    ds.Tables["Playeri"].Columns.Add(new DataColumn("image", typeof(System.Byte[])));
                    for (var i = 0; i < ds.Tables["Playeri"].Rows.Count; i++)
                    {
                        ds.Tables["Playeri"].Rows[i]["image"] = File.ReadAllBytes(Path.Combine(Path.GetFullPath(@"..\..\Pictures"), ds.Tables["Playeri"].Rows[i]["Picture"].ToString()));
                    }
                    da.SelectCommand.CommandText = "SELECT * FROM Club";
                    da.Fill(ds, "Club");
                   FormPlayerGroupRpt  rpt = new FormPlayerGroupRpt();
                    rpt.SetDataSource(ds);
                    crystalReportViewer1.ReportSource = rpt;
                    rpt.Refresh();
                    crystalReportViewer1.Refresh();
                }
            }
        }
    }
}
