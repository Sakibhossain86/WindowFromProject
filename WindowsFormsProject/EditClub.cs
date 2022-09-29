using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsProject
{
    public partial class EditClub : Form
    { 
        string action = "Edit";
        Club club;
        public int ClubToEditDelete { get; set; }
        public ICrossDataGet FormToReload { get; set; }
        public EditClub()
        {
            InitializeComponent();
        }

        private void EditClub_Load(object sender, EventArgs e)
        {
            ShowData();
        }
        private void ShowData()
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Club WHERE ClubId =@i", con))
                {
                    cmd.Parameters.AddWithValue("@i", this.ClubToEditDelete);
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        textBox1.Text = dr.GetInt32(0).ToString();
                        textBox2.Text = dr.GetString(1);
                        textBox3.Text = dr.GetString(2);
                        textBox4.Text = dr.GetString(3);
                        textBox5.Text = dr.GetInt32(4).ToString();
                    }
                    con.Close();
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.action = "Edit";
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {

                    using (SqlCommand cmd = new SqlCommand(@"UPDATE  Club  
                                            SET  ClubName=@n, Orgin=@o, League= @l, PlayerId=@p
                                            WHERE ClubId=@i", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text));
                        cmd.Parameters.AddWithValue("@n", textBox2.Text);
                        cmd.Parameters.AddWithValue("@o", textBox3.Text);
                        cmd.Parameters.AddWithValue("@l", textBox4.Text);
                        cmd.Parameters.AddWithValue("@p", textBox5.Text);


                        try
                        {
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Data Saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                club = new Club
                                {
                                    ClubId = int.Parse(textBox1.Text),
                                    ClubName = textBox2.Text,
                                    Origin = textBox3.Text,
                                    League = textBox4.Text,
                                    Playerid = int.Parse(textBox5.Text)
                                };
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

        private void button3_Click(object sender, EventArgs e)
        {
            ShowData();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.action = "Delete";
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {

                    using (SqlCommand cmd = new SqlCommand(@"DELETE  Club  
                                            WHERE ClubId=@i", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text));



                        try
                        {
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Data Saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

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

        private void EditClub_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.action == "edit")
                this.FormToReload.UpdateClub(club);
            else
                this.FormToReload.RemoveClub(Int32.Parse(this.textBox1.Text));
        }
    }
}
