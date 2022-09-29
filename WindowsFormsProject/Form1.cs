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

namespace WindowsFormsProject
{
    public partial class Form1 : Form,ICrossDataGet
    {
        DataSet ds;
        BindingSource psPlayer = new BindingSource();
        BindingSource psClub = new BindingSource();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            LoadData();
            BindData();
        }
        public void LoadData()
        {
            ds = new DataSet();
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Player", con))
                {
                    da.Fill(ds, "Player");
                    ds.Tables["Player"].Columns.Add(new DataColumn("image", typeof(System.Byte[])));
                    for (var i = 0; i < ds.Tables["Player"].Rows.Count; i++)
                    {
                        ds.Tables["Player"].Rows[i]["image"] = File.ReadAllBytes(Path.Combine(Path.GetFullPath(@"..\..\Pictures"), ds.Tables["Player"].Rows[i]["Picture"].ToString()));
                    }
                    da.SelectCommand.CommandText = "SELECT * FROM Club";
                    da.Fill(ds, "Club");
                    DataRelation rel = new DataRelation("FK_BOOK_TOC",
                        ds.Tables["Player"].Columns["Playerid"],
                        ds.Tables["Club"].Columns["Playerid"]);
                    ds.Relations.Add(rel);
                    ds.AcceptChanges();
                }
            }
        }
        private void BindData()
        {
            psPlayer.DataSource = ds;
            psPlayer.DataMember = "Player";
            psClub.DataSource = psPlayer;
            psClub.DataMember = "FK_BOOK_TOC";
            this.dataGridView1.DataSource = psClub;
            lbl1.DataBindings.Add(new Binding("Text", psPlayer, "PlayerName"));
            lbl2.DataBindings.Add(new Binding("Text", psPlayer, "DateOfBirth", true));
            lbl3.DataBindings.Add(new Binding("Text", psPlayer, "Country"));
            lbl4.DataBindings.Add(new Binding("Text", psPlayer, "PlayerPosition"));
            checkBox1.DataBindings.Add(new Binding("Checked", psPlayer, "IsPlaying"));
            pictureBox1.DataBindings.Add(new Binding("Image", psPlayer, "image", true));
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AddPlayer() { FormToReload = this }.ShowDialog();
        }

        public void ReloadData(List<Player> players)
        {
            foreach (var p in players)
            {
                DataRow dr = ds.Tables["Player"].NewRow();
                dr[0] = p.PlayerId;
                dr["PlayerName"] = p.PlayerName;
                dr["DateOfBirth"] = p.DateOfBirth;
                dr["Country"] = p.Country;
                dr["PlayerPosition"] = p.PlayerPosition;
                dr["Picture"] = p.Picture;
                dr["IsPlaying"] = p.IsPlaying;
                dr["image"] = File.ReadAllBytes(Path.Combine(Path.GetFullPath(@"..\..\Pictures"), p.Picture));
                ds.Tables["Player"].Rows.Add(dr);

            }
            ds.AcceptChanges();
            psPlayer.MoveLast();
        }

        public void UpdatePlayer(Player p)
        {
            for (var i = 0; i < ds.Tables["Player"].Rows.Count; i++)
            {
                if ((int)ds.Tables["Player"].Rows[i]["Playerid"] == p.PlayerId)
                {
                    ds.Tables["Player"].Rows[i]["PlayerName"] = p.PlayerName;
                    ds.Tables["Player"].Rows[i]["DateOfBirth"] = p.DateOfBirth;
                    ds.Tables["Player"].Rows[i]["Country"] = p.Country;
                    ds.Tables["Player"].Rows[i]["PlayerPosition"] = p.PlayerPosition;
                    ds.Tables["Player"].Rows[i]["image"] = File.ReadAllBytes(Path.Combine(Path.GetFullPath(@"..\..\Pictures"), p.Picture));
                    ds.Tables["Player"].Rows[i]["IsPlaying"] = p.IsPlaying;
                    break;
                }
            }
            ds.AcceptChanges();
        }

        public void RemovePlayer(int id)
        {
            for (var i = 0; i < ds.Tables["Player"].Rows.Count; i++)
            {
                if ((int)ds.Tables["Player"].Rows[i]["Playerid"] == id)
                {
                    ds.Tables["Player"].Rows.RemoveAt(i);
                    break;
                }
            }
            ds.AcceptChanges();
        }

        private void editDeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int id = (int)(this.psPlayer.Current as DataRowView).Row[0];
            new EditPlayer  { PlayerToEditDelete = id, FormToReload = this }.ShowDialog();
        }

        public void ReloadData2(List<Club> clubs)
        {
            foreach (var c in clubs)
            {
                DataRow dr = ds.Tables["Club"].NewRow();
                dr[0] = c.ClubId;
                dr["ClubName"] = c.ClubName;
                dr["Orgin"] = c.Origin;
                dr["League"] = c.League;
                dr["Playerid"] = c.Playerid;
                ds.Tables["Club"].Rows.Add(dr);
            }
            ds.AcceptChanges();
            
        }

        public void RemoveClub(int id)
        {
            for (var i = 0; i < ds.Tables["Club"].Rows.Count; i++)
            {
                if ((int)ds.Tables["Club"].Rows[i]["ClubId"] == id)
                {
                    ds.Tables["Club"].Rows.RemoveAt(i);
                    break;
                }
            }
        }

        public void UpdateClub(Club club)
        {
            for (var i = 0; i < ds.Tables["Club"].Rows.Count; i++)
            {
                if ((int)ds.Tables["Club"].Rows[i]["ClubId"] == club.ClubId)
                {
                    ds.Tables["Club"].Rows[i]["ClubName"] = club.ClubName;
                    ds.Tables["Club"].Rows[i]["Orgin"] = club.Origin;
                    ds.Tables["Club"].Rows[i]["League"] = club.League;
                    ds.Tables["Club"].Rows[i]["Palyerid"] = club.Playerid;
                    break;
                }
            }
            ds.AcceptChanges();
        }

        private void editDeteteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int id = (int)(this.psClub.Current as DataRowView).Row[0];
            new EditClub { ClubToEditDelete = id, FormToReload = this }.ShowDialog();
        }

        private void addToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new AddClub() { FormToReload = this }.ShowDialog();
        }

        private void toolStripButton13_Click(object sender, EventArgs e)
        {
            if (psPlayer.Position < psPlayer.Count - 1)
            {
                psPlayer.MoveNext();
            }
        }

        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            psPlayer.MoveFirst();
        }

        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            if (psPlayer.Position > 0)
            {
                psPlayer.MovePrevious();
            }
        }

        private void toolStripButton14_Click(object sender, EventArgs e)
        {
            psPlayer.MoveLast();
        }

        private void toolStripButton15_Click(object sender, EventArgs e)
        {
            int id = (int)(this.psPlayer.Current as DataRowView).Row[0];
            new EditPlayer { PlayerToEditDelete = id, FormToReload = this }.ShowDialog();
        }

        private void playerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FormPlayerRpt().ShowDialog();
        }

        private void playerClubToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FormPlayerGroup().ShowDialog();
        }
    }
}
