using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace teatr07
{
    public partial class PrintDialog: Form
    {
        public PrintDialog()
        {
            InitializeComponent();
            label1.Text = "Печать билета";
            timer1.Interval = 500;
            timer1.Tick += timer1_Tick;
        }
        private int dotCount = 0;
        private int animationStep = 0;


        private void PrintDialog_Load(object sender, EventArgs e)
        {

            label1.Text = "Печать билета";
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;

            timer1.Interval = 300;
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            dotCount = (dotCount + 1) % 4;
            label1.Text = "Печать билета" + new string('.', dotCount);

            if (progressBar1.Value < 100)
            {
                progressBar1.Value += 5; 
            }

            animationStep++;
            if (animationStep >= 20) 
            {
                timer1.Stop();
                label1.Text = "Билет отправлен на печать!";
                progressBar1.Value = 100;

                Task.Delay(2000).ContinueWith(_ =>
                {
                    this.Invoke((MethodInvoker)(() => this.Close()));
                });
            }
        }
    }
}
