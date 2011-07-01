using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using MBUtilities.Channel;

namespace MoronBot
{
    public partial class formMoronBot : Form
    {
        public MoronBot moronBot;

        BindingSource _bindingSourceChannels = new BindingSource();
        BindingSource _bindingSourceUsers = new BindingSource();

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

            _bindingSourceChannels.DataSource = ChannelList.Channels;
            _bindingSourceChannels.ListChanged += bindingSourceChannels_ListChanged;
            listChannels.DisplayMember = "Name";
            listChannels.DataSource = _bindingSourceChannels;

            listUsers.DisplayMember = "Nick";
            listUsers.DataSource = _bindingSourceUsers;

            moronBot.NickChanged += moronBot_NickChanged;
            moronBot.NewRawIRC += moronBot_NewRawIRC;
            moronBot.NewFormattedIRC += moronBot_NewFormattedIRC;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtIRC.Text = "";
            txtProgLog.Text = "";
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void moronBot_NewRawIRC(object sender, string text)
        {
            txtProgLog.Text += text + "\r\n";
        }

        private void txtProgLog_TextChanged(object sender, EventArgs e)
        {
            txtProgLog.SelectionStart = txtProgLog.Text.Length;
            txtProgLog.ScrollToCaret();
        }

        public void moronBot_NewFormattedIRC(object sender, string text)
        {
            txtIRC.Text += text + "\r\n";
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
                moronBot.Say(txtInput.Text, ChannelList.Channels[listChannels.SelectedIndex].Name);
                txtInput.Text = "";
            }
        }

        public void RefreshUsers()
        {
            _bindingSourceUsers.DataSource = null;
            _bindingSourceUsers.DataSource = ChannelList.Channels[listChannels.SelectedIndex].Users;
        }

        private void bindingSourceChannels_ListChanged(object sender, ListChangedEventArgs e)
        {
            RefreshUsers();
        }

        private void listChannels_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshUsers();
        }

        private void moronBot_NickChanged(object sender, string nick)
        {
            Text = nick;
        }

        private void listChannels_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.Graphics.DrawString(
                _bindingSourceChannels[e.Index].ToString(),
                new Font("Segoe UI", 8, FontStyle.Regular),
                new SolidBrush(Color.WhiteSmoke),
                e.Bounds
                );
            e.DrawFocusRectangle();
        }

        private void listUsers_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.Graphics.DrawString(
                _bindingSourceUsers[e.Index].ToString(),
                new Font("Segoe UI", 8, FontStyle.Regular),
                new SolidBrush(Color.WhiteSmoke),
                e.Bounds
                );
            e.DrawFocusRectangle();
        }
    }
}
