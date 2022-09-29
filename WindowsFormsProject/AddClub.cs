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
    public partial class AddClub : Form
    {
        
        List<Club> clubs = new List<Club>();
        public AddClub()
        {
            InitializeComponent();
        }
        public ICrossDataGet FormToReload { get; set; }

        private void AddClub_Load(object sender, EventArgs e)
        {
            this.textBox1.Text = this.GetNewClubId().ToString();
        }
        private int GetNewClubId()
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(Clubid), 0) FROM Club", con))
                {
                    con.Open();
                    int id = (int)cmd.ExecuteScalar();
                    con.Close();
                    return id + 1;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {

                    using (SqlCommand cmd = new SqlCommand(@"INSERT INTO Club 
                                            (Clubid, ClubName, Orgin, League, PlayerId) VALUES
                                            (@i, @n, @o, @l,@p)", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text));
                        cmd.Parameters.AddWithValue("@n", textBox2.Text);
                        cmd.Parameters.AddWithValue("@o", textBox3.Text);
                        cmd.Parameters.AddWithValue("@l", textBox4.Text);
                        cmd.Parameters.AddWithValue("@p", int.Parse(textBox5.Text));

                        try
                        {
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Data Saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                clubs.Add(new Club
                                {
                                    ClubId = int.Parse(textBox1.Text),
                                    ClubName = textBox2.Text,
                                    Origin = textBox3.Text,
                                    League = textBox4.Text,
                                    Playerid = int.Parse(textBox5.Text)
                                }); ; ;
                                tran.Commit();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error: {ex.Message}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            tran.Rollback();
                        }
                        finally
                        {
                            if (con.State == ConnectionState.Open)
                            {
                                con.Close();
                            }
                        }
                    }
                }
            }
        }
    }
}
