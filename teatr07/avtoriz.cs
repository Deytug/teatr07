using Guna.UI2.WinForms;
using Npgsql;
using System;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace teatr07
{
    public partial class avtoriz : Form
    {

        public avtoriz()
        {
            InitializeComponent();
           
        }
        private void button2_Click(object sender, EventArgs e)
        {
            main form1 = new main();
            form1.Show();
            this.Close();
        }
        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void avtoriz_Load(object sender, EventArgs e)
        {
            guna2TextBox2.PasswordChar = '*'; // для поля "Пароль"
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            main form1 = new main();
            form1.Show();
            this.Close();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            main form1 = new main();
            form1.Show();
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {

        }

       

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            string connectionString = "Host=172.20.7.53;Port=5432;Username=st3996;Password=pwd3996;Database=db3996_08";
            string username = guna2TextBox1.Text;
            string password = guna2TextBox2.Text;
            string hashedPassword = HashPassword(password);

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT role FROM teatr.ausers WHERE username = @username AND password = @password";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("username", username);
                    cmd.Parameters.AddWithValue("password", hashedPassword);
                    var role = cmd.ExecuteScalar() as string;
                    if (role == null)
                    {
                        MessageBox.Show("Неверный логин или пароль!");
                        return;
                    }

                    MessageBox.Show($"Вход выполнен! Роль: {role}");
                    Form nextForm = null;

                    if (role == "admin")
                        nextForm = new admin(username,role);
                    else if (role == "cashier")
                        nextForm = new cassir(username, role);
                    else if (role == "user")
                        nextForm = new poster(username);

                    if (nextForm != null)
                    {
                        nextForm.Show();
                        this.Hide();
                    }

                }
            }
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

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
           
          
        }
        private bool passwordVisible = false;

        private void TogglePasswordVisibility()
        {
            passwordVisible = !passwordVisible;
            guna2TextBox2.PasswordChar = passwordVisible ? '\0' : '*';
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            TogglePasswordVisibility();
        }

        private void label4_Click_1(object sender, EventArgs e)
        {
            main main = new main();
            main.Show();
            this.Close();
        }
    }
}
   
