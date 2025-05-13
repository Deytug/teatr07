using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

public class RoundedButton : Button
{
    public int CornerRadius { get; set; } = 30; // Радиус закругления

    protected override void OnPaint(PaintEventArgs pevent)
    {
        // Создаем путь для закругленной кнопки
        GraphicsPath path = new GraphicsPath();
        path.StartFigure();
        path.AddArc(0, 0, CornerRadius, CornerRadius, 180, 90);
        path.AddArc(Width - CornerRadius, 0, CornerRadius, CornerRadius, 270, 90);
        path.AddArc(Width - CornerRadius, Height - CornerRadius, CornerRadius, CornerRadius, 0, 90);
        path.AddArc(0, Height - CornerRadius, CornerRadius, CornerRadius, 90, 90);
        path.CloseFigure();

        this.Region = new Region(path); // Устанавливаем область кнопки

        // Рисуем кнопку
        using (SolidBrush brush = new SolidBrush(this.BackColor))
        {
            pevent.Graphics.FillPath(brush, path);
        }

        // Рисуем текст кнопки
        TextRenderer.DrawText(pevent.Graphics, this.Text, this.Font, this.ClientRectangle, this.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
    }
}
