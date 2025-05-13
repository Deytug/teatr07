using System;
using System.Drawing;
using System.Windows.Forms;
using Npgsql;
using System.Runtime.InteropServices;

namespace teatr07
{
    public partial class poster : Form
    {
        string connectionString = "Host=172.20.7.53;Port=5432;Username=st3996;Password=pwd3996;Database=db3996_08";
        private string _currentUser;
        private string _role;
        public poster(string username)
        {
            InitializeComponent();
            _currentUser = username;
            flowLayoutPanel1.MouseEnter += (s, e) => flowLayoutPanel1.Focus();
        }

        private void poster_Load(object sender, EventArgs e)
        {
            LoadPerformanceLabels();
        }

        private void LoadPerformanceLabels()
        {
            flowLayoutPanel1.Controls.Clear();
            flowLayoutPanel1.FlowDirection = FlowDirection.LeftToRight;
            flowLayoutPanel1.WrapContents = true;
            flowLayoutPanel1.AutoScroll = true;

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                // Исправленный запрос - берем name из performance, а не viewing
                string query = @"
                    SELECT p.id, p.name 
                    FROM teatr.performance p
                    JOIN teatr.viewing v ON p.id = v.performance_id
                    GROUP BY p.id, p.name
                    ORDER BY MIN(v.date), MIN(v.time)";

                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    int index = 1;
                    while (reader.Read())
                    {
                        int perfId = reader.GetInt32(0);
                        string perfName = reader.GetString(1);

                        Panel card = new Panel();
                        card.Size = new Size(200, 270);
                        card.BackColor = Color.White;
                        card.Margin = new Padding(20);
                        card.Padding = new Padding(10);

                        // Скругление углов панели
                        card.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, card.Width, card.Height, 20, 20));

                        // Обводка и тень для панели
                        card.Paint += (s, ev) =>
                        {
                            ControlPaint.DrawBorder(ev.Graphics, card.ClientRectangle,
                                Color.Gray, 2, ButtonBorderStyle.Solid,
                                Color.Gray, 2, ButtonBorderStyle.Solid,
                                Color.Gray, 2, ButtonBorderStyle.Solid,
                                Color.Gray, 2, ButtonBorderStyle.Solid);
                        };

                        // Изображение спектакля
                        PictureBox picture = new PictureBox();
                        picture.Size = new Size(160, 160);
                        picture.Location = new Point((card.Width - picture.Width) / 2, 10);
                        picture.SizeMode = PictureBoxSizeMode.Zoom;
                        picture.BackColor = Color.White;

                        // Загружаем изображение для каждого спектакля по индексу
                        Image rawImage = GetImageByName($"perf{index}");
                        if (rawImage != null)
                        {
                            picture.Image = ResizeImage(rawImage, 160, 160);
                        }

                        picture.Cursor = Cursors.Hand;
                        picture.Click += (s, e) =>
                        {
                            spek spekForm = new spek(_currentUser, perfId);
                            spekForm.Show();
                            this.Close();
                        };

                        // Название спектакля
                        Label nameLabel = new Label();
                        nameLabel.Text = perfName;
                        nameLabel.Font = new Font("Segoe UI", 11, FontStyle.Bold);
                        nameLabel.TextAlign = ContentAlignment.MiddleCenter;
                        nameLabel.Dock = DockStyle.Bottom;
                        nameLabel.ForeColor = Color.FromArgb(40, 40, 40);
                        nameLabel.Height = 100;

                        // Добавляем элементы в панель
                        card.Controls.Add(picture);
                        card.Controls.Add(nameLabel);
                        flowLayoutPanel1.Controls.Add(card);

                        index++;
                    }
                }
            }
        }

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect,
            int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        private Image GetImageByName(string name)
        {
            switch (name)
            {
                case "perf1": return Properties.Resources.img1;
                case "perf2": return Properties.Resources.img3;
                case "perf3": return Properties.Resources.img2;
                case "perf4": return Properties.Resources.img44;
                case "perf5": return Properties.Resources.img55;
                case "perf6": return Properties.Resources.img6;
                default: return null;
            }
        }

        private Image ResizeImage(Image image, int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(image, 0, 0, width, height);
            }
            return bmp;
        }

        // Остальные методы оставляем без изменений
        private void button1_Click(object sender, EventArgs e)
        {
            prof form6 = new prof(_currentUser);
            form6.Show();
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            buy form5 = new buy(_currentUser,_role);
            form5.Show();
            this.Close();
        }


        private void guna2PictureBox3_Click(object sender, EventArgs e)
        {
            spek spekForm = new spek(_currentUser, 3);
            spekForm.Show();
            this.Close();
        }

        private void guna2PictureBox1_Click_1(object sender, EventArgs e)
        {
            spek spekForm = new spek(_currentUser, 1);
            spekForm.Show();
            this.Close();
        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2PictureBox2_Click_1(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {
            buy form5 = new buy(_currentUser, _role);
            form5.Show();
            this.Close();
        }
    }
}