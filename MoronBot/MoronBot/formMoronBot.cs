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

        BindingList<Channel> _bindingListChannels;
        BindingList<User> _bindingListUsers;

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

            _bindingListChannels = ChannelList.Channels;
            listChannels.DataSource = _bindingListChannels;
            listChannels.DisplayMember = "Name";

            _bindingListUsers = new BindingList<User>();
            listUsers.DataSource = _bindingListUsers;
            listUsers.DisplayMember = "Nick";
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtIRC.Text = "";
            txtProgLog.Text = "";
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            moronBot.SaveXML("settings.xml");
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
                moronBot.Say(txtInput.Text, ChannelList.Channels[listChannels.SelectedIndex].Name);
                txtInput.Text = "";
            }
        }

        public void RefreshListBox()
        {
            //_bindingListChannels.ListChanged = 
            //_bindingListChannels.ResetBindings();
            //_bindingListUsers.ResetBindings();
            Refresh();
        }

        private void listChannels_SelectedIndexChanged(object sender, EventArgs e)
        {
            //_bindingListUsers = ChannelList.Channels[listChannels.SelectedIndex].Users;
            RefreshListBox();
        }

        private void listChannels_DrawItem(object sender, DrawItemEventArgs e)
        {
            //e.DrawBackground();
            //e.Graphics.DrawString(
            //    _bindingListChannels[e.Index].Name,
            //    new Font(FontFamily.GenericMonospace, 10, FontStyle.Regular),
            //    new SolidBrush(Color.WhiteSmoke),
            //    e.Bounds
            //    );
            //e.DrawFocusRectangle();
        }

        private void listUsers_DrawItem(object sender, DrawItemEventArgs e)
        {
            //e.DrawBackground();
            //e.Graphics.DrawString(
            //    _bindingListUsers[e.Index].Nick,
            //    new Font(FontFamily.GenericMonospace, 10, FontStyle.Regular),
            //    new SolidBrush(Color.WhiteSmoke),
            //    e.Bounds
            //    );
            //e.DrawFocusRectangle();
        }
    }
}
