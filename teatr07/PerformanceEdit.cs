using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace teatr07
{
    public partial class PerformanceEdit : Form
    {
        private string connectionString = "Host=172.20.7.53;Port=5432;Username=st3996;Password=pwd3996;Database=db3996_08";
        private string _username;
        private string _role;

        public PerformanceEdit(string username, string role)
        {
            InitializeComponent();
            _username = username;
            _role = role; // ✅ добавьте это
        }

        private void PerformanceEdit_Load(object sender, EventArgs e)
        {

      
            guna2ComboBox1.Items.Clear(); // Сброс залов
            guna2ComboBox3.Items.Clear(); // Сброс времени

            // Добавим залы
            guna2ComboBox1.Items.Add("1");
            guna2ComboBox1.Items.Add("2");

            // Добавим доступное время
            guna2ComboBox3.Items.Add("10:00");
            guna2ComboBox3.Items.Add("13:00");
            guna2ComboBox3.Items.Add("16:00");
            guna2ComboBox3.Items.Add("19:00");
            guna2ComboBox3.Items.Add("21:00");

            LoadPerformances();
        }

        private void LoadPerformances()
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
            SELECT p.id, p.name, p.opisanie, v.date, v.time, v.hall_id
            FROM teatr.performance p
            JOIN teatr.viewing v ON p.id = v.performance_id
            ORDER BY v.date, v.time";

                using (var cmd = new NpgsqlCommand(query, conn))
                using (var adapter = new NpgsqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView1.AutoGenerateColumns = true;
                    dataGridView1.DataSource = dt;
                }
            }
        }

        // ✅ Добавить спектакль
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            string name = guna2TextBox1.Text;
            string opisanie = guna2TextBox2.Text;
            string hall = guna2ComboBox1.SelectedItem?.ToString();
            string date = guna2DateTimePicker1.Value.ToString("yyyy-MM-dd");
            string time = guna2ComboBox3.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(opisanie) || hall == null || time == null)
            {
                MessageBox.Show("Заполните все поля.");
                return;
            }

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. performance
                        var cmdPerf = new NpgsqlCommand("INSERT INTO teatr.performance (name, opisanie) VALUES (@n, @o) RETURNING id", conn);
                        cmdPerf.Parameters.AddWithValue("n", name);
                        cmdPerf.Parameters.AddWithValue("o", opisanie);
                        int perfId = (int)cmdPerf.ExecuteScalar();

                        // 2. viewing
                        var cmdView = new NpgsqlCommand("INSERT INTO teatr.viewing (performance_id, date, time, hall_id) VALUES (@pid, @d, @t, @h)", conn);
                        cmdView.Parameters.AddWithValue("pid", perfId);
                        cmdView.Parameters.AddWithValue("d", DateTime.Parse(date));
                        cmdView.Parameters.AddWithValue("t", TimeSpan.Parse(time));
                        cmdView.Parameters.AddWithValue("h", int.Parse(hall));
                        cmdView.ExecuteNonQuery();

                        tran.Commit();
                        MessageBox.Show("Спектакль добавлен.");
                        LoadPerformances();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        MessageBox.Show("Ошибка: " + ex.Message);
                    }
                }
            }
        }

        // ❌ Удалить спектакль
        private void guna2Button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Выберите спектакль для удаления.");
                return;
            }

            int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        var delView = new NpgsqlCommand("DELETE FROM teatr.viewing WHERE performance_id = @id", conn);
                        delView.Parameters.AddWithValue("id", id);
                        delView.ExecuteNonQuery();

                        var delPerf = new NpgsqlCommand("DELETE FROM teatr.performance WHERE id = @id", conn);
                        delPerf.Parameters.AddWithValue("id", id);
                        delPerf.ExecuteNonQuery();

                        tran.Commit();
                        MessageBox.Show("Спектакль удален.");
                        LoadPerformances();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        MessageBox.Show("Ошибка: " + ex.Message);
                    }
                }
            }
        }

        // ✏️ Сохранить изменения
        private void guna2Button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Выберите спектакль для редактирования.");
                return;
            }

            int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);
            string name = guna2TextBox1.Text;
            string opisanie = guna2TextBox2.Text;
            string hall = guna2ComboBox1.SelectedItem?.ToString();
            string date = guna2DateTimePicker1.Value.ToString("yyyy-MM-dd");
            string time = guna2ComboBox3.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(opisanie) || hall == null || time == null)
            {
                MessageBox.Show("Заполните все поля.");
                return;
            }

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        var updPerf = new NpgsqlCommand("UPDATE teatr.performance SET name = @n, opisanie = @o WHERE id = @id", conn);
                        updPerf.Parameters.AddWithValue("n", name);
                        updPerf.Parameters.AddWithValue("o", opisanie);
                        updPerf.Parameters.AddWithValue("id", id);
                        updPerf.ExecuteNonQuery();

                        var updView = new NpgsqlCommand("UPDATE teatr.viewing SET date = @d, time = @t, hall_id = @h WHERE performance_id = @id", conn);
                        updView.Parameters.AddWithValue("d", DateTime.Parse(date));
                        updView.Parameters.AddWithValue("t", TimeSpan.Parse(time));
                        updView.Parameters.AddWithValue("h", int.Parse(hall));
                        updView.Parameters.AddWithValue("id", id);
                        updView.ExecuteNonQuery();

                        tran.Commit();
                        MessageBox.Show("Изменения сохранены.");
                        LoadPerformances();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        MessageBox.Show("Ошибка: " + ex.Message);
                    }
                }
            }
        }

        // При клике на строку — заполнить текстбоксы
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                guna2TextBox1.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                guna2TextBox2.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
                guna2DateTimePicker1.Value = DateTime.Parse(dataGridView1.CurrentRow.Cells[3].Value.ToString());
                guna2ComboBox3.SelectedItem = dataGridView1.CurrentRow.Cells[4].Value.ToString();
                guna2ComboBox1.SelectedItem = dataGridView1.CurrentRow.Cells[5].Value.ToString();
            }
        }

        private void guna2Button4_Click_1(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Выберите спектакль для редактирования.");
                return;
            }

            int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);
            string name = guna2TextBox1.Text;
            string opisanie = guna2TextBox2.Text;
            string hall = guna2ComboBox1.SelectedItem?.ToString();
            string date = guna2DateTimePicker1.Value.ToString("yyyy-MM-dd");
            string time = guna2ComboBox3.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(opisanie) || hall == null || time == null)
            {
                MessageBox.Show("Заполните все поля.");
                return;
            }

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        var updPerf = new NpgsqlCommand("UPDATE teatr.performance SET name = @n, opisanie = @o WHERE id = @id", conn);
                        updPerf.Parameters.AddWithValue("n", name);
                        updPerf.Parameters.AddWithValue("o", opisanie);
                        updPerf.Parameters.AddWithValue("id", id);
                        updPerf.ExecuteNonQuery();

                        var updView = new NpgsqlCommand("UPDATE teatr.viewing SET date = @d, time = @t, hall_id = @h WHERE performance_id = @id", conn);
                        updView.Parameters.AddWithValue("d", DateTime.Parse(date));
                        updView.Parameters.AddWithValue("t", TimeSpan.Parse(time));
                        updView.Parameters.AddWithValue("h", int.Parse(hall));
                        updView.Parameters.AddWithValue("id", id);
                        updView.ExecuteNonQuery();

                        tran.Commit();
                        MessageBox.Show("Изменения сохранены.");
                        LoadPerformances();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        MessageBox.Show("Ошибка: " + ex.Message);
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (_role == "admin")
            {
                admin adminForm = new admin(_username, _role);
                adminForm.Show();
            }
            else if (_role == "cashier")
            {
                cassir cassirForm = new cassir(_username,_role);
                cassirForm.Show();
            }
            else
            {
                MessageBox.Show("Неизвестная роль пользователя.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Close();
        }
    }
}
