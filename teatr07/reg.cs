using Npgsql;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace teatr07
{
    public partial class reg : Form
    {
        string connectionString = "Host=172.20.7.53;Port=5432;Username=st3996;Password=pwd3996;Database=db3996_08";
        public reg()
        {
            InitializeComponent();
        }
        private void button1_Click_1(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            main form1 = new main();
            form1.Show();
            this.Close();
        }

        private void reg_Load(object sender, EventArgs e)
        {
            guna2TextBox2.PasswordChar = '*'; // для поля "Пароль"
            guna2TextBox3.PasswordChar = '*'; // для поля "Подтверждение пароля"
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            string connectionString = "Host=172.20.7.53;Port=5432;Username=st3996;Password=pwd3996;Database=db3996_08";
            string username = guna2TextBox1.Text;
            string password = guna2TextBox2.Text;
            string confirmPassword = guna2TextBox3.Text;

            if (!IsUsernameValid(username))
            {
                MessageBox.Show("Логин не может состоять только из цифр.");
                return;
            }

            if (!IsPasswordStrong(password))
            {
                MessageBox.Show("Пароль слишком простой. Используйте минимум 8 символов, заглавные и строчные буквы и цифры.");
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают!");
                return;
            }

            string hashedPassword = HashPassword(password);

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO teatr.ausers (username, password, role) VALUES (@username, @password, 'user')";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("username", username);
                    cmd.Parameters.AddWithValue("password", hashedPassword);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show($"Регистрация успешна!");
                        poster form4 = new poster(username);
                        form4.Show();
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка регистрации: {ex.Message}");
                    }
                }
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {
            main form1 = new main();
            form1.Show();
            this.Close();
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
        private bool IsPasswordStrong(string password)
        {
            if (password.Length < 8) return false;
            bool hasUpper = false, hasLower = false, hasDigit = false;
            foreach (char c in password)
            {
                if (char.IsUpper(c)) hasUpper = true;
                if (char.IsLower(c)) hasLower = true;
                if (char.IsDigit(c)) hasDigit = true;
            }
            return hasUpper && hasLower && hasDigit;
        }
        private bool IsUsernameValid(string username)
        {
            return !System.Text.RegularExpressions.Regex.IsMatch(username, @"^\d+$");
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private bool passwordVisible = false;

        private void TogglePasswordVisibility()
        {
            passwordVisible = !passwordVisible;
            guna2TextBox2.PasswordChar = passwordVisible ? '\0' : '*';
            guna2TextBox3.PasswordChar = passwordVisible ? '\0' : '*';
        }

        private void button3_Click(object sender, EventArgs e)
        {
            TogglePasswordVisibility();


        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            TogglePasswordVisibility();
        }
    }
}


