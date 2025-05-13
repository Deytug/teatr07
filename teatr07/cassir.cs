using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using teatr07;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace teatr07
{
    public partial class cassir : Form
    {
        private string _currentUser;
        private string _role;

        public cassir(string username, string role)
        {
            InitializeComponent();
            _currentUser = username;
            _role = role;

        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            buyticket buyticket = new buyticket(_currentUser, _role);
            buyticket.Show();
            this.Close();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            PerformanceEdit form1 = new PerformanceEdit(_currentUser, _role);
            form1.Show();
            this.Close();
        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            buy form5 = new buy(_currentUser, _role);
            form5.Show();
            this.Close();
        }

        private void guna2Button4_Click_1(object sender, EventArgs e)
        {

            buy form5 = new buy(_currentUser, _role);
            form5.Show();
            this.Close();
        }

        private void cassir_Load(object sender, EventArgs e)
        {
            InitializeComponent();
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            Acter Acter = new Acter(_currentUser, _role);
            Acter.Show();
            this.Close();
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {
            
            this.Close();

            // Открыть форму входа или основную форму (например, MainForm)
            main mainForm = new main(); // или LoginForm
            mainForm.Show();
        }
    }
}