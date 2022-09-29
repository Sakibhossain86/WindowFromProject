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
    public partial class EditPlayer : Form
    {
        string filePath, oldFile, fileName;
        string action = "Edit";
        Player player;
        public EditPlayer()
        {
            InitializeComponent();
        }
        public int PlayerToEditDelete { get; set; }
        public ICrossDataGet FormToReload { get; set; }

        private void EditPlayer_Load(object sender, EventArgs e)
        {
            ShowData();
        }
        private void ShowData()
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Player WHERE Playerid =@i", con))
                {
                    cmd.Parameters.AddWithValue("@i", this.PlayerToEditDelete);
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        textBox1.Text = dr.GetInt32(0).ToString();
                        textBox2.Text = dr.GetString(1);
                        dateTimePicker1.Value = dr.GetDateTime(2);
                        textBox3.Text = dr.GetString(3);
                        textBox4.Text = dr.GetString(4);
                        oldFile = dr.GetString(5).ToString();
                        pictureBox1.Image = Image.FromFile(Path.Combine(@"..\..\Pictures", dr.GetString(5).ToString()));
                        checkBox1.Checked = dr.GetBoolean(6);
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

                    using (SqlCommand cmd = new SqlCommand(@"UPDATE  player  
                                            SET  playername=@n, dateofbirth=@d, country= @c, playerposition=@pp, picture=@p, isplaying=@ip 
                                            WHERE playerid=@i", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text));
                        cmd.Parameters.AddWithValue("@n", textBox2.Text);
                        cmd.Parameters.AddWithValue("@d", dateTimePicker1.Value);
                        cmd.Parameters.AddWithValue("@c", textBox3.Text);
                        cmd.Parameters.AddWithValue("@pp", textBox4.Text); ;
                        cmd.Parameters.AddWithValue("@ip", checkBox1.Checked);
                        if (!string.IsNullOrEmpty(this.filePath))
                        {
                            string ext = Path.GetExtension(this.filePath);
                            fileName = $"{Guid.NewGuid()}{ext}";
                            string savePath = Path.Combine(Path.GetFullPath(@"..\..\Pictures"), fileName);
                            File.Copy(filePath, savePath, true);
                            cmd.Parameters.AddWithValue("@p", fileName);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@p", oldFile);
                        }


                        try
                        {
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Data Saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                player = new Player
                                {
                                    PlayerId = int.Parse(textBox1.Text),
                                    PlayerName = textBox2.Text,
                                    DateOfBirth = dateTimePicker1.Value,
                                    Country = textBox3.Text,
                                    PlayerPosition = textBox4.Text,
                                    Picture = fileName,
                                    IsPlaying = checkBox1.Checked
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

        private void button4_Click(object sender, EventArgs e)
        {
            this.action = "Delete";
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {

                    using (SqlCommand cmd = new SqlCommand(@"DELETE  player  
                                            WHERE playerid=@i", con, tran))
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

        private void EditPlayer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.action == "edit")
                this.FormToReload.UpdatePlayer(player);
            else
                this.FormToReload.RemovePlayer(Int32.Parse(this.textBox1.Text));
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            ShowData();
        }

        private void EditBook_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.action == "edit")
                this.FormToReload.UpdatePlayer(player);
            else
                this.FormToReload.RemovePlayer(Int32.Parse(this.textBox1.Text));
        }
    }
}
