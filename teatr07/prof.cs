using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Word = Microsoft.Office.Interop.Word;
using QRCoder;

using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace teatr07
{
    public partial class prof: Form
    {
        int dotCount = 0;
        int animationStep = 0;

        private string _username;
        private string _role;
        public prof(string username)
        {
            InitializeComponent();
            _username = username;
        }
      
        private void prof_Load(object sender, EventArgs e)
        {
            label1.Text = $"Логин: {_username}";
            LoadUserTickets();
        }
        private void LoadUserTickets()
        {
            using (var conn = new NpgsqlConnection("Host=172.20.7.53;Port=5432;Username=st3996;Password=pwd3996;Database=db3996_08"))
            {
                conn.Open();
                // Исправленный запрос - используем performance_id вместо performance_name
                string query = @"
            SELECT 
                p.name as performance_name, 
              
                t.seats, 
                t.total_price 
            FROM teatr.tickets t
            JOIN teatr.performance p ON t.performance_id = p.id
            WHERE t.username = @username";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("username", _username);
                    using (var reader = cmd.ExecuteReader())
                    {
                        StringBuilder ticketInfo = new StringBuilder();
                        while (reader.Read())
                        {
                            ticketInfo.AppendLine($"🎭 {reader["performance_name"]}");
                            ticketInfo.AppendLine($"🎟 Места: {reader["seats"]}");
                            ticketInfo.AppendLine($"💰 Сумма: {reader["total_price"]} руб.");
                            ticketInfo.AppendLine("-----------------------------");
                        }
                        richTextBox1.Text = ticketInfo.ToString();
                    }
                }
            }
        }
        private void buttonOpenBuyForm_Click(object sender, EventArgs e)
        {
            buy buyForm = new buy(_username,_role); // Передаём имя пользователя
            buyForm.Show();
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button38_Click(object sender, EventArgs e)
        {
            poster form4 = new poster(_username);
            form4.Show();
            this.Close();

        }

        private void label4_Click(object sender, EventArgs e)
        {
            poster form4 = new poster(_username);
            form4.Show();
            this.Close();
        } 
        private void label3_Click(object sender, EventArgs e)
        {
            try
            {
                string ticketText = richTextBox1.Text;
                string ticketId = Guid.NewGuid().ToString().Substring(0, 8);
                string purchaseDate = DateTime.Now.ToString("g");

                Word.Application wordApp = new Word.Application();
                wordApp.Visible = false;
                Word.Document doc = wordApp.Documents.Add();

                // Заголовок
                Word.Paragraph header = doc.Content.Paragraphs.Add();
                header.Range.Text = "Театр \"Мир театра\"";
                header.Range.Font.Size = 20;
                header.Range.Font.Bold = 1;
                header.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                header.Range.InsertParagraphAfter();

                // Вставка логотипа
                string logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "icons8-театр-96.png");
                if (File.Exists(logoPath))
                {
                    Word.Paragraph logoPara = doc.Content.Paragraphs.Add();
                    Word.InlineShape picture = logoPara.Range.InlineShapes.AddPicture(logoPath);
                    picture.Width = 80;
                    picture.Height = 80;
                    logoPara.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    logoPara.Range.InsertParagraphAfter();
                }

                // Информация о билете
                Word.Paragraph info = doc.Content.Paragraphs.Add();
                info.Range.Text = $"Билет №: {ticketId}\nДата покупки: {purchaseDate}\nАдрес театра: г. Великий Новгород, ул. Чудес, д. 11\n";
                info.Range.Font.Size = 12;
                info.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                info.Range.InsertParagraphAfter();

                // Основной текст билета
                Word.Paragraph details = doc.Content.Paragraphs.Add();
                details.Range.Text = "Информация о заказе:\n" + ticketText.Trim();
                details.Range.Font.Size = 12;
                details.Range.Font.Name = "Calibri";
                details.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                details.Range.InsertParagraphAfter();

                // Генерация и вставка QR-кода
                using (QRCodeGenerator qrGen = new QRCodeGenerator())
                {
                    var qrData = qrGen.CreateQrCode("Билет №: " + ticketId, QRCodeGenerator.ECCLevel.Q);
                    using (QRCode qrCode = new QRCode(qrData))
                    {
                        using (Bitmap qrBitmap = qrCode.GetGraphic(5)) // уменьшенный QR
                        {
                            string qrPath = Path.Combine(Path.GetTempPath(), "qr.png");
                            qrBitmap.Save(qrPath, ImageFormat.Png);

                            Word.Paragraph qrPara = doc.Content.Paragraphs.Add();
                            Word.InlineShape qrShape = qrPara.Range.InlineShapes.AddPicture(qrPath);
                            qrShape.Width = 80;
                            qrShape.Height = 80;
                            qrPara.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight;
                            qrPara.Range.InsertParagraphAfter();
                        }
                    }
                }

                // Сохраняем
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"Билет_{ticketId}.docx");
                doc.SaveAs(filePath);
                doc.Close();
                wordApp.Quit();

                MessageBox.Show("Билет успешно создан на рабочем столе!", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
       
        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {

           
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            // Закрыть текущую форму профиля
            this.Close();

            // Открыть форму входа или основную форму (например, MainForm)
            main mainForm = new main(); // или LoginForm
            mainForm.Show();
        }
    }
}