using System;
using System.Drawing;
using System.Windows.Forms;
using Npgsql;

namespace teatr07
{
    public partial class spek : Form
    {
        private int _performanceId;
        private string _connectionString = "Host=172.20.7.53;Port=5432;Username=st3996;Password=pwd3996;Database=db3996_08";
        private string _currentUser;
        private string _role;

        public spek(string username, int performanceId)
        {
            InitializeComponent();
            _currentUser = username ?? throw new ArgumentNullException(nameof(username));
            _performanceId = performanceId;
        }

        private void spek_Load(object sender, EventArgs e)
        {
            LoadPerformanceData();
            LoadPerformanceImage();

        }

        private void LoadPerformanceData()
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                // Убрали actors из запроса
                string query = "SELECT name, opisanie FROM teatr.performance WHERE id = @id";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("id", _performanceId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            label2.Text = reader.GetString(0); // Название
                            label3.Text = reader.GetString(1); // Описание

                            // Получаем актеров из отдельного запроса
                            label4.Text = GetActorsForPerformance(_performanceId);
                        }
                    }
                }
            }
        }

        private string GetActorsForPerformance(int performanceId)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT name, role FROM teatr.actors WHERE performance_id = @id ORDER BY id";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("id", performanceId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        string actors = "";
                        while (reader.Read())
                        {
                            actors += $"{reader.GetString(0)} ({reader.GetString(1)})\n";
                        }
                        return actors;
                    }
                }
            }
        }

        private void LoadPerformanceImage()
        {
            try
            {
                switch (_performanceId)
                {
                    case 1:
                        guna2PictureBox2.Image = Properties.Resources.img1;
                        break;
                    case 2:
                        guna2PictureBox2.Image = Properties.Resources.img2;
                        break;
                    case 3:
                        guna2PictureBox2.Image = Properties.Resources.img3;
                        break;
                    case 4:
                        guna2PictureBox2.Image = Properties.Resources.img44;
                        break;
                    case 5:
                        guna2PictureBox2.Image = Properties.Resources.img55;
                        break;
                    case 6:
                        guna2PictureBox2.Image = Properties.Resources.img6;
                        break;
                    default:
                        guna2PictureBox2.Image = null;
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке изображения: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                guna2PictureBox2.Image = null;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            buy form5 = new buy(_currentUser,_role);
            form5.Show();
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            poster form4 = new poster(_currentUser);
            form4.Show();
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
