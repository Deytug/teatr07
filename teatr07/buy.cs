using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Npgsql;

namespace teatr07
{
    public partial class buy : Form
    {
        private string _username;
        private string connectionString = "Host=172.20.7.53;Port=5432;Username=st3996;Password=pwd3996;Database=db3996_08";
        private HashSet<int> selectedSeats = new HashSet<int>();
        private Dictionary<int, Button> seatButtons = new Dictionary<int, Button>();
        private int totalCost = 0;
        private string actionType = ""; // "buy" или "reserve"
        private string _role;
        public buy(string username, string role)
        {
            InitializeComponent();
            _username = username;
            _role = role;
            MapSeatButtons();
            LoadPerformances();
        }

        private void MapSeatButtons()
        {
            for (int i = 1; i <= 36; i++)
            {
                Button btn = Controls.Find($"button{i}", true).FirstOrDefault() as Button;
                if (btn != null)
                {
                    btn.Click += SeatButton_Click;
                    seatButtons[i] = btn;
                }
            }
        }

        private void LoadPerformances()
        {
            guna2ComboBox1.Items.Clear();
            guna2ComboBox2.Items.Clear();

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT DISTINCT name FROM teatr.performance ORDER BY name";
                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        guna2ComboBox1.Items.Add(reader["name"].ToString());
                }
            }

            guna2ComboBox1.SelectedIndexChanged += guna2ComboBox1_SelectedIndexChanged;
            guna2ComboBox2.SelectedIndexChanged += guna2ComboBox2_SelectedIndexChanged;
        }

        private void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            guna2ComboBox2.Items.Clear();
            if (guna2ComboBox1.SelectedItem == null) return;

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
                    SELECT v.date, v.time 
                    FROM teatr.viewing v
                    JOIN teatr.performance p ON v.performance_id = p.id
                    WHERE p.name = @name 
                    ORDER BY v.date, v.time";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("name", guna2ComboBox1.SelectedItem.ToString());
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DateTime date = reader.GetDateTime(0);
                            TimeSpan time = reader.GetTimeSpan(1);
                            guna2ComboBox2.Items.Add($"{date:yyyy-MM-dd} {time:hh\\:mm}");
                        }
                    }
                }
            }
        }

        private void guna2ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (guna2ComboBox1.SelectedItem == null || guna2ComboBox2.SelectedItem == null)
                return;

            selectedSeats.Clear();
            totalCost = 0;
            textBox1.Text = "0";

            foreach (var btn in seatButtons.Values)
            {
                btn.Enabled = true;
                btn.BackColor = Color.Black;
            }

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string[] dateTimeParts = guna2ComboBox2.SelectedItem.ToString().Split(' ');
                DateTime showDate = DateTime.Parse(dateTimeParts[0]);
                TimeSpan showTime = TimeSpan.Parse(dateTimeParts[1]);

                int viewingId = 0;
                using (var cmd = new NpgsqlCommand(@"
                    SELECT v.id 
                    FROM teatr.viewing v
                    JOIN teatr.performance p ON v.performance_id = p.id
                    WHERE p.name = @name AND v.date = @date AND v.time = @time", conn))
                {
                    cmd.Parameters.AddWithValue("name", guna2ComboBox1.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("date", showDate);
                    cmd.Parameters.AddWithValue("time", showTime);
                    viewingId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                if (viewingId > 0)
                    LoadSeats(conn, viewingId);

                textBox1.Text = totalCost.ToString();
            }
        }
        private void LoadSeats(NpgsqlConnection conn, int viewingId)
        {
            // Сначала получаем performance_id из viewing
            int performanceId = 0;
            using (var cmd = new NpgsqlCommand(
                "SELECT performance_id FROM teatr.viewing WHERE id = @vid", conn))
            {
                cmd.Parameters.AddWithValue("vid", viewingId);
                performanceId = Convert.ToInt32(cmd.ExecuteScalar());
            }
            using (var cmd = new NpgsqlCommand(
                "SELECT seat_number, is_booked, booked_by FROM teatr.seat WHERE performance_id = @pid", conn))
            {
                cmd.Parameters.AddWithValue("pid", performanceId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int seatNumber = Convert.ToInt32(reader["seat_number"]);
                        bool isBooked = Convert.ToBoolean(reader["is_booked"]);
                        string bookedBy = reader["booked_by"]?.ToString();

                        if (seatButtons.TryGetValue(seatNumber, out Button btn))
                        {
                            if (isBooked)
                            {
                                btn.BackColor = Color.Red;
                                btn.Enabled = false;
                            }
                            else
                            {
                                btn.BackColor = Color.Black;
                                btn.Enabled = true;
                            }
                        }
                    }
                }
            }
        }


        private void SeatButton_Click(object sender, EventArgs e)
        {
            if (guna2ComboBox1.SelectedItem == null || guna2ComboBox2.SelectedItem == null)
                return;

            Button btn = (Button)sender;
            int seatNumber = int.Parse(btn.Text);

            if (btn.BackColor == Color.Red)
            {
                MessageBox.Show("Это место уже занято другим пользователем.");
                return;
            }

            if (selectedSeats.Contains(seatNumber))
            {
                selectedSeats.Remove(seatNumber);
                totalCost -= (seatNumber <= 28) ? 700 : 1000;
                btn.BackColor = Color.Black;
            }
            else
            {
                selectedSeats.Add(seatNumber);
                totalCost += (seatNumber <= 28) ? 700 : 1000;
                btn.BackColor = actionType == "buy" ? Color.Green : Color.Orange;
            }

            textBox1.Text = totalCost.ToString();
        }

        private void guna2Button1_Click_1(object sender, EventArgs e)
        {
            if (guna2ComboBox1.SelectedItem == null || guna2ComboBox2.SelectedItem == null || selectedSeats.Count == 0)
            {
                MessageBox.Show("Выберите спектакль, дату и места.");
                return;
            }

            var result = MessageBox.Show("Вы хотите КУПИТЬ билет? (Да — купить, Нет — забронировать)", "Подтверждение", MessageBoxButtons.YesNoCancel);

            if (result == DialogResult.Cancel) return;
            actionType = result == DialogResult.Yes ? "buy" : "reserve";

            string perfName = guna2ComboBox1.SelectedItem.ToString();
            string[] dateTimeParts = guna2ComboBox2.SelectedItem.ToString().Split(' ');
            DateTime showDate = DateTime.Parse(dateTimeParts[0]);
            TimeSpan showTime = TimeSpan.Parse(dateTimeParts[1]);

            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    // 1. Сначала получаем ID спектакля
                    int perfId = 0;
                    using (var cmd = new NpgsqlCommand(
                        "SELECT id FROM teatr.performance WHERE name = @name", conn))
                    {
                        cmd.Parameters.AddWithValue("name", perfName);
                        perfId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    if (perfId == 0)
                    {
                        MessageBox.Show("Не удалось найти выбранный спектакль.");
                        return;
                    }

                    // 2. Проверяем, что места не заняты другим пользователем
                    foreach (var seat in selectedSeats)
                    {
                        using (var cmd = new NpgsqlCommand(
                            "SELECT is_booked, booked_by FROM teatr.seat WHERE performance_id = @pid AND seat_number = @sn", conn))
                        {
                            cmd.Parameters.AddWithValue("pid", perfId);
                            cmd.Parameters.AddWithValue("sn", seat);
                            using (var reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    bool isBooked = Convert.ToBoolean(reader["is_booked"]);
                                    string bookedBy = reader["booked_by"]?.ToString();

                                    if (isBooked && bookedBy != _username)
                                    {
                                        MessageBox.Show($"Место {seat} уже занято другим пользователем.");
                                        return;
                                    }
                                }
                            }
                        }
                    }

                    // 3. Получаем ID просмотра (viewing)
                    int viewingId = 0;
                    using (var cmd = new NpgsqlCommand(@"
                SELECT id 
                FROM teatr.viewing 
                WHERE performance_id = @pid AND date = @date AND time = @time", conn))
                    {
                        cmd.Parameters.AddWithValue("pid", perfId);
                        cmd.Parameters.AddWithValue("date", showDate);
                        cmd.Parameters.AddWithValue("time", showTime);
                        viewingId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    if (viewingId == 0)
                    {
                        MessageBox.Show("Не удалось найти выбранный сеанс.");
                        return;
                    }

                    // 4. Вставляем билет
                    int ticketId = 0;
                    using (var cmd = new NpgsqlCommand(@"
                INSERT INTO teatr.tickets(
                    username, 
                    performance_id, 
                    viewimg_id,
                    seats, 
                    total_price,
                    status) 
                VALUES(@u, @pid, @vid, @s, @price, @status) 
                RETURNING id", conn))
                    {
                        cmd.Parameters.AddWithValue("u", _username);
                        cmd.Parameters.AddWithValue("pid", perfId);
                        cmd.Parameters.AddWithValue("vid", viewingId);
                        cmd.Parameters.AddWithValue("s", string.Join(", ", selectedSeats));
                        cmd.Parameters.AddWithValue("price", totalCost);
                        cmd.Parameters.AddWithValue("status", actionType);
                        ticketId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // 5. Обновляем места
                    foreach (var seat in selectedSeats)
                    {
                        using (var cmd = new NpgsqlCommand(@"
                    UPDATE teatr.seat 
                    SET is_booked = TRUE, 
                        booked_by = @user, 
                        status = @status, 
                        ticket_id = @ticketId 
                    WHERE performance_id = @pid AND seat_number = @sn", conn))
                        {
                            cmd.Parameters.AddWithValue("user", _username);
                            cmd.Parameters.AddWithValue("status", actionType);
                            cmd.Parameters.AddWithValue("ticketId", ticketId);
                            cmd.Parameters.AddWithValue("pid", perfId);
                            cmd.Parameters.AddWithValue("sn", seat);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                MessageBox.Show($"Билет успешно {(actionType == "buy" ? "куплен" : "забронирован")}!\n\n" +
                                $"Спектакль: {perfName}\nДата: {showDate:dd.MM.yyyy}\nВремя: {showTime:hh\\:mm}\n" +
                                $"Места: {string.Join(", ", selectedSeats)}\nСумма: {totalCost} руб.",
                                "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                guna2ComboBox2_SelectedIndexChanged(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button38_Click(object sender, EventArgs e)
        {
            poster poster = new poster(_username);
            poster.Show();
            this.Close();
        }

        private void buy_Load(object sender, EventArgs e)
        {
            // Получаем роль пользователя (например, из базы данных)
            string userRole = GetUserRole(_username); // Получаем роль пользователя по имени (передаем имя пользователя в метод)

            // Если роль - 'cashier', показываем кнопку, иначе скрываем
            if (userRole == "cashier")
            {
                label11.Visible = true;
            }
            else
            {
                label11.Visible = false;
            }
        }
        private string GetUserRole(string username)
        {
            string role = "";

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT role FROM teatr.ausers WHERE username = @username";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("username", username);
                    role = cmd.ExecuteScalar()?.ToString();
                }
            }

            return role;
        }
        private void label11_Click(object sender, EventArgs e)
        {
            cassir cashierForm = new cassir(_username, _role);  // Передаем имя пользователя
            cashierForm.Show();
            this.Close(); // Закрываем текущую форму (если нужно)

        }
    }
}