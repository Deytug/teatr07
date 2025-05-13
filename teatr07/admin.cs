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
    public partial class admin: Form
    {
        private string _currentUser;
        private string _role;
        public admin(string username, string role)
        {
            InitializeComponent();
            _currentUser = username;
            _role = role;
        }

        private void admin_Load(object sender, EventArgs e)
        {
            InitializeComponent();
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            buyticket form1 = new buyticket(_currentUser, _role);
            form1.Show();
            this.Close();
        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            Acter buyticket = new Acter(_currentUser, _role);
            buyticket.Show();
            this.Close();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            PerformanceEdit form1 = new PerformanceEdit(_currentUser, _role);
            form1.Show();
            this.Close();
        }

        private void label7_Click(object sender, EventArgs e)
        {  
            this.Close();

          
            main mainForm = new main(); 
            mainForm.Show();

        }
    }
}
