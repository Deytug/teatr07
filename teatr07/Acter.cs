using Guna.UI2.WinForms.Suite;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace teatr07
{
    public partial class Acter : Form
    {

        private string connectionString = "Host=172.20.7.53;Port=5432;Username=st3996;Password=pwd3996;Database=db3996_08";
        private string _username;
        private string _role;
        public Acter(string username,string role)
        {
            InitializeComponent();
            _username = username;  // Значение username передается правильно
            _role = role;

        }

       

        private void Acter_Load(object sender, EventArgs e)
        {
            LoadActors();
        }
        private void LoadActors()
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
            SELECT a.id, p.name AS performance_name, a.name AS actor_name, a.role
            FROM teatr.actors a
            JOIN teatr.performance p ON a.performance_id = p.id
            ORDER BY p.name";

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

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            string performanceName = guna2TextBox1.Text;
            string actorName = guna2TextBox2.Text;
            string actorRole = guna2TextBox3.Text;

            if (string.IsNullOrWhiteSpace(performanceName) || string.IsNullOrWhiteSpace(actorName) || string.IsNullOrWhiteSpace(actorRole))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                // Найдём performance_id по названию
                var cmdFind = new NpgsqlCommand("SELECT id FROM teatr.performance WHERE name = @name", conn);
                cmdFind.Parameters.AddWithValue("name", performanceName);
                object result = cmdFind.ExecuteScalar();

                if (result == null)
                {
                    MessageBox.Show("Спектакль не найден.");
                    return;
                }

                int performanceId = Convert.ToInt32(result);

                var cmdInsert = new NpgsqlCommand("INSERT INTO teatr.actors(name, role, performance_id) VALUES (@name, @role, @performance_id)", conn);
                cmdInsert.Parameters.AddWithValue("name", actorName);
                cmdInsert.Parameters.AddWithValue("role", actorRole);
                cmdInsert.Parameters.AddWithValue("performance_id", performanceId);
                cmdInsert.ExecuteNonQuery();

                MessageBox.Show("Актёр добавлен.");
                LoadActors();
            }
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите актёра для удаления.");
                return;
            }

            int actorId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["id"].Value);

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new NpgsqlCommand("DELETE FROM teatr.actors WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("id", actorId);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Актёр удалён.");
                LoadActors();
            }
        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите актёра для редактирования.");
                return;
            }

            int actorId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["id"].Value);
            string newName = guna2TextBox2.Text;
            string newRole = guna2TextBox3.Text;

            if (string.IsNullOrWhiteSpace(newName) || string.IsNullOrWhiteSpace(newRole))
            {
                MessageBox.Show("Введите имя и роль для обновления.");
                return;
            }

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new NpgsqlCommand("UPDATE teatr.actors SET name = @name, role = @role WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("name", newName);
                cmd.Parameters.AddWithValue("role", newRole);
                cmd.Parameters.AddWithValue("id", actorId);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Изменения сохранены.");
                LoadActors();
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
                cassir cassirForm = new cassir(_username, _role);
                cassirForm.Show();
            }
            else
            {
                MessageBox.Show("Неизвестная роль пользователя.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Close();
        }
        private void button37_Click(object sender, EventArgs e)
        {

        }
    } 
       
 }
