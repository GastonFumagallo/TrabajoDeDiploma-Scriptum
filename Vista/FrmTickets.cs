using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Modelo;

namespace Vista
{
    public partial class FrmTickets : Form
    {
        private TicketVentaDTO ticketActual;
        public FrmTickets()
        {
            InitializeComponent();
            // Evitar ejecución de lógica que requiere datos en tiempo de diseño
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
                return;
        }

        public FrmTickets(TicketVentaDTO ticket) : this()
        {
            this.ticketActual = ticket;

            MostrarTicket();
        }

        private void MostrarTicket()
        {
            rtbTicket.Font = new Font("Consolas", 12);
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("========================================");
            sb.AppendLine();
            sb.AppendLine("           LIBRERIA SCRIPTUM");
            sb.AppendLine();
            sb.AppendLine("========================================");
            sb.AppendLine();

            sb.AppendLine($"Ticket: TK-{ticketActual.NumeroVenta:D6}");
            sb.AppendLine($"Venta N°: {ticketActual.NumeroVenta}");
            sb.AppendLine($"Fecha: {ticketActual.Fecha:dd/MM/yyyy HH:mm}");
            sb.AppendLine($"Cliente: {ticketActual.Cliente}");
            sb.AppendLine($"Método de Pago: {ticketActual.MetodoPago}");

            sb.AppendLine();
            sb.AppendLine("------------------------------------");

            foreach (var item in ticketActual.Detalles)
            {
                sb.AppendLine(item.Libro);
                sb.AppendLine($"   {item.Cantidad} x ${item.PrecioUnitario:N2} = ${item.Subtotal:N2}");
                sb.AppendLine();
            }

            sb.AppendLine("------------------------------------");
            sb.AppendLine($"TOTAL: ${ticketActual.Total:N2}");
            sb.AppendLine("------------------------------------");
            sb.AppendLine();
            sb.AppendLine("Gracias por su compra");

            rtbTicket.Text = sb.ToString();
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Font titulo = new Font("Arial", 18, FontStyle.Bold);
            Font texto = new Font("Consolas", 11);
            Font total = new Font("Consolas", 12, FontStyle.Bold);

            float y = 50;

            StringFormat centrado = new StringFormat();
            centrado.Alignment = StringAlignment.Center;

            Image logo = Properties.Resources.scriptumLogo;


            e.Graphics.DrawImage(
                logo,
                (e.PageBounds.Width - 100) / 2,
                (int)y,
                100,
                100);

            y += 120;


            // Encabezado
            e.Graphics.DrawString(
                "LIBRERIA SCRIPTUM",
                titulo,
                Brushes.Black,
                new RectangleF(0, y, e.PageBounds.Width, 30),
                centrado);

            y += 50;

            e.Graphics.DrawString(
                $"Ticket: TK-{ticketActual.NumeroVenta:D6}",
                texto,
                Brushes.Black,
                50,
                y);

            y += 25;

            e.Graphics.DrawString(
                $"Fecha: {ticketActual.Fecha:dd/MM/yyyy HH:mm}",
                texto,
                Brushes.Black,
                50,
                y);

            y += 25;

            e.Graphics.DrawString(
                $"Cliente: {ticketActual.Cliente}",
                texto,
                Brushes.Black,
                50,
                y);

            y += 25;

            e.Graphics.DrawString(
                $"Método de Pago: {ticketActual.MetodoPago}",
                texto,
                Brushes.Black,
                50,
                y);

            y += 35;

            e.Graphics.DrawString(
                new string('-', 60),
                texto,
                Brushes.Black,
                50,
                y);

            y += 30;

            // Libros
            foreach (var item in ticketActual.Detalles)
            {
                e.Graphics.DrawString(
                    item.Libro,
                    texto,
                    Brushes.Black,
                    50,
                    y);

                y += 20;

                e.Graphics.DrawString(
                    $"{item.Cantidad} x ${item.PrecioUnitario:N2} = ${item.Subtotal:N2}",
                    texto,
                    Brushes.Black,
                    70,
                    y);

                y += 35;
            }

            e.Graphics.DrawString(
                new string('-', 60),
                texto,
                Brushes.Black,
                50,
                y);

            y += 30;

            e.Graphics.DrawString(
                $"TOTAL: ${ticketActual.Total:N2}",
                total,
                Brushes.Black,
                50,
                y);

            y += 50;

            e.Graphics.DrawString(
                "Gracias por su compra",
                texto,
                Brushes.Black,
                new RectangleF(0, y, e.PageBounds.Width, 30),
                centrado);
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            try
            {
                using (PrintPreviewDialog preview = new PrintPreviewDialog())
                {
                    preview.Document = printDocument1;
                    preview.ShowDialog();
                }
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                MessageBox.Show("La impresión fue cancelada.","Impresión",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
