using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net.Mail;

namespace Najm
{
    public partial class ReportBug : Form
    {
        public ReportBug()
        {
            InitializeComponent();
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            Cursor orig = Cursor;
            Cursor = Cursors.WaitCursor;
            try
            {
                string msg = string.IsNullOrEmpty(emailAddressTextBox.Text) ? commentTextBox.Text : (emailAddressTextBox.Text + "\n\n" + commentTextBox.Text);
                MailMessage mm = new MailMessage("snowprison@gmail.com", "farbluestar@gmail.com", "NAJM.Bug", msg);
                SmtpClient c = new SmtpClient("smtp.gmail.com");
                c.EnableSsl = true;
                c.Credentials = new System.Net.NetworkCredential("snowprison", "A!B@C#D$");
                c.Send(mm);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Cursor = orig;
            }
        }
    }
}
