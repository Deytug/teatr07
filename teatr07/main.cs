using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace teatr07
{
    public partial class main : Form
    {
        public main()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            avtoriz form1 = new avtoriz();
            form1.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            reg form1 = new reg();
            form1.Show();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void main_Load(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            avtoriz form1 = new avtoriz();
            form1.Show();
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            reg form1 = new reg();
            form1.Show();
        }
    }
}
