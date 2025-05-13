using Npgsql;
using System;
using System.Data;
using System.Windows.Forms;
using ClosedXML.Excel;

namespace teatr07
{
    public partial class buyticket : Form
    {
        private string connectionString = "Host=172.20.7.53;Port=5432;Username=st3996;Password=pwd3996;Database=db3996_08";
        private string _username;
        private string _role;

        public buyticket(string username, string role)
        {
            InitializeComponent();
            _username = username;
            _role = role;
        }

        // Метод для загрузки названий спектаклей в комбобокс
        private void LoadPerformanceNames()
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT name FROM teatr.performance ORDER BY name;";
                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    guna2ComboBox1.Items.Clear(); // Очистить комбобокс перед загрузкой данных
                    while (reader.Read())
                    {
                        guna2ComboBox1.Items.Add(reader.GetString(0)); // Добавляем название спектакля
                    }
                }
            }
        }

        // Кнопка для генерации отчета
        private void guna2Button2_Click(object sender, EventArgs e)
        {
            string selectedPerformance = guna2ComboBox1.Text;
            DateTime startDate = guna2DateTimePicker1.Value.Date;
            DateTime endDate = guna2DateTimePicker2.Value.Date;

            // Проверка, выбрал ли пользователь спектакль
            if (string.IsNullOrEmpty(selectedPerformance))
            {
                MessageBox.Show("Выберите спектакль.");
                return;
            }

            // Проверка, выбраны ли даты
            if (startDate > endDate)
            {
                MessageBox.Show("Дата начала не может быть позже даты окончания.");
                return;
            }

            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT t.id, t.username, t.seats, t.total_price, t.status, v.date, v.time
                        FROM teatr.tickets t
                        JOIN teatr.viewing v ON t.viewimg_id = v.id
                        JOIN teatr.performance p ON v.performance_id = p.id
                        WHERE p.name = @performanceName AND v.date BETWEEN @startDate AND @endDate
                        ORDER BY v.date, v.time";

                    var cmd = new NpgsqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("performanceName", selectedPerformance);
                    cmd.Parameters.AddWithValue("startDate", startDate);
                    cmd.Parameters.AddWithValue("endDate", endDate);

                    var adapter = new NpgsqlDataAdapter(cmd);
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt; // Отображаем данные в DataGridView
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
            }
        }

        // Кнопка для экспорта отчета в Excel
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.DataSource == null || ((DataTable)dataGridView1.DataSource).Rows.Count == 0)
            {
                MessageBox.Show("Нет данных для экспорта.");
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel Workbook|*.xlsx" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var dt = (DataTable)dataGridView1.DataSource;
                        using (var wb = new ClosedXML.Excel.XLWorkbook())
                        {
                            wb.Worksheets.Add(dt, "Отчет по билетам");
                            wb.SaveAs(sfd.FileName); // Сохраняем файл
                        }

                        MessageBox.Show("Отчёт успешно сохранён.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}");
                    }
                }
            }
        }

        // Кнопка для возврата к предыдущей форме (например, кассир)
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

        private void buyticket_Load(object sender, EventArgs e)
        {
            LoadPerformanceNames(); // Загружаем спектакли при загрузке формы
        }
    }
}
