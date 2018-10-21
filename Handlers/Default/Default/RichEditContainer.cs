using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Najm.FITSIO;

namespace Default
{
    internal partial class RichEditContainer : UserControl
    {
        internal RichEditContainer()
        {
            InitializeComponent();
        }

        internal void DisplayHDUHeader(IHDU hdu)
        {
            FillSummary(hdu);
            FillDetailsListView(hdu);
            FillWCS(hdu);
        }

        private void FillSummary(IHDU hdu)
        {
            richTextBox.Clear();
            Font oldFont = richTextBox.SelectionFont;
            Font boldFont = new Font(richTextBox.SelectionFont, FontStyle.Bold | FontStyle.Underline);
            richTextBox.SelectionFont = boldFont;
            richTextBox.AppendText("Summary:\n");
            richTextBox.SelectionFont = oldFont;
            richTextBox.AppendText(hdu.Header.SummaryText + "\n");
            richTextBox.SelectionFont = oldFont;
            // well, sometimes the scroll is far down for some reason ans sometimes is up as expected.
            // So, I'm scrolling all the way up now.
            richTextBox.SelectionStart = 0;
            richTextBox.ScrollToCaret();
        }

        private void FillDetailsListView(IHDU hdu)
        {
            headerListView.Items.Clear();
            headerListView.Font = new Font(headerListView.Font.FontFamily, 11F);
            for (uint i = 0; i < hdu.Header.CardImages.Length; i++)
            {
                ICardImage ci = hdu.Header.CardImages[i];
                if (!ci.IsEmpty)
                {
                    ListViewItem item = new ListViewItem(new string[] { ci.Key, ci.Value, ci.Comment });
                    item.UseItemStyleForSubItems = false;
                    if (ci.IsComment)
                    {
                        item.SubItems[0].ForeColor = Color.DarkGreen;
                    }
                    else if (ci.IsHistory)
                    {
                        item.SubItems[0].ForeColor = Color.SaddleBrown;
                    }
                    else if (ci.IsWCS)
                    {
                        item.SubItems[0].ForeColor = Color.Blue;
                    }
                    else if (ci.IsTable)
                    {
                        item.SubItems[0].ForeColor = Color.FromArgb(255, 64, 128, 128);
                    }
                    Color c = ((i & 1) != 0) ? Color.FromArgb(255, 240, 240, 240) : item.BackColor;
                    item.SubItems[0].BackColor = item.SubItems[1].BackColor = item.SubItems[2].BackColor = c;

                    item.SubItems[0].Font = new Font(item.SubItems[0].Font, FontStyle.Bold);
                    headerListView.Items.Add(item);
                }
            }
        }

        void FillWCS(IHDU hdu)
        {
            string rtf =
            @"{\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{\f0\fswiss\fcharset0 Microsoft Sans Serif;}}" + "\n" +
            @"{\*\generator Najm 1.0 beta;}\viewkind4\uc1\pard\f0\fs22" + "\n";
            rtf += hdu.Header.WCSInfo.ToString();
            wcsRichTextBox.Rtf = rtf;
        }
    }
}
