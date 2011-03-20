using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MoronBot
{
    public partial class formMoronBot : Form
    {
        MoronBot moronBot;
        private readonly BindingSource _bindingSourceChannels = new BindingSource();
        private readonly BindingSource _bindingSourceUsers = new BindingSource();

        public formMoronBot()
        {
            InitializeComponent();
        }

        private void formMoronBot_Load(object sender, EventArgs e)
        {
            txtIRC.Text = "";
            txtProgLog.Text = "";
            txtInput.Text = "";

            moronBot = new MoronBot();

            _bindingSourceChannels.DataSource = moronBot.Channels;
            listChannels.DataSource = _bindingSourceChannels;
            listChannels.DisplayMember = "Name";

            //_bindingSourceUsers.DataSource = moronBot.Channels[listChannels.SelectedIndex].Users;
            listUsers.DataSource = _bindingSourceChannels;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtIRC.Text = "";
            txtProgLog.Text = "";
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            moronBot.SaveXML("settings.xml");
            moronBot.Quit();
            Close();
        }

        public void txtProgLog_Update(string p_text)
        {
            txtProgLog.Text += p_text + "\r\n";
        }

        private void txtProgLog_TextChanged(object sender, EventArgs e)
        {
            txtProgLog.SelectionStart = txtProgLog.Text.Length;
            txtProgLog.ScrollToCaret();
        }

        public void txtIRC_Update(string p_text)
        {
            txtIRC.Text += p_text + "\r\n";
        }

        private void txtIRC_TextChanged(object sender, EventArgs e)
        {
            txtIRC.SelectionStart = txtIRC.Text.Length;
            txtIRC.ScrollToCaret();
        }

        private void txtInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                moronBot.Say(txtInput.Text, moronBot.Channels[listChannels.SelectedIndex].Name);
                txtInput.Text = "";
            }
        }

        public void RefreshListBox()
        {
            _bindingSourceChannels.ResetBindings(false);
            _bindingSourceUsers.ResetBindings(false);
            Refresh();
        }

        private void listChannels_SelectedIndexChanged(object sender, EventArgs e)
        {
            listUsers.DataSource = moronBot.Channels[listChannels.SelectedIndex].Users;
            RefreshListBox();
        }

        private void listChannels_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.Graphics.DrawString(
                moronBot.Channels[e.Index].Name,
                new Font(FontFamily.GenericMonospace, 10, FontStyle.Regular),
                new SolidBrush(Color.WhiteSmoke),
                e.Bounds
                );
            e.DrawFocusRectangle();
        }

        private void listUsers_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.Graphics.DrawString(
                moronBot.Channels[listChannels.SelectedIndex].Users[e.Index],
                new Font(FontFamily.GenericMonospace, 10, FontStyle.Regular),
                new SolidBrush(Color.WhiteSmoke),
                e.Bounds
                );
            e.DrawFocusRectangle();
        }
    }
}
