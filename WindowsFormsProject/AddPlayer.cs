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
    public partial class AddPlayer : Form
    {
        string filePath = "";
        List<Player> player = new List<Player>();
        public AddPlayer()
        {
            InitializeComponent();
        }
        public ICrossDataGet FormToReload { get; set; }
        private void AddPlayer_Load(object sender, EventArgs e)
        {
            this.textBox1.Text = this.GetNewPlayerId().ToString();
        }
        private int GetNewPlayerId()
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(Playerid), 0) FROM Player", con))
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

                    using (SqlCommand cmd = new SqlCommand(@"INSERT INTO Player 
                                            (Playerid, PlayerName, DateOfBirth, Country, PlayerPosition, Picture,IsPlaying) VALUES
                                            (@i, @n, @d, @c, @pp, @pi,@ip)", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text));
                        cmd.Parameters.AddWithValue("@n", textBox2.Text);
                        cmd.Parameters.AddWithValue("@d", dateTimePicker1.Value);
                        cmd.Parameters.AddWithValue("@c", textBox3.Text);
                        cmd.Parameters.AddWithValue("@pp", textBox4.Text); ;
                        cmd.Parameters.AddWithValue("@ip", checkBox1.Checked);
                        string ext = Path.GetExtension(this.filePath);
                        string fileName = $"{Guid.NewGuid()}{ext}";
                        string savePath = Path.Combine(Path.GetFullPath(@"..\..\Pictures"), fileName);
                        File.Copy(filePath, savePath, true);
                        cmd.Parameters.AddWithValue("@pi", fileName);

                        try
                        {
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Data Saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                player.Add(new Player
                                {
                                    PlayerId = int.Parse(textBox1.Text),
                                    PlayerName = textBox2.Text,
                                    DateOfBirth = dateTimePicker1.Value,
                                    Country = textBox3.Text,
                                    PlayerPosition = textBox4.Text,
                                    Picture = fileName,
                                    IsPlaying=checkBox1.Checked
                                }); ;
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

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.filePath = this.openFileDialog1.FileName;
                this.label9.Text = Path.GetFileName(this.filePath);
                this.pictureBox1.Image = Image.FromFile(this.filePath);
            }
        }

        private void AddPlayer_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.FormToReload.ReloadData(this.player);
        }
    }
    
}
